// The MIT License (MIT)
// 
// Copyright (c) 2014 Maurizio de Pascale
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;



namespace Lemniscate
{
	using Lemniscate.DDL;


	//monitors a folder and generates new interop sources as soon as an input ddl changes
	public class DDL_Compiler
	{
		public DDL_Compiler()
		{
			Mark_Generated_Files_As_Readonly = true;
		}

		public void Process_Immediately(string path, string output_folder)
		{
			this.output_folder = output_folder;

			On_File_Changed(path);
		}

		public void Process_And_Monitor_Directory(string directory, string output_folder)
		{
			Logging.Verbose("Watching {0}", directory);
			
			this.output_folder = output_folder;

			watcher = new FileSystemWatcher();
			watcher.Path = Path.GetFullPath(directory);
			watcher.Filter = "*.xml";
			watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
 
			watcher.Changed += On_File_Changed;
			watcher.Created += On_File_Changed;
			watcher.Renamed += On_File_Changed;
 
			watcher.EnableRaisingEvents = true;

			foreach ( var filename in Directory.GetFiles(directory, "*.xml") )
			{
				On_File_Changed(filename);
 			}
		}

		public bool Mark_Generated_Files_As_Readonly
		{
			get; set;
		}


		#region Private
		private void On_File_Changed(object sender, object args)
		{
			//TODO: we should track import dependencies, or alternatively just process the whole folder each time a single file changes

			if (args is RenamedEventArgs)
			{
				var arg = args as RenamedEventArgs;
				
				On_File_Changed(arg.FullPath);
			}
			else if(args is FileSystemEventArgs)
			{
				var arg = args as FileSystemEventArgs;
 
				On_File_Changed(arg.FullPath);
			}
			else Debug.Fail("unexpected");
		}
		private void On_File_Changed(string filename)
		{
			lock (this)
			{
				Logging.Verbose("{0} has changed", filename);
 
			try_again:
				try
				{
					Func<string, Stream> load_delegate = (string module) =>
					{
						var module_filename = Path.Combine( Path.GetDirectoryName(filename), module );
						var buffer = File.ReadAllBytes(module_filename);
						return new MemoryStream(buffer);
					};

					Document document = XML_Parser.Load( Path.GetFileName(filename), load_delegate );
					DDL_Type_Analyzer.Compute_Sizes_And_Alignments(document);

					Process_CPP(document, filename);
					Process_CS(document, filename);
				}
				catch (FormatException exception)
				{
					Logging.Error("exception while processing {0}: {1}\n{2}", Path.GetFileName(filename), exception.Message, exception.StackTrace);
				}
				catch (IOException exception)
				{
					Logging.Warning("IO error while parsing {0}: {1}", filename, exception.Message);
					goto try_again;
				}
			}
		}

		private void Process_CPP(Document document, string filename)
		{
			var cpp_output = document.outputs.SingleOrDefault(o => o.cplusplus);
			if (cpp_output == null) return;

			var generated_cpp = new CPlusPlus_Generator().Generate(document);
			//GetFullPath will convert forward slashes to back slashes
			var filename_cpp = Path.GetFullPath( Path.Combine(output_folder, cpp_output.file) );

			if ( File.Exists(filename_cpp) )
			{
				if (File.ReadAllText(filename_cpp) == generated_cpp)
				{
					Logging.Verbose("{0} is up to date", filename_cpp);
					return;
				}

				new FileInfo(filename_cpp).IsReadOnly = false;
			}
			File.WriteAllText(filename_cpp, generated_cpp);
			if (Mark_Generated_Files_As_Readonly) new FileInfo(filename_cpp).IsReadOnly = true;

			Logging.Info("{0} was regenerated", filename_cpp);
		}
		private void Process_CS(Document document, string filename)
		{
			var cs_output = document.outputs.SingleOrDefault(o => o.csharp);
			if (cs_output == null) return;

			var generated_cs = new CSharp_Generator().Generate(document);
			//GetFullPath will convert forward slashes to back slashes
			var filename_cs = Path.GetFullPath( Path.Combine(output_folder, cs_output.file) );
 
			if ( File.Exists(filename_cs) )
			{
				if (File.ReadAllText(filename_cs) == generated_cs)
				{
					Logging.Verbose("{0} is up to date", filename_cs);
					return;
				}

				new FileInfo(filename_cs).IsReadOnly = false;
			}
			File.WriteAllText(filename_cs, generated_cs);
			if (Mark_Generated_Files_As_Readonly) new FileInfo(filename_cs).IsReadOnly = true;

			Logging.Info("{0} was regenerated", filename_cs);
		}

		FileSystemWatcher watcher;
		string output_folder;
		#endregion
	}
}