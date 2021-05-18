using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGL.RenderContextProviders
{
	public class NativeFBORenderContextProvider : RenderContextProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NativeFBORenderContextProvider"/> class.
		/// </summary>
		public NativeFBORenderContextProvider()
		{
			//  We cannot layer GDI drawing on top of open gl drawing.
			GDIDrawingEnabled = false;
		}

		/// <summary>
		/// Creates the render context provider. Must also create the OpenGL extensions.
		/// </summary>
		/// <param name="gl">The OpenGL context.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="bitDepth">The bit depth.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns></returns>
		public override bool Create(OpenGL gl, int width, int height, int bitDepth, object parameter)
		{
			this.gl = gl;

			//  Call the base.
			base.Create(gl, width, height, bitDepth, parameter);

			//  Cast the parameter to the device context.
			try
			{
				windowHandle = (IntPtr)parameter;
			}
			catch
			{
				throw new Exception("A valid Window Handle must be provided for the NativeWindowRenderContextProvider");
			}

			//	Get the window device context.
			deviceContextHandle = Win32.GetDC(windowHandle);

			//	Setup a pixel format.
			Win32.PIXELFORMATDESCRIPTOR pfd = new Win32.PIXELFORMATDESCRIPTOR();
			pfd.Init();
			pfd.nVersion = 1;
			pfd.dwFlags = Win32.PFD_DRAW_TO_WINDOW | Win32.PFD_SUPPORT_OPENGL | Win32.PFD_DOUBLEBUFFER;
			pfd.iPixelType = Win32.PFD_TYPE_RGBA;
			pfd.cColorBits = 24;
			pfd.cDepthBits = 24;
			pfd.cStencilBits = 8;
			pfd.cAlphaBits = 8;
			pfd.cAuxBuffers = 0;
			pfd.iLayerType = Win32.PFD_MAIN_PLANE;
			pfd.cAccumBits = 0;

			//	Match an appropriate pixel format 
			int iPixelformat;
			if ((iPixelformat = Win32.ChoosePixelFormat(deviceContextHandle, pfd)) == 0)
				return false;

			//	Sets the pixel format
			if (Win32.SetPixelFormat(deviceContextHandle, iPixelformat, pfd) == 0)
			{
				return false;
			}

			//	Create the render context.
			renderContextHandle = Win32.wglCreateContext(deviceContextHandle);
			
			//  Make the context current.
			MakeCurrent();

			multiSampleExtensionsAvailable = gl.IsExtensionFunctionSupported("glRenderbufferStorageMultisampleEXT") && gl.IsExtensionFunctionSupported("glBlitFramebufferEXT");

			CreateFramebuffers();

			//  Return success.
			return true;
		}

		public int MultiSamples
		{
			get
			{
				return multiSamples;
			}
			set
			{
				if (multiSamples != value)
				{
					multiSamples = value;

					DestroyFramebuffers();

					CreateFramebuffers();
				}
			}
		}

		private void CreateFramebuffers()
		{
			fboInitialized = false;

			if (MultiSamples > 1 && multiSampleExtensionsAvailable)
			{
				uint[] ids = new uint[1];

				// Create multisample FBO
				ids = new uint[1];
				gl.GenFramebuffersEXT(1, ids);
				multiSampleFrameBufferID = ids[0];
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, multiSampleFrameBufferID);

				// Color
				gl.GenRenderbuffersEXT(1, ids);
				multiSampleColourRenderBufferID = ids[0];
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, multiSampleColourRenderBufferID);
				gl.RenderbufferStorageMultisampleEXT(OpenGL.GL_RENDERBUFFER_EXT, MultiSamples, OpenGL.GL_RGBA8, Width, Height);
				gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_RENDERBUFFER_EXT, multiSampleColourRenderBufferID);

				// Depth
				gl.GenRenderbuffersEXT(1, ids);
				multiSampleDepthRenderBufferID = ids[0];
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, multiSampleDepthRenderBufferID);
				gl.RenderbufferStorageMultisampleEXT(OpenGL.GL_RENDERBUFFER_EXT, MultiSamples, OpenGL.GL_DEPTH_COMPONENT24, Width, Height);
				gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, multiSampleDepthRenderBufferID);

				// Create normal FBO (which we can render to screen)
				gl.GenFramebuffersEXT(1, ids);
				frameBufferID = ids[0];
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBufferID);

				// Color
				uint[] color_tex = new uint[1];
				gl.GenTextures(1, color_tex);
				colourRenderBufferID = color_tex[0];
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, colourRenderBufferID);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
				gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
				gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA8, Width, Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, null);
				gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, colourRenderBufferID, 0);

				//	Depth
				gl.GenRenderbuffersEXT(1, ids);
				depthRenderBufferID = ids[0];
				gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, depthRenderBufferID);
				gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_DEPTH_COMPONENT24, Width, Height);
				gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, depthRenderBufferID);

				fboInitialized = true;
			}
		}

		private void DestroyFramebuffers()
		{
			if (fboInitialized)
			{
				//  Delete the render buffers.
				gl.DeleteRenderbuffersEXT(4, new uint[] { colourRenderBufferID, depthRenderBufferID, multiSampleColourRenderBufferID, multiSampleDepthRenderBufferID });

				//	Delete the framebuffer.
				gl.DeleteFramebuffersEXT(2, new uint[] { frameBufferID, multiSampleFrameBufferID });

				//  Reset the IDs.
				colourRenderBufferID = 0;
				depthRenderBufferID = 0;
				frameBufferID = 0;

				multiSampleColourRenderBufferID = 0;
				multiSampleDepthRenderBufferID = 0;
				multiSampleFrameBufferID = 0;
			}

			fboInitialized = false;
		}

		/// <summary>
		/// Destroys the render context provider instance.
		/// </summary>
		public override void Destroy()
		{
			//  Delete the render buffers.
			DestroyFramebuffers();

			//	Release the device context.
			Win32.ReleaseDC(windowHandle, deviceContextHandle);

			//	Call the base, which will delete the render context handle.
			base.Destroy();
		}

		/// <summary>
		/// Sets the dimensions of the render context provider.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public override void SetDimensions(int width, int height)
		{
			//  Call the base.
			base.SetDimensions(width, height);

			DestroyFramebuffers();

			CreateFramebuffers();
		}

		/// <summary>
		/// Blit the rendered data to the supplied device context.
		/// </summary>
		/// <param name="hdc">The HDC.</param>
		public override void Blit(IntPtr hdc)
		{
			if (MultiSamples > 1)
			{
				gl.BindFramebufferEXT(OpenGL.GL_READ_FRAMEBUFFER_EXT, multiSampleFrameBufferID);
				gl.BindFramebufferEXT(OpenGL.GL_DRAW_FRAMEBUFFER_EXT, frameBufferID);
				gl.BlitFramebufferEXT(0, 0, Width, Height, 0, 0, Width, Height, OpenGL.GL_COLOR_BUFFER_BIT, OpenGL.GL_NEAREST);

				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);

				gl.PushAttrib(OpenGL.GL_ENABLE_BIT);

				gl.Enable(OpenGL.GL_TEXTURE_2D);
				gl.Disable(OpenGL.GL_CULL_FACE);
				gl.Disable(OpenGL.GL_DEPTH_TEST);
				gl.Disable(OpenGL.GL_LIGHTING);
				gl.Enable(OpenGL.GL_COLOR_MATERIAL);
				gl.Disable(OpenGL.GL_BLEND);

				gl.MatrixMode(OpenGL.GL_PROJECTION);
				gl.LoadIdentity();
				gl.Ortho2D(0, 1, 0, 1);

				gl.MatrixMode(OpenGL.GL_MODELVIEW);
				gl.LoadIdentity();

				gl.BindTexture(OpenGL.GL_TEXTURE_2D, colourRenderBufferID);

				gl.Begin(OpenGL.GL_QUADS);

				gl.Color(1.0, 1.0, 1.0, 1.0);

				gl.Vertex(0, 0, 0);
				gl.TexCoord(1, 0);

				gl.Vertex(1, 0, 0);
				gl.TexCoord(1, 1);

				gl.Vertex(1, 1, 0);
				gl.TexCoord(0, 1);

				gl.Vertex(0, 1, 0);
				gl.TexCoord(0, 0);

				gl.End();

				gl.PopAttrib();
			}

			if (deviceContextHandle != IntPtr.Zero || windowHandle != IntPtr.Zero)
			{
				//	Swap the buffers.
				Win32.SwapBuffers(deviceContextHandle);
			}
		}

		/// <summary>
		/// Makes the render context current.
		/// </summary>
		public override void MakeCurrent()
		{
			if (renderContextHandle != IntPtr.Zero)
			{
				Win32.wglMakeCurrent(deviceContextHandle, renderContextHandle);
			}

			if (MultiSamples > 1)
			{
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, multiSampleFrameBufferID);
			}
			else
			{
				gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
			}
		}

		/// <summary>
		/// The window handle.
		/// </summary>
		protected IntPtr windowHandle = IntPtr.Zero;

		protected uint colourRenderBufferID = 0;
		protected uint depthRenderBufferID = 0;
		protected uint frameBufferID = 0;

		protected uint multiSampleColourRenderBufferID = 0;
		protected uint multiSampleDepthRenderBufferID = 0;
		protected uint multiSampleFrameBufferID = 0;

		protected IntPtr dibSectionDeviceContext = IntPtr.Zero;
		protected OpenGL gl;

		protected int multiSamples = 4;
		protected bool multiSampleExtensionsAvailable;
		protected bool fboInitialized = false;
	}
}
