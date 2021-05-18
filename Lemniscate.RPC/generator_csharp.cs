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

	//generates C# interop sources from a DDL Document
	class CSharp_Generator : CLike_Generator
	{
		public string Generate(Document document)
		{
			document = document.Clone();
			Add_Needed_Assemblies(document);

			Generate_Header(document);

			Generate_Using_Definitions(document);

			if (document.cs_namespace == null) throw new FormatException("Missing Namespace Attribute: " + document.name);
			generated_source.Append_Line("namespace {0}", document.cs_namespace);
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();

			Generate_Enum_Definitions(document);

			Generate_Struct_Definitions(document);

			Generate_Interface_Definitions(document);

			Generate_Stubs(document);

			generated_source.Trim_Empty_Lines();

			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("}");

			generated_source.Append_Empty_Line();
			Generate_Verbatim_Sections(document);
			generated_source.Trim_Empty_Lines();


			return generated_source.Get_Text_And_Flush();
		}

		private void Generate_Using_Definitions(Document document)
		{
			foreach (var ddl_using in document.usings)
			{
				generated_source.Append_Line("using {0};", ddl_using.assembly);
			}

			if (document.usings.Count() > 0)
			{
				generated_source.Append_Empty_Lines(3);
			}
		}

		protected override void Generate_Enum_Definition(Enum ddl_enum)
		{
			if (ddl_enum.Has_Comment)
			{
				Generate_Comment(ddl_enum.comment);
			}

			generated_source.Append_Line("public enum {0} : {1}", ddl_enum.name, ddl_enum.underlying_type.cs_name);
			
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();


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
			
			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("}");




			///serialization
			generated_source.Append_Empty_Lines(2);
			generated_source.Append_Line("public static class {0}_Serialization_Extensions", ddl_enum.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			
			generated_source.Append_Line("public static void Serialize(this Linear_Serializer serializer, {0} value)", ddl_enum.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line( "serializer.Serialize( ({0}) value );", ddl_enum.underlying_type.cs_name);
			generated_source.Decrement_Indent_Level().Append_Line("}");

			generated_source.Append_Line("public static void Deserialize(this Linear_Serializer serializer, out {0} value)", ddl_enum.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("{0} return_value;", ddl_enum.underlying_type.cs_name);
			generated_source.Append_Line("serializer.Deserialize(out return_value);" );
			generated_source.Append_Line("value = ({0}) return_value;", ddl_enum.cs_name);
			generated_source.Decrement_Indent_Level().Append_Line("}");


			//end of serialization extensions
			generated_source.Decrement_Indent_Level().Append_Line("}");
		}

		protected override void Generate_Interface_Definition(Interface ddl_interface)
		{
			if (ddl_interface.Has_Comment)
			{
				Generate_Comment(ddl_interface.comment);
			}

			
			generated_source.Append_Line("public interface {0}", ddl_interface.name);
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();



			foreach (var method in ddl_interface.methods)
			{
				if (method.Has_Comment)
				{
					Generate_Comment(method.comment);
				}

				var return_typename = method.Has_Return ? method.type.cs_name : "void";
				generated_source.Append_Line("{0} {1}({2});", return_typename, method.name, Parameters_Types_And_Names_List(method) );
				generated_source.Append_Empty_Line();
			}

			generated_source.Trim_Empty_Lines();
			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("}");
		}

		protected override void Generate_Struct_Definition(Struct ddl_struct, Document document)
		{
			if (!ddl_struct.Is_CSharp) return;

			if (ddl_struct.Has_Comment)
			{
				Generate_Comment(ddl_struct.comment);
			}

			generated_source.Append_Line("[Serializable]");
			generated_source.Append_Line("public class {0}", ddl_struct.name);
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();



			foreach (var field in ddl_struct.fields)
			{
				if (field.Has_Comment)
				{
					Generate_Comment(field.comment);
				}

				generated_source.Append_Line("public {0} {1};", field.type.cs_name, field.name);
				generated_source.Append_Empty_Line();
			}

			generated_source.Trim_Empty_Lines();


			//verbatimref block
			foreach (var ddl_verbatim in ddl_struct.verbatims)
			{
				generated_source.Append_Empty_Lines(2);
			
				var verbatim = document.Get_Verbatim(ddl_verbatim.verbatim, "C#");
				if (verbatim != null)
				{
					generated_source.Append_Multiline(verbatim.text);
				}
			}
			generated_source.Trim_Empty_Lines();
			

			//end of struct
			generated_source.Decrement_Indent_Level().Append_Line("}");
		}
		
		protected override void Generate_Struct_Serialization(Struct ddl_struct, Document document)
		{
			if (!ddl_struct.Is_CSharp) return;


			// serialization
			generated_source.Append_Empty_Lines(2);
			generated_source.Append_Line("public static class {0}_Serialization_Extensions", ddl_struct.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			

			///linear serialization
			
			//serialize
			generated_source.Append_Line("public static void Serialize(this Linear_Serializer serializer, {0} value)", ddl_struct.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			foreach (var ddl_field in ddl_struct.fields)
			{
				generated_source.Append_Line("serializer.Serialize(value.{0});", ddl_field.name);
			}
			generated_source.Decrement_Indent_Level().Append_Line("}");
			
			//deserialize
			generated_source.Append_Line("public static void Deserialize(this Linear_Serializer serializer, out {0} value)", ddl_struct.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("value = new {0}();", ddl_struct.name);
			foreach (var ddl_field in ddl_struct.fields)
			{
				generated_source.Append_Line("serializer.Deserialize(out value.{0});", ddl_field.name);
			}
			generated_source.Decrement_Indent_Level().Append_Line("}");
			

			///end of serialization extensions
			generated_source.Decrement_Indent_Level().Append_Line("}");
		}


		private void Generate_Stubs(Document document)
		{
			foreach ( var stub in document.stubs.Where(s => s.csharp) )
			{
				if (stub.type == "RPC_Client_Proxy")
				{
					Generate_RPC_Client_Proxy( document.Get_Interface(stub.interface_type) );
				}
				else if (stub.type == "RPC_Server_Proxy")
				{
					Generate_RPC_Server_Proxy( document.Get_Interface(stub.interface_type), use_delegate : false );
				}
				else if (stub.type == "RPC_Server_Receiver")
				{
					Generate_RPC_Server_Proxy( document.Get_Interface(stub.interface_type), use_delegate : true );
				}
				else throw new Exception( string.Format("unsupported stub: {0}", stub.interface_type) );

				generated_source.Append_Empty_Lines(2);
			}
		}

		private void Generate_RPC_Client_Proxy(Interface ddl_interface)
		{
			generated_source.Append_Line("public class RPC_Client_Proxy_{0} : RPC_Client_Proxy, {0}", ddl_interface.name);
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("public RPC_Client_Proxy_{0}(Linear_Serializer serializer) : base(serializer){{ Channel_ID = {1}; }}", ddl_interface.name, ddl_interface.index);
			generated_source.Append_Empty_Lines(1);

			foreach (var method in ddl_interface.methods)
			{
				var return_typename = method.Has_Return ? method.type.cs_name : "void";
				
				generated_source.Append_Line( "public {0} {1}({2})", return_typename, method.name, Parameters_Types_And_Names_List(method) );

				generated_source.Append_Line("{").Increment_Indent_Level();

				generated_source.Append_Line("Begin();");

				generated_source.Append_Line("const byte method_index = {0};", method.index);
				generated_source.Append_Line("serializer.Serialize(method_index);");


				foreach ( var parameter in method.parameters.Where(p => !p.Is_Out) )
				{
					if (parameter.type.cs_converter != null)
					{
						generated_source.Append_Line("serializer.Serialize( ({0}) {1} );", parameter.type.cs_converter, parameter.name);
					}
					else
					{
						generated_source.Append_Line("serializer.Serialize({0});", parameter.name);
					}
				}

				generated_source.Append_Line("Flip(); //switch from sending to receiving");


				foreach ( var parameter in method.parameters.Where(p => p.Is_Out) )
				{
					if (parameter.type.cs_converter != null)
					{
						generated_source.Append_Line("{0} {1}_intermediate;", parameter.type.cs_converter, parameter.name);
						generated_source.Append_Line("serializer.Deserialize(out {0}_intermediate);", parameter.name);
						generated_source.Append_Line("{0} = {0}_intermediate;", parameter.name);
					}
					else
					{
						generated_source.Append_Line( "serializer.Deserialize(out {0});", parameter.name );
					}
				}

				if (method.Has_Return)
				{
					generated_source.Append_Line("{0} return_value;", method.type.cs_converter != null ? method.type.cs_converter : method.type.cs_name);
					generated_source.Append_Line("serializer.Deserialize(out return_value);");
				}

				generated_source.Append_Line("End();");

				if (method.Has_Return)
				{
					generated_source.Append_Line("return return_value;");
				}


				generated_source.Decrement_Indent_Level().Append_Line("}");
				generated_source.Append_Empty_Line();
			}
			generated_source.Trim_Empty_Lines();

			generated_source.Decrement_Indent_Level().Append_Line("}");
		}

		private void Generate_RPC_Server_Proxy(Interface ddl_interface, bool use_delegate)
		{
			if (ddl_interface.methods.Count() == 0) throw new FormatException("interface should contain at least 1 method: " + ddl_interface);


			if (use_delegate)
			{
				generated_source.Append_Line("public class RPC_Server_Receiver_{0} : RPC_Server_Receiver", ddl_interface.name);
			}
			else
			{
				generated_source.Append_Line("public class RPC_Server_Proxy_{0} : RPC_Server_Proxy<{0}>", ddl_interface.name);
			}
			generated_source.Append_Line("{");
			generated_source.Increment_Indent_Level();
			
			if (use_delegate)
			{
				generated_source.Append_Line("public RPC_Server_Receiver_{0}(Linear_Serializer serializer) : base(serializer){{ Channel_ID = {1}; }}", ddl_interface.name, ddl_interface.index);
			}
			else
			{
				generated_source.Append_Line("public RPC_Server_Proxy_{0}(Linear_Serializer serializer, {0} obj) : base(serializer, obj){{ Channel_ID = {1}; }}", ddl_interface.name, ddl_interface.index);
			}
			
			generated_source.Append_Empty_Lines(1);
			
			generated_source.Append_Line("public override void Receive_Call()");
			generated_source.Append_Line("{").Increment_Indent_Level();
			generated_source.Append_Line("byte method_index;");
			generated_source.Append_Line("serializer.Deserialize(out method_index);");


			generated_source.Append_Line("switch(method_index)");
			generated_source.Append_Line("{").Increment_Indent_Level();
			


			foreach (var method in ddl_interface.methods)
			{
				generated_source.Append_Line("case {0}: //{1}", method.index, method.name);
				generated_source.Append_Line("{").Increment_Indent_Level();

				generated_source.Append_Line("Begin();");


				foreach (var parameter in method.parameters)
				{
					generated_source.Append_Line("{0} {1};", parameter.type.cs_name, parameter.name);
					
					if (!parameter.Is_Out)
					{
						generated_source.Append_Line("serializer.Deserialize(out {0});", parameter.name);
					}
				}


				generated_source.Append_Line("Flip(); //switch from receiving to sending");


				if (method.Has_Return)
				{
					generated_source.Append("{0} return_value = ", method.type.cs_name);
				}

				if (use_delegate)
				{
					generated_source.Append_Line( "{0}({1});", method.name, Parameters_Names_List(method) );
				}
				else
				{
					generated_source.Append_Line("this.obj.{0}({1});", method.name, Parameters_Names_List(method) );
				}
				

				foreach (var parameter in method.parameters)
				{
					if (parameter.Is_Out)
					{
						generated_source.Append_Line("serializer.Serialize({0});", parameter.name);
					}
				}


				if (method.Has_Return)
				{
					generated_source.Append_Line("serializer.Serialize(return_value);");
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


			//events
			if (use_delegate)
			{
				foreach (var method in ddl_interface.methods)
				{
					var return_typename = method.Has_Return ? method.type.cs_name : "void"; 

					generated_source.Append_Empty_Lines(1);
					generated_source.Append_Line( "public delegate {0} {1}_Delegate({2});", return_typename, method.name, Parameters_Types_And_Names_List(method) );
					generated_source.Append_Line("public {0}_Delegate {0};", method.name);
				}
				generated_source.Append_Empty_Line();
			}
			generated_source.Trim_Empty_Lines();


			generated_source.Decrement_Indent_Level();
			generated_source.Append_Line("}");
		}

		protected void Generate_Verbatim_Sections(Document document)
		{
			foreach ( var ddl_verbatim in document.verbatims.Where(v => v.Is_CSharp && v.name == null) )
			{
				generated_source.Append_Multiline(ddl_verbatim.text);
			}
		}

		private void Add_Needed_Assemblies(Document document)
		{
			document.usings.Add( new Using() {assembly = "System"} );

			//TODO: this should be based on what's really needeed
			//if ( Has_Stubs(document) )
			{
				document.usings.Add( new Using() {assembly = "System.IO"} );
			}
		}

		//e.g. Test(string a, int b, out int c) -> a, b, out c
		protected string Parameters_Names_List(Method method)
		{
			return string.Join(", ",	from parameter in method.parameters
										select (parameter.Is_Out ? "out " : "") + parameter.name);
		}
		
		//e.g. Test(string a, int b) -> string, int
		protected string Parameters_Types_List(Method method)
		{
			return string.Join(", ",	from parameter in method.parameters
										select (parameter.Is_Out ? "out " : "") + parameter.type.cs_name);
		}

		//e.g. Test(string a, int b) -> string a, int b
		protected string Parameters_Types_And_Names_List(Method method)
		{
			return string.Join( ", ",	from parameter in method.parameters
										select string.Format("{0}{1} {2}", parameter.Is_Out ? "out " : "", parameter.type.cs_name, parameter.name) );
		}
	}
}