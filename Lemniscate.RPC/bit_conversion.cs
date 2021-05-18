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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;



namespace Lemniscate
{
	public static class Bit_Conversion
	{
		public static bool Is_Supported(object obj)
		{
			return Is_Supported( obj.GetType() );
		}
		public static bool Is_Supported(Type type)
		{
			//TODO: is this equivalent:
			//return type.IsPrimitive || type.IsEnum;

			if		(type == typeof(Byte)		)	return Bit_Conversion<Byte>.Is_Supported;
			else if	(type == typeof(SByte)		)	return Bit_Conversion<SByte>.Is_Supported;
			else if	(type == typeof(Boolean)	)	return Bit_Conversion<Boolean>.Is_Supported;
			else if	(type == typeof(Int16)		)	return Bit_Conversion<Int16>.Is_Supported;
			else if	(type == typeof(UInt16)		)	return Bit_Conversion<UInt16>.Is_Supported;
			else if	(type == typeof(Int32)		)	return Bit_Conversion<Int32>.Is_Supported;
			else if	(type == typeof(UInt32)		)	return Bit_Conversion<UInt32>.Is_Supported;
			else if	(type == typeof(Int64)		)	return Bit_Conversion<Int64>.Is_Supported;
			else if	(type == typeof(UInt64)		)	return Bit_Conversion<UInt64>.Is_Supported;
			else if	(type == typeof(Single)		)	return Bit_Conversion<Single>.Is_Supported;
			else if	(type == typeof(Double)		)	return Bit_Conversion<Double>.Is_Supported;
			else if (type.IsEnum)
			{
				return Is_Supported( type.GetEnumUnderlyingType() );
			}
			else
			{
				return false;
			}
		}

		public static object Convert_Enum_To_Underlying_Type(Enum enum_value)
		{
			var enum_type = enum_value.GetType();

			var under_type = Enum.GetUnderlyingType(enum_type);

			return Convert.ChangeType(enum_value, under_type);
		}

		public static byte[] To_Bytes(object obj)
		{
			Type type = obj.GetType();

			if		(type == typeof(Byte)		)	return Bit_Conversion<Byte>.To_Bytes( (Byte) obj );
			else if	(type == typeof(SByte)		)	return Bit_Conversion<SByte>.To_Bytes( (SByte) obj );
			else if	(type == typeof(Boolean)	)	return Bit_Conversion<Boolean>.To_Bytes( (Boolean) obj );
			else if	(type == typeof(Int16)		)	return Bit_Conversion<Int16>.To_Bytes( (Int16) obj );
			else if	(type == typeof(UInt16)		)	return Bit_Conversion<UInt16>.To_Bytes( (UInt16) obj );
			else if	(type == typeof(Int32)		)	return Bit_Conversion<Int32>.To_Bytes( (Int32) obj );
			else if	(type == typeof(UInt32)		)	return Bit_Conversion<UInt32>.To_Bytes( (UInt32) obj );
			else if	(type == typeof(Int64)		)	return Bit_Conversion<Int64>.To_Bytes( (Int64) obj );
			else if	(type == typeof(UInt64)		)	return Bit_Conversion<UInt64>.To_Bytes( (UInt64) obj );
			else if	(type == typeof(Single)		)	return Bit_Conversion<Single>.To_Bytes( (Single) obj );
			else if	(type == typeof(Double)		)	return Bit_Conversion<Double>.To_Bytes( (Double) obj );
			else if (type.IsEnum)
			{
				return To_Bytes(  Convert_Enum_To_Underlying_Type( (Enum)obj )  );
			}
			else if	(type == typeof(Char) )
			{
				throw new NotSupportedException("Chars should be converted via a given Encoding!");
			}
			else
			{
				throw new NotSupportedException("non templated Bit_Conversion can only be used for primitive types");
			}
		}
		public static void To_Bytes(object obj, byte[] buffer, int index = 0)
		{
			var bytes = To_Bytes(obj);
			Array.Copy(bytes, 0, buffer, index, bytes.Length);
		}
		public static object From_Bytes(Type type, byte[] buffer, int index = 0)
		{
			if		(type == typeof(Byte)		)	return Bit_Conversion<Byte>.From_Bytes(buffer, index);
			else if	(type == typeof(SByte)		)	return Bit_Conversion<SByte>.From_Bytes(buffer, index);
			else if	(type == typeof(Boolean)	)	return Bit_Conversion<Boolean>.From_Bytes(buffer, index);
			else if	(type == typeof(Int16)		)	return Bit_Conversion<Int16>.From_Bytes(buffer, index);
			else if	(type == typeof(UInt16)		)	return Bit_Conversion<UInt16>.From_Bytes(buffer, index);
			else if	(type == typeof(Int32)		)	return Bit_Conversion<Int32>.From_Bytes(buffer, index);
			else if	(type == typeof(UInt32)		)	return Bit_Conversion<UInt32>.From_Bytes(buffer, index);
			else if	(type == typeof(Int64)		)	return Bit_Conversion<Int64>.From_Bytes(buffer, index);
			else if	(type == typeof(UInt64)		)	return Bit_Conversion<UInt64>.From_Bytes(buffer, index);
			else if	(type == typeof(Single)		)	return Bit_Conversion<Single>.From_Bytes(buffer, index);
			else if	(type == typeof(Double)		)	return Bit_Conversion<Double>.From_Bytes(buffer, index);
			else if (type.IsEnum)
			{
				var obj = From_Bytes( type.GetEnumUnderlyingType(), buffer, index );
				return Convert.ChangeType(obj, type);
			}
			else if	(type == typeof(Char) )
			{
				throw new NotSupportedException("Chars should be converted via a given Encoding!");
			}
			else
			{
				throw new NotSupportedException("non templated Bit_Conversion can only be used for primitive types");
			}
		}

		public static int Bytes_Needed(Type type)
		{
			if		(type == typeof(Byte)		)	return Bit_Conversion<Byte>.Bytes_Needed;
			else if	(type == typeof(SByte)		)	return Bit_Conversion<SByte>.Bytes_Needed;
			else if	(type == typeof(Boolean)	)	return Bit_Conversion<Boolean>.Bytes_Needed;
			else if	(type == typeof(Int16)		)	return Bit_Conversion<Int16>.Bytes_Needed;
			else if	(type == typeof(UInt16)		)	return Bit_Conversion<UInt16>.Bytes_Needed;
			else if	(type == typeof(Int32)		)	return Bit_Conversion<Int32>.Bytes_Needed;
			else if	(type == typeof(UInt32)		)	return Bit_Conversion<UInt32>.Bytes_Needed;
			else if	(type == typeof(Int64)		)	return Bit_Conversion<Int64>.Bytes_Needed;
			else if	(type == typeof(UInt64)		)	return Bit_Conversion<UInt64>.Bytes_Needed;
			else if	(type == typeof(Single)		)	return Bit_Conversion<Single>.Bytes_Needed;
			else if	(type == typeof(Double)		)	return Bit_Conversion<Double>.Bytes_Needed;
			else if (type.IsEnum)
			{
				return Bytes_Needed( type.GetEnumUnderlyingType() );
			}
			else
			{
				throw new NotSupportedException("non templated Bit_Conversion can only be used for primitive types");
			}
		}
	}

	//a generics wrapper around BitConverter from Marc Gravell
	public static class Bit_Conversion<T>
	{
		#region Private
		static Bit_Conversion()
		{
			if (typeof(T).IsEnum)
			{
				to_bytes = (T en) =>
				{
					return Bit_Conversion.To_Bytes(en);
				};

				from_bytes = (byte[] bytes, int offset) =>
				{
					return (T) Bit_Conversion.From_Bytes(typeof(T), bytes, offset);
				};

				bytes_needed = Bit_Conversion.Bytes_Needed( typeof(T) );

				return;
			}
			else if ( typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) )
			{
				//there's no BitConverter method for these types... let's create our own delegates

				//TODO: couldn't find a way of converting directly, without going through boxing/unboxing...
				to_bytes = (T value) =>
				{
					return new byte[1]{(byte)(object)(value)};
				};

				from_bytes = (byte[] bytes, int offset) =>
				{
					return (T) (object) bytes[offset];
				};

				bytes_needed = 1;
			}
			else //other types that should be supported by the BitConverter
			{
				try
				{
					var write_method = typeof(BitConverter).GetMethod("GetBytes", new Type[] { typeof(T) });
					if (write_method == null) return;
			
					var read_method = typeof(BitConverter).GetMethod("To" + typeof(T).Name, new Type[] {typeof(byte[]), typeof(int)});
					if (read_method == null) return;

					to_bytes = (Func<T, byte[]>) Delegate.CreateDelegate(typeof(Func<T, byte[]>), write_method);
					from_bytes = (Func<byte[], int, T>) Delegate.CreateDelegate(typeof(Func<byte[], int, T>), read_method);

					//TODO: write a unit test to check this actually matches To_Bytes( default(T) ).Length
					bytes_needed = To_Bytes( default(T) ).Length;
				}
				catch(AmbiguousMatchException exception)
				{
					Code.Unreferenced_Params(exception);

					//type is unsupported
					to_bytes = null;
					from_bytes = null;
					bytes_needed = 0;
				}
			}
		}

		static Func<byte[], int, T> from_bytes;
		static Func<T, byte[]> to_bytes;
		static int bytes_needed;
		#endregion


		#region Public
		public static T From_Bytes(byte[] buffer, int index = 0)
		{
			return from_bytes(buffer, index);
		}
		public static byte[] To_Bytes(T value)
		{
			return to_bytes(value);
		}
		public static byte[] To_Bytes(object value)
		{
			return to_bytes( (T) value );
		}
		public static bool Is_Supported
		{
			get
			{
				return to_bytes != null;
			}
		}
		public static int Bytes_Needed
		{
			get
			{
				return bytes_needed;
			}
		}
		#endregion
	}
}
