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



namespace Lemniscate
{
	using Lemniscate.DDL;

	//common functionalities for both C++ and C# generators
	abstract class CLike_Generator
	{
		protected void Generate_Header(Document document)
		{
			generated_source.Append_Line("/////////////////////////////////////////////////");
			generated_source.Append_Line("// THIS FILE IS AUTOGENERATED, DON'T CHANGE IT //");
			generated_source.Append_Line("/////////////////////////////////////////////////");

			if ( !string.IsNullOrEmpty(document.comment) )
			{
				generated_source.Append_Empty_Line();
			
				Generate_Comment(document.comment);

				generated_source.Append_Empty_Line();
			}
		}

		protected void Generate_Enum_Definitions(Document document)
		{
			foreach (var ddl_enum in document.enums)
			{
				Generate_Enum_Definition(ddl_enum);

				generated_source.Append_Empty_Lines(2);
			}
		}

		protected void Generate_Interface_Definitions(Document document)
		{
			foreach (var ddl_interface in document.interfaces)
			{
				Generate_Interface_Definition(ddl_interface);

				generated_source.Append_Empty_Lines(2);
			}
		}

		protected void Generate_Struct_Definitions(Document document)
		{
			foreach (var ddl_struct in document.structs)
			{
				Generate_Struct_Definition(ddl_struct, document);
				Generate_Struct_Serialization(ddl_struct, document);

				generated_source.Append_Empty_Lines(2);
			}
		}


		//TODO: maybe factorize many of these... for enums the only difference is the "enum class" vs "enum" part, and the ; at the end
		protected abstract void Generate_Interface_Definition(Interface ddl_interface);

		protected abstract void Generate_Enum_Definition(Enum ddl_enum);

		protected abstract void Generate_Struct_Definition(Struct ddl_struct, Document document);
		protected abstract void Generate_Struct_Serialization(Struct ddl_struct, Document document);


		protected void Generate_Comment(string comment)
		{
			//support multiline comments
			foreach ( var comment_line in comment.Split(new char[]{'\n'}) )
			{
				generated_source.Append_Line("//{0}", comment_line);
			}
		}

		protected bool Has_Stubs(Document document)
		{
			foreach (var stub in document.stubs)
			{
				if (stub.type == "RPC_Client_Proxy")	return true;
				if (stub.type == "RPC_Server_Proxy")	return true;
				if (stub.type == "RPC_Server_Receiver")	return true;
			}

			return false;
		}


		#region Fields
		protected readonly Text_Builder generated_source = new Text_Builder();
		#endregion
	}
}