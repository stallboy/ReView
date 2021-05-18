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
using System.Text;



namespace Lemniscate
{
	//a wrapper around string builder that supports indentation
	public class Text_Builder
	{
		public Text_Builder Append(string text, params object[] args)
		{
			if (is_on_new_line)
			{
				is_on_new_line = false;
				builder.Append(line_header);
			}

			if (args.Length == 0)
			{
				builder.Append(text);
			}
			else
			{
				builder.AppendFormat(text, args);
			}

			return this;
		}
		public Text_Builder Append_Line(string text, params object[] args)
		{
			if (is_on_new_line)
			{
				builder.Append(line_header);
				is_on_new_line = false; //redundant
			}

			if (args.Length == 0)
			{
				builder.AppendLine(text);
			}
			else
			{
				builder.AppendFormat(text, args);
				builder.AppendLine();
			}

			is_on_new_line = true;

			return this;
		}

		public Text_Builder Append_Multiline(string multiline_text, params object[] args)
		{
			foreach ( var line in multiline_text.Replace("\r","").Split('\n') )
			{
				Append_Line(line, args);
			}

			return this;
		}

		public Text_Builder Append_Empty_Line()
		{
			Append_Empty_Lines(1);

			return this;
		}
		public Text_Builder Append_Empty_Lines(int count)
		{
			while (count-- != 0)
			{
				builder.AppendLine();
			}
			is_on_new_line = true;

			return this;
		}

		public Text_Builder Increment_Indent_Level()
		{
			line_header = line_header + "\t";
			return this;
		}
		public Text_Builder Decrement_Indent_Level()
		{
			Debug.Assert(line_header.Length != 0);
			line_header = line_header.Substring(1);
			return this;
		}

		public Text_Builder Trim_Empty_Lines()
		{
			if (is_on_new_line) //nothing to trim if we're not an a new line
			{
				while( Char.IsWhiteSpace(builder[builder.Length-1]) )
				{
					builder.Length = builder.Length-1;
				}
				builder.AppendLine();
			}

			return this;
		}

		public string Get_Text_And_Flush()
		{
			var text = builder.ToString();
			builder.Clear();
			return text;
		}

		public override string ToString()
		{
			return builder.ToString();
		}


		#region Fields
		private bool is_on_new_line = true;
		private string line_header = "";
		private readonly StringBuilder builder = new StringBuilder();
		#endregion
	}
}