using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	[Serializable]
	public class Color32
	{
		// 0, 0, 0, 0
		public Color32()
		{
			components = new byte[] { 0, 0, 0, 0 };
		}

		// From r, g, b, a (byte 0..255)
		public Color32(byte inR, byte inG, byte inB, byte inA)
		{
			components = new byte[] { inR, inG, inB, inA};
		}

		// From r, g, b, a (double 0..1)
		public Color32(double inR, double inG, double inB, double inA)
		{
			components = new byte[] { (byte)(SMath.Clamp(inR, 0, 1) * 255), (byte)(SMath.Clamp(inG, 0, 1) * 255), (byte)(SMath.Clamp(inB, 0, 1) * 255), (byte)(SMath.Clamp(inA, 0, 1) * 255) };
		}

		// From hue, sat, val, alpha (double 0..1)
		public static Color32 FromHSVA(double inHue, double inSaturation, double inValue, double inAlpha)
		{
			int h = (int)(inHue * 6);
			double f = inHue * 6 - h;
			double pD = inValue * (1 - inSaturation);
			double qD = inValue * (1 - f * inSaturation);
			double tD = inValue * (1 - (1 - f) * inSaturation);

			byte alpha = (byte)(SMath.Clamp(inAlpha, 0, 1) * 255);
			byte p = (byte)(SMath.Clamp(pD, 0, 1) * 255);
			byte q = (byte)(SMath.Clamp(qD, 0, 1) * 255);
			byte t = (byte)(SMath.Clamp(tD, 0, 1) * 255);
			byte value = (byte)(SMath.Clamp(inValue, 0, 1) * 255);

			switch (h % 6)
			{
				case 0: return new Color32(value, t, p, alpha);
				case 1: return new Color32(q, value, p, alpha);
				case 2: return new Color32(p, value, t, alpha);
				case 3: return new Color32(p, q, value, alpha);
				case 4: return new Color32(t, p, value, alpha);
				case 5: return new Color32(value, p, q, alpha);
			}

			return new Color32(0, 0, 0, 0);
		}

		public static Color32 Random()
		{
			return new Color32(SRandom.Double(1), SRandom.Double(1), SRandom.Double(1), 1);
		}

		public double Brightness
		{
			get
			{
				return SMath.Clamp(R_Double * 0.3 + G_Double * 0.55 + B_Double * 0.15, 0.0, 1.0);
			}
		}

		// Red as double (0..1)
		public double R_Double
		{
			get
			{
				return ((double)components[0]) / 255;
			}
			set
			{
				components[0] = (byte)(SMath.Clamp(value, 0, 1) * 255);
			}
		}

		// Green as double (0..1)
		public double G_Double
		{
			get
			{
				return ((double)components[1]) / 255;
			}
			set
			{
				components[1] = (byte)(SMath.Clamp(value, 0, 1) * 255);
			}
		}

		// Blue as double (0..1)
		public double B_Double
		{
			get
			{
				return ((double)components[2]) / 255;
			}
			set
			{
				components[2] = (byte)(SMath.Clamp(value, 0, 1) * 255);
			}
		}

		// Alpha as double (0..1)
		public double A_Double
		{
			get
			{
				return ((double)components[3]) / 255;
			}
			set
			{
				components[3] = (byte)(SMath.Clamp(value, 0, 1) * 255);
			}
		}

		// Red as byte (0..255)
		public byte R
		{
			get
			{
				return components[0];
			}
			set
			{
				components[0] = value;
			}
		}

		// Green as byte (0..255)
		public byte G
		{
			get
			{
				return components[1];
			}
			set
			{
				components[1] = value;
			}
		}

		// Blue as byte (0..255)
		public byte B
		{
			get
			{
				return components[2];
			}
			set
			{
				components[2] = value;
			}
		}

		// Alpha as byte (0..255)
		public byte A
		{
			get
			{
				return components[3];
			}
			set
			{
				components[3] = value;
			}
		}

		// Get 'index'th component
		public byte this[int index]
		{
			get
			{
				return components[index];
			}
			set
			{
				components[index] = value;
			}
		}

		// Check equality
		public static bool operator ==(Color32 a, Color32 b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			for (int i = 0; i < 4; i++)
			{
				if (a.components[i] != b.components[i])
					return false;
			}
			return true;
		}

		// Check inequality
		public static bool operator !=(Color32 a, Color32 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			Color32 other = obj as Color32;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", R, G, B, A);
		}

		public byte[] components;
	}
}
