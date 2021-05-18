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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;



namespace Lemniscate
{
	public static class Reflection
	{
		//a dynamic extension methods searcher from Jon Skeet
		public static IEnumerable<MethodInfo> Get_Extension_Methods(Assembly assembly, Type extended_type)
		{
			return	from type in assembly.GetTypes()
					where type.IsSealed && !type.IsGenericType && !type.IsNested
					from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					where method.IsDefined(typeof(ExtensionAttribute), false)
					where method.GetParameters()[0].ParameterType == extended_type
					select method;
		}
	}

	public static class Collection_Extensions
	{
		#region IDictionary Extensions
		public static TValue Get_Value_Or_Default<TKey, TValue>(this IDictionary<TKey,TValue> dictionary, TKey key)
		{
			TValue value;
			if ( dictionary.TryGetValue(key, out value) )
			{
				return value;
			}
			else
			{
				return default(TValue);
			}
		}
		#endregion


		#region NameValueCollection Extensions
		public static string First_Value_Or_Default(this NameValueCollection collection, string key)
		{
			var values = collection.GetValues(key);
			return values.Length > 0 ? values[0] : null;
		}

		public static IEnumerable<KeyValuePair<string, string>> As_KeyValue_Pairs(this NameValueCollection collection)
		{
			return collection.AllKeys.SelectMany(	collection.GetValues,
													(k, v) => new KeyValuePair<string, string>(k, v)	);
		}
		#endregion
	}



	public static class Dictionary_As_Cache_Extensions
	{
		//get an entry from the cache, eventually adding it if not present
		public static TValue Get_Value_Or_Add<TKey, TValue>(this Dictionary<TKey, TValue> cache, TKey key, Func<TValue> add_entry_delegate)
		{
			TValue value;
			if (cache.TryGetValue(key, out value) == false)
			{
				value = add_entry_delegate();
				cache.Add(key, value);
			}
			return value;
		}
		public static TValue Get_Value_Or_Add<TKey, TValue>(this Dictionary<TKey, TValue> cache, TKey key, Func<TKey, TValue> add_entry_delegate)
		{
			TValue value;
			if (cache.TryGetValue(key, out value) == false)
			{
				value = add_entry_delegate(key);
				cache.Add(key, value);
			}
			return value;
		}

		//remove elements, if present
		public static void Evict_Keys<TKey, TValue>(this Dictionary<TKey, TValue> cache, params TKey[] keys)
		{
			foreach (var key in keys)
			{
				if ( cache.ContainsKey(key) ) cache.Remove(key);
			}
		}
	}



	public static class Stream_Extensions
	{
		public static byte Read_Byte(this Stream stream)
		{
			int byte_value = -1;
			while (byte_value < 0)
			{
				byte_value = stream.ReadByte();
			}

			if (byte_value == -1) throw new IOException();
			return (byte)byte_value;
		}

		public static byte[] Read_Bytes(this Stream stream, int size)
		{
			var buffer = new byte[size];
			stream.Read_Bytes(buffer, 0, size);
			return buffer;
		}

		public static void Read_Bytes(this Stream stream, byte[] buffer, int offset, int size)
		{
			Debug.Assert(offset + size <= buffer.Length);

			while (size > 0)
			{
				var length = stream.Read(buffer, offset, size);
				if (length == 0)
				{
					throw new IOException("stream is shorter than expected");
				}
				size -= length;
				offset += length;
			}
		}

		public static bool Try_Read_Bytes(this Stream stream, byte[] buffer, int offset, int size)
		{
			Debug.Assert(offset + size <= buffer.Length);

			while (size > 0)
			{
				var length = stream.Read(buffer, offset, size);
				if (length == 0)
				{
					return false;
				}
				size -= length;
				offset += length;
			}
			return true;
		}
	}



	public static class LINQ_To_XML_Extensions
	{
		public static string Attribute_Value(this XElement element, string attribute_name)
		{
			var attribute = element.Attribute(attribute_name);
			return attribute != null ? attribute.Value : null;
		}
 
		public static string Element_Value(this XElement element, string sub_element_name)
		{
			var sub_element = element.Element(sub_element_name);
			return sub_element != null ? sub_element.Value : null;
		}
	}
}