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
using System.Linq;
using System.Text;



namespace Lemniscate
{
	using Lemniscate.DDL;

	//generates C++ interop header from a DDL Document
	class CPlusPlus_Generator : CLike_Generator
	{
		public string Generate(Document document)
		{
			//create a copy of the original document so that we can apply further language specific transformations
			document = document.Clone();

			//...like adding includes
			Add_Needed_Includes(document);

			Generate_Header(document);

			var include_guard = document.outputs.SingleOrDefault(o => o.cplusplus).file.Replace('/','_').Replace('.','_').ToUpper();

			generated_source.Append_Line("#ifndef {0}", include_guard);
			generated_source.Append_Line("#define {0}", include_guard);
			generated_source.Append_Empty_Line();

			Generate_Include_Statements(document);

			if (include_namespace)
			{
				generated_source.Append_Line("namespace Lemniscate");
				generated_source.Append_Line("{");
				generated_source.Increment_Indent_Level();
			}

			Generate_Enum_Definitions(document);

			Generate_Struct_Definitions(document);

			Generate_Interface_Definitions(document);

			Generate_Stubs(document);
			generated_source.Trim_Empty_Lines();
			

			if (include_namespace)
			{
				generated_source.Decrement_Indent_Level();
				generated_source.Append_Line("}");
			}

			Generate_Verbatim_Sections(document);

			generated_source.Trim_Empty_Lines();
			generated_source.Append_Empty_Lines(3);
			generated_source.Append_Line("#endif//{0}", include_guard);


			return generated_source.Get_Text_And_Flush();
		}

		private void Generate_Include_Statements(Document document)
		{
			foreach (var include in document.includes)
			{
				generated_source.Append_Line(include.is_standard_header ? "#include <{0}>" : "#include \"{0}\"", include.header);
			}

			if (document.includes.Count() > 0)
			{
				generated_source.Append_Empty_Lines(3);
			}
		}

		protected override void Generate_Enum_Definition(Enum ddl_enum)
		{
			if (!ddl_enum.Is_CPlusPlus) return;


			if (ddl_enum.Has_Comment)
			{
				Generate_Comment(ddl_enum.comment);
			}

			generated_source.Append_Line("enum class {0} : {1}", ddl_enum.name, ddl_enum.underlying_type.cpp_name);
			
			generated_source.Append_Line("{").Increment_Indent_Level();


			foreach ( var value in ddl_enum.values )
			{
				if (value.Has_Comment)
				{
					Generate_Comment(value.comment);
				}
				
				generated_source.Append( "{0}", value.name);
				if (value.Has_Value) generated_source.Append(" = {0}", value.value);
				generated_source.Append_Line(",");
				generated_source.Append_Empty_Line();
			}
			
			generated_source.Trim_Empty_Lines();
			generated_source.Decrement_Indent_Level().Append_Line("};");



			//serialization
			generated_source.Append_Empty_Line();

			generated_source.Append_Line("template <typename S>");
			generated_source.Append_Line("void Serialize(S& stream, const {0}& value)", ddl_enum.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("Serialize( stream, Enum_Cast(value) );");
			generated_source.Decrement_Indent_Level().Append_Line("}");

			generated_source.Append_Line("template <typename S>");
			generated_source.Append_Line("void Deserialize(S& stream, {0}& value)", ddl_enum.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("Deserialize( stream, Enum_Cast(value) );");
			generated_source.Decrement_Indent_Level().Append_Line("}");
		}

		protected override void Generate_Interface_Definition(Interface ddl_interface)
		{
			if (ddl_interface.Has_Comment)
			{
				Generate_Comment(ddl_interface.comment);
			}

			//public interface ITest
			//{
			generated_source.Append_Line("class {0}", ddl_interface.name);
			generated_source.Append_Line("{");
			generated_source.Append_Line("public:");
			generated_source.Increment_Indent_Level();



			foreach (var method in ddl_interface.methods)
			{
				if (method.Has_Comment)
				{
					Generate_Comment(method.comment);
				}

				var return_typename = method.Has_Return ? method.type.cpp_name : "void";
				generated_source.Append_Line( "virtual {0} {1}({2}) =0;", return_typename, method.name, Parameters_Types_And_Names_List(method) );
				generated_source.Append_Empty_Line();
			}

			generated_source.Trim_Empty_Lines();
			generated_source.Decrement_Indent_Level().Append_Line("};");
		}

		protected override void Generate_Struct_Definition(Struct ddl_struct, Document document)
		{
			if (!ddl_struct.Is_CPlusPlus) return;

			if (ddl_struct.Has_Comment)
			{
				Generate_Comment(ddl_struct.comment);
			}

			generated_source.Append_Line("struct {0}", ddl_struct.name);
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();


			foreach (var field in ddl_struct.fields)
			{
				if (field.Has_Comment)
				{
					Generate_Comment(field.comment);
				}

				generated_source.Append_Line("{0} {1};", field.type.cpp_name, field.name);
				generated_source.Append_Empty_Line();
			}
			generated_source.Trim_Empty_Lines();


			//verbatimref blocks
			foreach (var ddl_verbatim in ddl_struct.verbatims)
			{
				generated_source.Append_Empty_Lines(2);

				var verbatim = document.Get_Verbatim(ddl_verbatim.verbatim, "C++");
				if (verbatim != null)
				{
					generated_source.Append_Multiline(verbatim.text);
				}
			}
			generated_source.Trim_Empty_Lines();
			

			//end of struct
			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("};");

			//TODO: eventually re-enable this, if the memory-ready serializer needs it
			//generated_source.Append_Empty_Line();
			//generated_source.Append_Line("COMPILETIME_ASSERT(sizeof({0}) == {1}, \"unexpected size of generated underlying_type\");", ddl_struct.name, ddl_struct.size);
		}
		
		protected override void Generate_Struct_Serialization(Struct ddl_struct, Document document)
		{
			if (!ddl_struct.Is_CPlusPlus) return;

			//serialization
			generated_source.Append_Empty_Line();

			generated_source.Append_Line("template <typename S>");
			generated_source.Append_Line("void Serialize(S & stream, const {0}& value)", ddl_struct.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append("Serialize(stream");
			foreach (var ddl_field in ddl_struct.fields)
			{
				generated_source.Append(", value.{0}", ddl_field.name);
			}
			generated_source.Append_Line(");");
			generated_source.Decrement_Indent_Level().Append_Line("}");

			generated_source.Append_Line("template <typename S>");
			generated_source.Append_Line("void Deserialize(S & stream, {0}& value)", ddl_struct.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append("Deserialize(stream");
			foreach (var ddl_field in ddl_struct.fields)
			{
				generated_source.Append(", value.{0}", ddl_field.name);
			}
			generated_source.Append_Line(");");
			generated_source.Decrement_Indent_Level().Append_Line("}");
		}


		private void Generate_Stubs(Document document)
		{
			foreach ( var stub in document.stubs.Where(s => s.cplusplus) )
			{
				if (stub.type == "RPC_Client_Proxy")
				{
					Generate_RPC_Client_Proxy( document.Get_Interface(stub.interface_type), document );
				}
				else if (stub.type == "RPC_Server_Proxy")
				{
					Generate_RPC_Server_Proxy( document.Get_Interface(stub.interface_type) );
				}
				else throw new Exception( string.Format("unsupported stub: {0}", stub.interface_type) );

				generated_source.Append_Empty_Lines(2);
			}
		}

		private void Generate_RPC_Client_Proxy(Interface ddl_interface, Document document)
		{
			generated_source.Append_Line("template <typename T>");
			generated_source.Append_Line("class RPC_Client_Proxy_{0} : public RPC_Client_Proxy<T>, public {0}", ddl_interface.name);
			generated_source.Append_Line("{");
			generated_source.Append_Line("public:").Increment_Indent_Level();
			generated_source.Append_Line("RPC_Client_Proxy_{0}(T& stream) : RPC_Client_Proxy(stream){{ Set_prefix((unsigned char){1}); }}", ddl_interface.name, ddl_interface.index);
			generated_source.Append_Empty_Lines(1);

			generated_source.Decrement_Indent_Level().Append_Line("public:").Increment_Indent_Level();
			

			foreach (var method in ddl_interface.methods)
			{
				var return_typename = method.Has_Return ? method.type.cpp_name : "void";
				
				generated_source.Append_Line("{0} {1}({2})", return_typename, method.name, Parameters_Types_And_Names_List(method) );

				generated_source.Append_Line("{").Increment_Indent_Level();

				generated_source.Append_Line("Begin();");

				generated_source.Append_Line("const byte method_index = {0};", method.index);
				generated_source.Append_Line("Serialize(stream, method_index);");

				foreach ( var parameter in method.parameters.Where(p => !p.Is_Out) )
				{
					generated_source.Append_Line("Serialize(stream, {0});", parameter.name);
				}

				generated_source.Append_Line("Flip();");


				foreach ( var parameter in method.parameters.Where(p => p.Is_Out) )
				{
					generated_source.Append_Line("Deserialize(stream, {0});", parameter.name);
				}

				generated_source.Append_Line("End();");

				if (method.Has_Return)
				{
					generated_source.Append_Line("return Deserialize<{0}>(stream);", return_typename);
				}

				generated_source.Decrement_Indent_Level();
				generated_source.Append_Line("}");
				generated_source.Append_Empty_Line();
			}

			generated_source.Trim_Empty_Lines();
			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("};");
		}

		private void Generate_RPC_Server_Proxy(Interface ddl_interface)
		{
			Debug.Assert(ddl_interface.methods.Count() > 0); //interface should contain at least 1 method


			generated_source.Append_Line("template <typename  S>");
			generated_source.Append_Line("class RPC_Server_Proxy_{0} : public RPC_Server_Proxy<S, {0}>", ddl_interface.name);
			generated_source.Append_Line("{");
			generated_source.Append_Line("public:");
			generated_source.Increment_Indent_Level();
			
			generated_source.Append_Line("RPC_Server_Proxy_{0}(S & stream, {0}* obj) : RPC_Server_Proxy<S,{0}>(stream, obj){{ Set_prefix((unsigned char){1}); }}", ddl_interface.name, ddl_interface.index);
			generated_source.Append_Empty_Line();
			
			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("public:");
			generated_source.Increment_Indent_Level();
			

			generated_source.Append_Line("virtual void Receive_Call()");
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("const byte method_index = Deserialize<byte>(stream);");

			generated_source.Append_Line("switch(method_index)");
			generated_source.Append_Line("{").Increment_Indent_Level();
			

			foreach (var method in ddl_interface.methods)
			{
				generated_source.Append_Line("case {0}: //{1}", method.index, method.name);
				generated_source.Append_Line("{").Increment_Indent_Level();

				generated_source.Append_Line("Begin();");

				foreach (var parameter in method.parameters)
				{
					var parameter_typename = parameter.type.cpp_name;
					generated_source.Append_Line( "auto const {0} = Deserialize<{1}>(stream);", parameter.name, Deserialize_Call_From_Type(parameter_typename) );
				}

				generated_source.Append_Line("Flip();");

				if (method.Has_Return)
				{
					generated_source.Append("auto const result = ");
				}

				generated_source.Append_Line("this->obj->{0}({1});", method.name, Parameters_Names_List(method) );
				
				
				if (method.Has_Return)
				{
					generated_source.Append_Line("Serialize(stream, result);");
				}

				generated_source.Append_Line("End();");
				
				generated_source.Decrement_Indent_Level().Append_Line("}");
				generated_source.Append_Line("break;");
				generated_source.Append_Empty_Line();
			}

			
			generated_source.Append_Line("default:");
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("On_Invalid_Method_Index(method_index);");
			generated_source.Decrement_Indent_Level().Append_Line("}");
			generated_source.Append_Line("break;");
				

			generated_source.Decrement_Indent_Level().Append_Line("}");
			generated_source.Decrement_Indent_Level().Append_Line("}");
			
			//end of Receive_Call method


			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("};");
		}

		protected void Generate_Verbatim_Sections(Document document)
		{
			foreach ( var ddl_verbatim in document.verbatims.Where(v => v.Is_CPlusPlus && v.name == null) )
			{
				generated_source.Append_Empty_Line();
				generated_source.Append_Multiline(ddl_verbatim.text);
			}
		}

		string Deserialize_Call_From_Type(string type)
		{
			if (type == "const char*") return "std::string";
			else return type;
		}


		private void Add_Needed_Includes(Document document)
		{
			//TODO: shall we remove this function entirely?
		}

		//e.g. Test(string a, int b) -> a, b
		protected string Parameters_Names_List(Method method)
		{
			return string.Join(", ",	from parameter in method.parameters
										select (parameter.Is_Out ? "&" : "") + parameter.name);
		}
		
		//e.g. Test(string a, int b) -> string, int
		protected string Parameters_Types_List(Method method)
		{
			return string.Join(", ",	from parameter in method.parameters
										select (parameter.Is_Out ? "out " : "") + parameter.type.cpp_name);
		}

		//e.g. Test(string a, int b) -> string, int
		protected string Parameters_Types_And_Names_List(Method method)
		{
			return string.Join( ", ",	from parameter in method.parameters
										select string.Format(	"{0}{1}{2} {3}", 
																parameter.type.cpp_pass_by_ref	? "const " : "",
																parameter.type.cpp_name,
																(parameter.Is_Out || parameter.type.cpp_pass_by_ref) ? "&" : "",
																parameter.name) );
		}

		#region Fields
		bool include_namespace = false;
		#endregion
	}
}