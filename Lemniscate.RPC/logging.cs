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
using System.Text;



namespace Lemniscate
{
	public static class Logging
	{
		public static void Info(string format, params object[] args)
		{
			Log("INFO", format, args);
		}
		public static void Verbose(string format, params object[] args)
		{
			Log("VERBOSE", format, args);
		}
		public static void Error(string format, params object[] args)
		{
			Log("ERROR", format, args);
		}
		public static void Warning(string format, params object[] args)
		{
			Log("WARNING", format, args);
		}
		
		private static void Log(string type, string format, params object[] args)
		{
			var time = DateTime.Now;
			var text = string.Format(format, args);
			var message = string.Format("{0} [{1}]: {2}\n", time, type, text);
			logger.Write(message);
		}


		static Logging()
		{
			var process_filename = Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", ""); //strip the vshost when running from Visual Studio
			var process_name = Path.GetFileName(process_filename);
			var appdomain_name = AppDomain.CurrentDomain.FriendlyName.Replace(".vshost", ""); //strip the vshost when running from Visual Studio
			
			var log_filename =	(process_name == appdomain_name)
							?	process_filename + ".log"
							:	process_filename + " [" + appdomain_name.Replace("Sandbox ","").Replace(".dll", "") + "].log";
			
			logger = new Logger(log_filename);
		}

		static Logger logger;
	}

	//NOTE: for low-frequency logging perhaps File.AppendAllText would be good enough
	internal class Logger : IDisposable
	{
		public Logger(string filename)
		{
			//sharing settings to play nicely with tools reading the log while we write, and other app domains from the same app
			file = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
		}

		public void Dispose()
		{
			file.Close();
			file = null;
		}

		public void Write(string text)
		{
			var message = Encoding.UTF8.GetBytes(text);
			file.Write(message, 0, message.Length);
			file.Flush();
		}

		FileStream file;
	}
}