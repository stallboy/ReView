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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;



//Data Definition Language
namespace Lemniscate {
namespace DDL
{
	internal static class Constant
	{
		public const uint Invalid_Size = 0;			//min size for a type is 1
		public const uint Invalid_Alignment = 0;	//min alignment for a type is 1
	}

	internal static class Utilities
	{
		public static uint Parse_Size_Or_Alignment(string value)
		{
			return string.IsNullOrEmpty(value) ? Constant.Invalid_Size : uint.Parse(value);
		}

		//e.g. Slice<Byte>
		public static bool Is_Generic_Type(string name)
		{
			return name.Contains("<");
		}
		
		//e.g. Slice<Byte> -> Byte
		public static string Generic_Type_Arguments(string name)
		{
			if ( !Is_Generic_Type(name) ) return null;
			int idx = name.IndexOf("<");
			return name.Substring(idx+1, name.Length-2-idx);
		}

		//e.g. Slice(Byte) -> Slice<Byte>
		public static string Unescape_Typename(string typename)
		{
			return typename.Replace('(','<').Replace(')','>');
		}
		
		//e.g. Slice<Byte> -> Slice<>
		public static string Generic_Typename(string typename)
		{
			return typename.Substring(0, typename.IndexOf('<')) + "<>";
		}

		public static Builtin_Type Instantiate_Specialization(Type generic_type, Type argument_type)
		{
			Debug.Assert( Is_Generic_Type(generic_type.name) );
			
			return new Builtin_Type()
			{
				name = generic_type.name.TrimEnd('>') + argument_type.name + '>',
				cpp_name = generic_type.cpp_name.TrimEnd('>') + argument_type.cpp_name + '>',
				
				//support instantiating a generic as a T[]
				cs_name = generic_type.cs_name == "[]"
						?	argument_type.cs_name + "[]"
						:	generic_type.cs_name.TrimEnd('>') + argument_type.cs_name + '>',
				
				size = generic_type.size,
				alignment = generic_type.alignment,
				cpp_pass_by_ref = generic_type.cpp_pass_by_ref
			};
		}
	}

	[DebuggerDisplay("{name}")]
	internal class Document
	{
		#region Fields
		//the filename of the xml this document was parsed from
		public string name;

		//a multi-line string that will be included as a comment at the beginning of generated files
		public string comment;

		//the outermost namespace in C#
		public string cs_namespace;

		public readonly List<Import> imports = new List<Import>();
		public readonly List<Builtin_Type> builtins = new List<Builtin_Type>();
		public readonly List<Output> outputs = new List<Output>();
		public readonly List<Using> usings = new List<Using>();
		public readonly List<Include> includes = new List<Include>();
		public readonly List<Interface> interfaces = new List<Interface>();
		public readonly List<Stub> stubs = new List<Stub>();
		public readonly List<Enum> enums = new List<Enum>();
		public readonly List<Struct> structs = new List<Struct>();
		public readonly List<Verbatim> verbatims = new List<Verbatim>();
		#endregion


		#region Queries
		public Interface Get_Interface(string name)
		{
			return interfaces.Single(interface_ => interface_.name == name);
		}

		public Type Get_Type(string typename, bool instantiate_generics = true)
		{
			if ( Utilities.Is_Generic_Type(typename) )
			{
				//find the exact generic specialization e.g. Slice<byte>
				var matching_type = builtins.SingleOrDefault(type => type.name == typename);
				if (matching_type != null) return matching_type;


				//couldn't find the specialized version, let's see if we can find the non-specialized version e.g. Slice<>
				string generic_typename = Utilities.Generic_Typename(typename);
				Type generic_type = builtins.SingleOrDefault(type => type.name == generic_typename);
				
				//if we couldn't find it in this module, search in imported modules
				if (generic_type == null)
				{
					foreach(var import in imports)
					{
						generic_type = import.module.Get_Type(generic_typename, instantiate_generics : false);
						if (generic_type != null) break;
					}
				}

				//we found the non specialized generic, let's instantiate the needed specialization
				if (generic_type != null && instantiate_generics)
				{
					var argument_type = Get_Type( Utilities.Generic_Type_Arguments(typename) );
					Debug.Assert(argument_type != null);
					var new_type = Utilities.Instantiate_Specialization(generic_type, argument_type);
					builtins.Add(new_type);

					return new_type;
				}
			}
			else //non-generic type. simply search recursively
			{
				var matching_type = 	(Type) builtins.SingleOrDefault(type => type.name == typename)
									??	(Type) enums.SingleOrDefault(type => type.name == typename)
									??	(Type) structs.SingleOrDefault(type => type.name == typename);

				if (matching_type != null) return matching_type;

				//couldn't find it... let's check in the imported modules
				foreach(var import in imports)
				{
					var imported_type = import.module.Get_Type(typename);
					if (imported_type != null) return imported_type;
				}
			}

			//couldn't find. let's hope caller will assert if unexpected
			return null;
		}

		public Verbatim Get_Verbatim(string name, string language)
		{
			return verbatims.SingleOrDefault( item => item.name == name && item.language.Contains(language) );
		}
		#endregion


		//creates a deep copy that can be freely and safely modified
		public Document Clone()
		{
			return this.MemberwiseClone() as Document;
		}
	}

	[DebuggerDisplay("{module_name}")]
	internal class Import
	{
		public string module_name;
		public Document module;
	}

	[DebuggerDisplay("{name} ({cs_name} in C#, {cpp_name} in C++)")]
	internal class Type :  IEquatable<Type>
	{
		public string name;
		public string cs_name;
		public string cs_converter;
		public string cpp_name;
		public uint size;
		public uint alignment;
		public bool cpp_pass_by_ref;

		public bool Equals(Type rhs)
		{
			return rhs.name == name;
		}
	}

	//represents a builtin type that is not defined via DDL, but directly in one (or both) target languages
	[DebuggerDisplay("{name} ({cs_name} in C#, {cpp_name} in C++)")]
	internal class Builtin_Type : Type, IEquatable<Builtin_Type>
	{
		public bool Equals(Builtin_Type rhs)
		{
			//TODO: find out what's wrong with this
			//return ((Type) this) == ((Type) rhs);
			return rhs.name == name;
		}
	}

	[DebuggerDisplay("{file}")]
	internal class Output
	{
		public string file;
		public bool cplusplus; //TODO: these two might just come from the extension
		public bool csharp;
	}

	[DebuggerDisplay("{name}")]
	internal class Verbatim
	{
		public string name;
		public string text;
		public string language;
		
		public bool Is_CPlusPlus
		{
			get
			{
				return language.Contains("C++");
			}
		}
		public bool Is_CSharp
		{
			get
			{
				return language.Contains("C#");
			}
		}
	}

	[DebuggerDisplay("{assembly}")]
	internal class Using
	{
		public string assembly;
	}

	[DebuggerDisplay("{header}")]
	internal class Include
	{
		public string header;
		public bool is_standard_header;
	}

	[DebuggerDisplay("{name} : {underlying_type}")]
	internal class Enum : Type
	{
		public Type underlying_type;
		
		public string comment;
		public string language;
		public readonly List<Enum_Value> values = new List<Enum_Value>();


		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
		public bool Is_CPlusPlus
		{
			get
			{
				return language.Contains("C++");
			}
		}
		public bool Is_CSharp
		{
			get
			{
				return language.Contains("C#");
			}
		}
	}

	[DebuggerDisplay("{name}")]
	internal class Enum_Value
	{
		public string name;
		public string comment;
		public string value;
		public uint index;
		
		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
		public bool Has_Value
		{
			get
			{
				return !string.IsNullOrEmpty(value);
			}
		}
	}

	[DebuggerDisplay("{name} {index}")]
	internal class Interface
	{
		public string name;
		public byte index;
		public string comment;
		public readonly List<Method> methods = new List<Method>();

		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
	}

	[DebuggerDisplay("{type} {name}(...)")]
	internal class Method
	{
		public Type type;
		public string name;
		public string comment;
		public uint index;
		public readonly List<Parameter> parameters = new List<Parameter>();

		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
		public bool Has_Return
		{
			get
			{
				return type != null;
			}
		}
	}

	[DebuggerDisplay("{type} {name}")]
	internal class Parameter
	{
		public Type type;
		public string name;
		public uint index;

		public bool Is_Out
		{
			get; set;
		}
	}

	internal class Stub
	{
		public string type;
		public string interface_type;
		
		public bool cplusplus;
		public bool csharp;
	}

	[DebuggerDisplay("{ref}")]
	internal class VerbatimRef
	{
		public string verbatim;
	}

	[DebuggerDisplay("{name}")]
	internal class Struct : Type
	{
		public string comment;
		public string language;
		public readonly List<Field> fields = new List<Field>();
		public readonly List<VerbatimRef> verbatims = new List<VerbatimRef>();

		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
		public bool Is_CPlusPlus
		{
			get
			{
				return language.Contains("C++");
			}
		}
		public bool Is_CSharp
		{
			get
			{
				return language.Contains("C#");
			}
		}
	}

	[DebuggerDisplay("{type} {name}")]
	internal class Field
	{
		public Type type;
		public string name;
		public string comment;
		public uint index;
		public uint? offset;
		
		public bool Has_Comment
		{
			get
			{
				return !string.IsNullOrEmpty(comment);
			}
		}
	}


	//an helper class to compute fields size and alignment, using same rules as C++ compiler
	internal static class DDL_Type_Analyzer
	{
		public static void Compute_Sizes_And_Alignments(Document document)
		{
			foreach(var ddl_enum in document.enums)
			{
				ddl_enum.size = ddl_enum.underlying_type.size;
				Debug.Assert(ddl_enum.size != Constant.Invalid_Size);
				ddl_enum.alignment = ddl_enum.size;
			}

			foreach(var ddl_struct in document.structs)
			{
				uint size = 0;
				foreach(var ddl_field in ddl_struct.fields)
				{
					uint field_alignment = ddl_field.type.alignment;
					Debug.Assert(field_alignment != Constant.Invalid_Alignment);

					uint field_size = ddl_field.type.size;
					Debug.Assert(field_size != Constant.Invalid_Size);
					
					ddl_struct.alignment = Math.Max(ddl_struct.alignment, field_alignment);

					size = Align_Value(size, field_alignment);
					ddl_field.offset = size;
					
					size += field_size;
				}
			
				size = Align_Value(size, ddl_struct.alignment);
				
				if (ddl_struct.size == Constant.Invalid_Size)
				{
					ddl_struct.size = size;
				}
				else
				{
					if (ddl_struct.size != size)
					{
						throw new FormatException("computed struct size doesn't match specified value: " + ddl_struct);
					}
				}
			}
		}

		private static uint Align_Value(uint value, uint alignment)
		{
			uint misalign = alignment > 1 ? value % alignment : 0;
			if (misalign != 0) value += alignment - misalign;
			return value;
		}
	}


	//an helper class to parse an XML file into a DDL document
	internal static class XML_Parser
	{
		//build a DDL document from XML sources, handling imports
		public static Document Load(string module_name, Func<string, Stream> load_delegate)
		{
			var doc_element = XElement.Load( load_delegate(module_name) );

			var document = new Document();
			document.name = module_name;
			document.comment = doc_element.Attribute_Value("Comment") ?? doc_element.Element_Value("Comment");
			document.cs_namespace = doc_element.Attribute_Value("Namespace");

			Add_Imports(document, doc_element);


			//recursively import moduless
			foreach(var ddl_import in document.imports)
			{
				var import_module = Load(ddl_import.module_name, load_delegate);
				ddl_import.module = import_module;

				//inherit includes and using from the imported modules
				document.includes.AddRange(import_module.includes);
				document.usings.AddRange(import_module.usings);
			}
			
			Add_Outputs(document, doc_element);
			Add_Usings(document, doc_element);
			Add_Includes(document, doc_element);
			Add_Builtin_Types(document, doc_element);
			Add_Enums(document, doc_element);
			Add_Structs(document, doc_element);
			Add_Interfaces(document, doc_element);
			Add_Stubs(document, doc_element);
			Add_Verbatims(document, doc_element);
			
			return document;
		}


		#region Internal
		private static void Add_Outputs(Document document, XElement doc_element)
		{
			foreach ( var output_element in doc_element.Elements("Output") )
			{
				var output = new Output();

				output.file = output_element.Attribute_Value("File");

				var language = output_element.Attribute_Value("Language");
				if (language == null) throw new FormatException("Output element without Language attribute: " + output_element);
				output.cplusplus = language == "C++";
				output.csharp = language == "C#";

				document.outputs.Add(output);
			}
		}
		private static void Add_Usings(Document document, XElement doc_element)
		{
			foreach ( var using_element in doc_element.Elements("Using") )
			{
				var using_ = new Using();

				using_.assembly = using_element.Attribute_Value("Source");

				document.usings.Add(using_);
			}
		}
		private static void Add_Includes(Document document, XElement doc_element)
		{
			foreach ( var include_element in doc_element.Elements("Include") )
			{
				var include = new Include();

				include.header = include_element.Attribute_Value("Source");
				include.is_standard_header = bool.Parse( include_element.Attribute_Value("Stdlib") ?? "false" );
				
				document.includes.Add(include);
			}
		}
		private static void Add_Stubs(Document document, XElement doc_element)
		{
			foreach ( var stub_element in doc_element.Elements("Stub") )
			{
				var stub = new Stub();

				stub.type = stub_element.Attribute_Value("Type");
				stub.interface_type = stub_element.Attribute_Value("Interface");

				var languages = stub_element.Attribute_Value("Language") ?? "C++,C#";
				stub.cplusplus = languages.Contains("C++");
				stub.csharp = languages.Contains("C#");

				document.stubs.Add(stub);
			}
		}
		private static void Add_Imports(Document document, XElement doc_element)
		{
			foreach ( var import_element in doc_element.Elements("Import") )
			{
				var ddl_import = new Import();

				ddl_import.module_name = import_element.Attribute_Value("Module");
				if (ddl_import.module_name == null) throw new Exception("Import without Module");
				
				document.imports.Add(ddl_import);
			}
		}
		private static void Add_Structs(Document document, XElement doc_element)
		{
			foreach ( var struct_element in doc_element.Elements("Struct") )
			{
				var ddl_struct = new Struct();

				ddl_struct.name = struct_element.Attribute_Value("Name");
				ddl_struct.cpp_name = ddl_struct.cs_name = ddl_struct.name;
				
				ddl_struct.comment = struct_element.Attribute_Value("Comment") ?? struct_element.Element_Value("Comment");
				ddl_struct.size = Utilities.Parse_Size_Or_Alignment( struct_element.Attribute_Value("Size") );
				ddl_struct.alignment = Utilities.Parse_Size_Or_Alignment( struct_element.Attribute_Value("Alignment") );
				ddl_struct.language = struct_element.Attribute_Value("Language") ?? "C#,C++";

				uint field_index = 0;
				foreach ( var field_element in struct_element.Elements("Field") )
				{
					var ddl_field = new Field();

					//default to void if return type is not specified
					
					ddl_field.type = document.Get_Type(  Utilities.Unescape_Typename( field_element.Attribute_Value("Type") )  );
					if (ddl_field.type == null) throw new FormatException("unknwon type in Field: " + ddl_field);
					ddl_field.name = field_element.Attribute_Value("Name");
					ddl_field.comment = field_element.Attribute_Value("Comment") ?? field_element.Element_Value("Comment");
					ddl_field.index = field_index++;

					var offset_string = field_element.Attribute_Value("Offset");
					if (offset_string != null)
					{
						ddl_field.offset = uint.Parse( field_element.Attribute_Value("Offset") );
					}
					
					ddl_struct.fields.Add(ddl_field);
				}

				foreach ( var verbatim_element in struct_element.Elements("VerbatimRef") )
				{
					var ddl_verbatimref = new VerbatimRef();

					 ddl_verbatimref.verbatim = verbatim_element.Value;

					ddl_struct.verbatims.Add(ddl_verbatimref);
				}

				document.structs.Add(ddl_struct);
			}
		}
		private static void Add_Enums(Document document, XElement doc_element)
		{
			foreach ( var enum_element in doc_element.Elements("Enum") )
			{
				var ddl_enum = new Enum();

				ddl_enum.name = enum_element.Attribute_Value("Name");
				ddl_enum.cpp_name = ddl_enum.cs_name = ddl_enum.name;
				ddl_enum.underlying_type = document.Get_Type( enum_element.Attribute_Value("Type") );
				if (ddl_enum.underlying_type == null) throw new FormatException("Enum without Type attribute: " + ddl_enum);
				ddl_enum.comment = enum_element.Attribute_Value("Comment") ?? enum_element.Element_Value("Comment");
				ddl_enum.language = enum_element.Attribute_Value("Language") ?? "C#,C++";


				uint value_index = 0;
				foreach ( var item_element in enum_element.Elements("Item") )
				{
					var ddl_value = new Enum_Value();

					ddl_value.name = item_element.Attribute_Value("Name");
					ddl_value.value = item_element.Attribute_Value("Value");
					ddl_value.comment = item_element.Attribute_Value("Comment") ?? item_element.Element_Value("Comment");
					ddl_value.index = value_index++;

					ddl_enum.values.Add(ddl_value);
				}


				document.enums.Add(ddl_enum);
			}
		}
		private static void Add_Interfaces(Document document, XElement doc_element)
		{
			foreach ( var interface_element in doc_element.Elements("Interface") )
			{
				var ddl_interface = new Interface();

				ddl_interface.name = interface_element.Attribute_Value("Name");
				ddl_interface.index = (byte)uint.Parse(interface_element.Attribute_Value("Index"));
				ddl_interface.comment = interface_element.Attribute_Value("Comment") ?? interface_element.Element_Value("Comment");


				uint method_index = 0;
				foreach (var method_element in interface_element.Elements("Method"))
				{
					var ddl_method = new Method();

					//default to void if return type is not specified
					var return_typename = method_element.Attribute_Value("Return");
					
					if (return_typename != null)
					{
						ddl_method.type = document.Get_Type( Utilities.Unescape_Typename(return_typename) );
						if (ddl_method.type == null) throw new FormatException("unknown return type for Method:: " + ddl_method);
					}

					ddl_method.name = method_element.Attribute_Value("Name");
					ddl_method.comment = method_element.Attribute_Value("Comment") ?? method_element.Element_Value("Comment");
					ddl_method.index = method_index++;

					uint arg_index = 0;
					foreach (var arg_element in method_element.Elements("Arg"))
					{
						var parameter = new Parameter();
						parameter.index = arg_index++;
						parameter.type = document.Get_Type(  Utilities.Unescape_Typename( arg_element.Attribute_Value("Type") )  );
						if(parameter.type == null) throw new FormatException("unknown argument type: " + arg_element);

						parameter.name = arg_element.Attribute_Value("Name");
						parameter.Is_Out = ( arg_element.Attribute_Value("Modifiers") ?? "" ).Contains("Out");

						ddl_method.parameters.Add(parameter);
					}

					ddl_interface.methods.Add(ddl_method);
				}
				
				document.interfaces.Add(ddl_interface);
			}
		}
		private static void Add_Builtin_Types(Document document, XElement doc_element)
		{
			foreach ( var builtin_element in doc_element.Elements("Builtin_Type") )
			{
				var ddl_builtin = new Builtin_Type();

				ddl_builtin.name = Utilities.Unescape_Typename( builtin_element.Attribute_Value("Name") );
				ddl_builtin.cs_name = Utilities.Unescape_Typename( builtin_element.Attribute_Value("CS_Name") ?? ddl_builtin.name );
				ddl_builtin.cs_converter = builtin_element.Attribute_Value("CS_Converter");
				ddl_builtin.cpp_name = Utilities.Unescape_Typename( builtin_element.Attribute_Value("CPP_Name") ?? ddl_builtin.name );
				ddl_builtin.size = uint.Parse( builtin_element.Attribute_Value("Size") );
				ddl_builtin.alignment = uint.Parse(builtin_element.Attribute_Value("Alignment") ?? "1");
				ddl_builtin.cpp_pass_by_ref = (builtin_element.Attribute_Value("CPP_Pass_By") ?? "Enum_Value") == "Ref";

				document.builtins.Add(ddl_builtin);
			}
		}
		private static void Add_Verbatims(Document document, XElement doc_element)
		{
			foreach ( var verbatim_element in doc_element.Elements("Verbatim") )
			{
				var ddl_verbatim = new Verbatim();

				ddl_verbatim.name = verbatim_element.Attribute_Value("Name");
				ddl_verbatim.text = verbatim_element.Value.Trim();
				ddl_verbatim.language = verbatim_element.Attribute_Value("Language") ?? "C#,C++";

				document.verbatims.Add(ddl_verbatim);
			}
		}
		#endregion
	}
}//namespace DDL
}//namespace Lemniscate