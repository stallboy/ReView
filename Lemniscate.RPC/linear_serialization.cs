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
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


/////////////////////////////////////////////
// PARENTAL ADVISORY - EXPLICIT REFLECTION //
/////////////////////////////////////////////


namespace Lemniscate
{
	//wrappers around string, to define the encoding to use when serializing
	public class UTF16_String
	{
		public UTF16_String(string text)
		{
			this.text = text;
		}
		public static implicit operator string(UTF16_String s)
		{
			return s.text;
		}
		public static implicit operator UTF16_String(string s)
		{
			return new UTF16_String(s);
		}

		public string As_String { get { return text;} }

		string text;
	}
	
	public static class UTF16_String_Serialization_Extensions
	{
		public static void Serialize(this Linear_Serializer serializer, UTF16_String value)
		{
			serializer.Serialize_String(value.As_String, Linear_Serializer.String_Encoding.UTF16);
		}
		public static void Deserialize(this Linear_Serializer serializer, out UTF16_String value)
		{
			string text;
			serializer.Deserialize_String(out text, Linear_Serializer.String_Encoding.UTF16);
			value = text;
		}
	}
	
	public class UTF8_String
	{
		public UTF8_String(string text)
		{
			this.text = text;
		}
		public static implicit operator string(UTF8_String s)
		{
			return s.text;
		}
		public static implicit operator UTF8_String(string s)
		{
			return new UTF8_String(s);
		}

		public string As_String { get { return text;} }

		string text;
	}

	public static class UTF8_String_Serialization_Extensions
	{
		public static void Serialize(this Linear_Serializer serializer, UTF8_String value)
		{
			serializer.Serialize_String(value.As_String, Linear_Serializer.String_Encoding.UTF8);
		}
		public static void Deserialize(this Linear_Serializer serializer, out UTF8_String value)
		{
			string text;
			serializer.Deserialize_String(out text, Linear_Serializer.String_Encoding.UTF8);
			value = text;
		}
	}



	public class Linear_Serializer
	{
		public enum Array_Length_Encoding
		{
			Variable_Length_7Bits_Encoding,
			Fixed_Length_32bits
		}

		public enum String_Encoding
		{
			//TODO: perhaps we could use the Encoding.*.CodePage property and then fetch it via GetEncoding.
			UTF8,
			UTF16,
		}


		public Linear_Serializer(Stream stream, bool swap_endianness = false, Array_Length_Encoding array_length_encoding = Array_Length_Encoding.Variable_Length_7Bits_Encoding)
		{
			this.stream = stream;
			this.swap_endianness = swap_endianness;
			this.array_length_encoding = array_length_encoding;
		}
		
		//NOTE: C# doesn't support template specialization, so to kind of mimick that we:
		//		1) use overloading (meaning even for Deserialize we use out, instead of just returning)
		//		2) use runtime reflection to bind to the given extension method (done only once, using static constructors)
		public void Serialize<T>(T value)
		{
			//primitive types will be handled here...
			if (Bit_Conversion<T>.Is_Supported)
			{
				var bytes = Bit_Conversion<T>.To_Bytes(value);
				if (swap_endianness)
				{
					Array.Reverse(bytes);
				}
				stream.Write(bytes, 0, bytes.Length);
			}
			else
			{
				//...everything else will go via their extension, and come back to this method for individual fields
				Linear_Serialization_Helper<T>.Serialize(this, value);
			}
		}

		public void Deserialize<T>(out T value)
		{
			if (Bit_Conversion<T>.Is_Supported)
			{
				var bytes = stream.Read_Bytes(Bit_Conversion<T>.Bytes_Needed);
				if (swap_endianness)
				{
					Array.Reverse(bytes);
				}
				value = Bit_Conversion<T>.From_Bytes(bytes);
			}
			else
			{
				Linear_Serialization_Helper<T>.Deserialize(this, out value);
			}
		}

		public void Serialize<T>(T[] data)
		{
			if (data != null)
			{
				var length = (ulong) data.Length;
				Serialize_Array_Length(length);
				for(int i=0 ; i<data.Length ; i++)
				{
					Serialize(data[i]);
				}
			}
			else
			{
				stream.WriteByte(0);
			}
		}

		public void Deserialize<T>(out T[] data)
		{
			var length = Deserialize_Array_Length();
			if (length == 0)
			{
				data = null;
			}
			else
			{
				data = new T[length];
				for(int i=0 ; i<data.Length ; i++)
				{
					Deserialize(out data[i]);
				}
			}
		}

		public void Serialize(byte value)
		{
			stream.WriteByte(value);
		}

		public void Deserialize(out byte value)
		{
			value = stream.Read_Byte();
		}

		public void Serialize(byte[] data)
		{
			if (data != null)
			{
				var length = (ulong) data.Length;
				Serialize_Array_Length(length);
				stream.Write(data, 0, data.Length);
			}
			else
			{
				stream.WriteByte(0);
			}
		}

		public void Deserialize(out byte[] data)
		{
			var length = Deserialize_Array_Length();
			if (length == 0)
			{
				data = null;
			}
			else
			{
				data = stream.Read_Bytes( (int) length );
			}
		}

		public void Serialize_String(string text, String_Encoding encoding)
		{
			var string_encoder = Get_Encoder(encoding, swap_endianness);
			if (text != null)
			{
				//NOTE: encoder handling byte ordering
				var buffer = string_encoder.GetBytes(text);
				Serialize(buffer);
			}
			else
			{
				stream.WriteByte(0);
			}
		}

		public void Deserialize_String(out string text, String_Encoding encoding)
		{
			byte[] buffer;
			Deserialize(out buffer);

			if (buffer != null)
			{ 
				var string_encoder = Get_Encoder(encoding, swap_endianness);
				text = string_encoder.GetString(buffer);
			}
			else
			{
				text = null;
			}
		}

		//by default serialize string as UTF8. wrap with a UTF16_String to encode differently
		public void Serialize(string text)
		{
			Serialize_String(text, String_Encoding.UTF8);
		}

		public void Deserialize(out string text)
		{
			Deserialize_String(out text, String_Encoding.UTF8);
		}

		public void Serialize_Array_Length(ulong value)
		{
			switch(array_length_encoding)
			{
			case Array_Length_Encoding.Variable_Length_7Bits_Encoding:
				Serialize_7Bits_Encoded_Uinteger(value);
				break;

			case Array_Length_Encoding.Fixed_Length_32bits:
				Debug.Assert(value < uint.MaxValue);
				Serialize( (uint)value );
				break;

			default:
				throw new NotSupportedException();
			}
		}

		public ulong Deserialize_Array_Length()
		{
			switch(array_length_encoding)
			{
			case Array_Length_Encoding.Variable_Length_7Bits_Encoding:
				return Deserialize_7Bits_Encoded_Uinteger();

			case Array_Length_Encoding.Fixed_Length_32bits:
				uint length;
				Deserialize(out length);
				return length;

			default:
				throw new NotSupportedException();
			}
		}

		public void Serialize_7Bits_Encoded_Uinteger(ulong value)
		{
			for (;;)
			{
				if (value < 0x80)
				{
					var block = (byte) value;
					stream.WriteByte(block);
					return;
				}
				else
				{
					var byte_value = (byte)( (value & 0x7F) | 0x80 );
					stream.WriteByte(byte_value);
					value = value >> 7;
				}
			}
		}

		public ulong Deserialize_7Bits_Encoded_Uinteger()
		{
			ulong value = 0;
			for (int i=0 ; ; i++)
			{
				byte block = stream.Read_Byte();
				value |= ( (ulong)(block & 0x7F) ) << 7*i;
				if ((block & 0x80) != 0x80) return value;
			}
		}

		public void Flush()
		{
			stream.Flush();
		}

		public T Deserialize<T>()
		{
			T value;
			Deserialize(out value);
			return value;
		}

		public void Close()
		{
			stream.Dispose();
			stream = null;
		}

		#region Private
		private static Encoding Get_Encoder(String_Encoding string_encoding, bool swap_endianness)
		{
			//get the right encoder, also keeping byte ordering into account.
			//this way there would be no need to individually flip characters as the encoder will take care of that
			return	string_encoding == String_Encoding.UTF8	?	Encoding.UTF8
				:	swap_endianness							?	Encoding.BigEndianUnicode
				:												Encoding.Unicode;
		}
		#endregion


		#region Fields
		//how to encode the array length prefix?
		Array_Length_Encoding array_length_encoding;

		//shall we flip byte ordering when transmitting/receiving?
		bool swap_endianness;
		
		//the input/output stream
		private Stream stream;
		#endregion
	}

	//dynamically resolve to the extension methods for the type. must be defined in the same assembly as the type T
	public static class Linear_Serialization_Helper<T>
	{
		public static void Serialize(Linear_Serializer serializer, T value)
		{
			serialize(serializer, value);
		}

		public static void Deserialize(Linear_Serializer serializer, out T value)
		{
			deserialize(serializer, out value);
		}

		#region Internals
		static void Serialize_Primitive(Linear_Serializer serializer, T value)
		{
			serializer.Serialize(value);
		}

		static void Deserialize_Primitive(Linear_Serializer serializer, out T value)
		{
			serializer.Deserialize(out value);
		}

		static void Serialize_Array(Linear_Serializer serializer, T[] array)
		{
			serializer.Serialize(array);
		}

		static void Deserialize_Array(Linear_Serializer serializer, out T[] array)
		{
			serializer.Deserialize(out array);
		}

		static void Serialize_Byte_Array(Linear_Serializer serializer, byte[] array)
		{
			serializer.Serialize(array);
		}

		static void Deserialize_Byte_Array(Linear_Serializer serializer, out byte[] array)
		{
			serializer.Deserialize(out array);
		}

		static Linear_Serialization_Helper()
		{
			if (Bit_Conversion<T>.Is_Supported)
			{ 
				var type = typeof(Linear_Serialization_Helper<T>);
				var method = type.GetMethod("Serialize_Primitive", BindingFlags.NonPublic | BindingFlags.Static);
				serialize = (Serialize_Delegate) Delegate.CreateDelegate(typeof(Serialize_Delegate), method);

				method = type.GetMethod("Deserialize_Primitive", BindingFlags.NonPublic | BindingFlags.Static);
				deserialize = (Deserialize_Delegate) Delegate.CreateDelegate(typeof(Deserialize_Delegate), method);

				return;
			}

			//byte[] fast path
			if ( typeof(T) == typeof(byte[]) )
			{
				var type = typeof(Linear_Serialization_Helper<T>);
				var method = type.GetMethod("Serialize_Byte_Array", BindingFlags.NonPublic | BindingFlags.Static);
				serialize = (Serialize_Delegate) Delegate.CreateDelegate(typeof(Serialize_Delegate), method);

				method = type.GetMethod("Deserialize_Byte_Array", BindingFlags.NonPublic | BindingFlags.Static);
				deserialize = (Deserialize_Delegate) Delegate.CreateDelegate(typeof(Deserialize_Delegate), method);

				return;
			}

			if (typeof(T).IsArray)
			{
				var type = typeof(Linear_Serialization_Helper<>).MakeGenericType( typeof(T).GetElementType() );
				var method = type.GetMethod("Serialize_Array", BindingFlags.NonPublic | BindingFlags.Static);
				serialize = (Serialize_Delegate) Delegate.CreateDelegate(typeof(Serialize_Delegate), method);

				method = type.GetMethod("Deserialize_Array", BindingFlags.NonPublic | BindingFlags.Static);
				deserialize = (Deserialize_Delegate) Delegate.CreateDelegate(typeof(Deserialize_Delegate), method);

				return;
			}

			foreach( var assembly in All_Assemblies() )
			{
				try
				{
					var method = Get_Extension_Methods( assembly, "Serialize", typeof(T) ).SingleOrDefault();
					if (method == null && typeof(T).IsGenericType)
					{
						method = Get_Extension_Methods( assembly, "Serialize", typeof(T).GetGenericTypeDefinition() ).SingleOrDefault();
						if (method == null) continue;

						method = method.MakeGenericMethod(typeof(T).GetGenericArguments());
					}
					if (method == null) continue;
					serialize = (Serialize_Delegate) Delegate.CreateDelegate(typeof(Serialize_Delegate), method);



					method = Get_Extension_Methods( assembly, "Deserialize", typeof(T).MakeByRefType() ).SingleOrDefault();
					if (method == null && typeof(T).IsGenericType)
					{
						method = Get_Extension_Methods( assembly, "Deserialize", typeof(T).GetGenericTypeDefinition().MakeByRefType() ).SingleOrDefault();
						if (method == null) continue;

						method = method.MakeGenericMethod(typeof(T).GetGenericArguments());
					}
					if (method == null) continue;
					deserialize = (Deserialize_Delegate) Delegate.CreateDelegate(typeof(Deserialize_Delegate), method);

					return;
				}
				catch(Exception exception)
				{
					throw new NotSupportedException("unable to fetch serialization extensions for " + typeof(T) + " from " + assembly, exception);
				}
			}

			throw new NotSupportedException( "unable to find serialization extensions for " + typeof(T) );
		}

		delegate void Serialize_Delegate(Linear_Serializer serializer, T value);
		delegate void Deserialize_Delegate(Linear_Serializer serializer, out T value);
		
		static Serialize_Delegate serialize;
		static Deserialize_Delegate deserialize;
		
		//modified from a dynamic extension methods searcher from Jon Skeet
		private static IEnumerable<MethodInfo> Get_Extension_Methods(Assembly assembly, string method_name, Type argument_type)
		{
			return	//method must be coming from the this assembly
					from type in assembly.GetTypes()
					//extensions methods are defined in static classes, so let's filter by these conditions
					where type.IsSealed && !type.IsGenericType && !type.IsNested
					//get all methods from those classes
					from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					//that are extensions
					where method.IsDefined(typeof(ExtensionAttribute), false)
					//that have the given name
					where method.Name == method_name
					//that have exactly 2 arguments
					where method.GetParameters().Length == 2
					//where the first one is the linear serializer
					where method.GetParameters()[0].ParameterType == typeof(Linear_Serializer)
					//and the second argument matches the type we're looking for
					where Is_Matching_Parameter_Type(method.GetParameters()[1].ParameterType, argument_type)
					//well... if we got this far... we really want this method :)
					select method;
		}

		private static bool Is_Matching_Parameter_Type(Type parameter_type, Type argument_type)
		{
			//the argument matches exactly the type we're looking for
			if (parameter_type == argument_type) return true;
			
			if (parameter_type.ContainsGenericParameters)
			{
				//or alternatively is a generic type that matches our argument
				if (parameter_type.IsByRef)
				{
					return parameter_type.GetElementType().GetGenericTypeDefinition().MakeByRefType() == argument_type;
				}
				else
				{
					return parameter_type.GetGenericTypeDefinition() == argument_type;
				}
			}
			return false;
		}

		//return all assemblies to look into, starting from the one containing the type (which should be the most likely to contain the extensions)
		private static IEnumerable<Assembly> All_Assemblies()
		{
			var assembly = Assembly.GetAssembly( typeof(T) );
			return new Assembly[]{assembly}.Concat( AppDomain.CurrentDomain.GetAssemblies().Where(a => a != assembly) );
		}

		#endregion
	}
}