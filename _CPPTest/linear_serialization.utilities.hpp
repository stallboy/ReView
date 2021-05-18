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
// THE SOFTWARE.#ifndef BASE_SERIALIZATION_UTILS_HPP_INCLUDED
#define BASE_SERIALIZATION_UTILS_HPP_INCLUDED

#include "base.common.hpp"
#include "linear_serialization.hpp"
#include "lowlevel.hpp"



//a type that serializes/deserializes as a variable sized 7bits encoded uinteger
class Uinteger_7Bits_Encoded
{
public:
	Uinteger_7Bits_Encoded(uinteger value) : value(value){}
	void operator =(uinteger new_value) { value = new_value;}
	template<typename T> operator T() {return Lossless_Cast<T>(value);}

public:
	static uinteger Needed_Bytes(uinteger value)
	{
		uinteger needed_bytes = 1;
		while (value >= 0x80)
		{
			value = value >> 7;
			needed_bytes++;
		}
		return needed_bytes;
	}

	static uinteger Convert_To_Bytes(uinteger value, byte * const buffer)
	{
		for (uinteger i=0 ; ; i++)
		{
			if (value < 0x80)
			{
				buffer[i] = Lossless_Cast<byte>(value);
				return i + 1;
			}
			else
			{
				buffer[i] = Lossless_Cast<byte>(value & 0x7F | 0x80);
				value = value >> 7;
			}
		}
	}

private:
	uinteger value;
};

template <typename S>
void Serialize(S & stream, Uinteger_7Bits_Encoded value)
{
	Serialize_7Bit_Encoded_Uinteger(stream, value);
}

template <typename S>
void Deserialize(S & stream, Uinteger_7Bits_Encoded & value)
{
	value = Deserialize_7Bit_Encoded_Uinteger(stream);
}



//serialize to this stream to compute the total number of bytes required for serialization
struct Length_Counting_Stream
{
	void operator += (uinteger value) {length += value;}
	uinteger length = 0;
};

template<> inline
void Serialize(Length_Counting_Stream& stream, byte const *, uinteger const length)
{
	stream += length;
}



//can be used to construct packets with a known small max-size
template<uinteger N>
struct Packet_Builder
{
//user API
public:
	byte const * Pointer_To_Packet() const
	{
		RUNTIME_ASSERT(prefix_length != 0);
		return buffer + 10 - prefix_length;
	}
	uinteger Total_Packet_Length() const
	{
		RUNTIME_ASSERT(prefix_length != 0);
		return length + prefix_length;
	}


//serializer API
private:
	//add bytes to the packet builder
	void Write_Bytes(byte const * const array, uinteger const count)
	{
		RUNTIME_ASSERT(length + count <= N);
		Copy_Memory(buffer + 10 + length, array, count);
		length += count;
	}
	
	//computes length, and writes it in the reserved header at the beginning of the packet
	void Prepend_Length()
	{
		prefix_length = Uinteger_7Bits_Encoded::Needed_Bytes(length);
		auto const p = buffer + 10 - prefix_length;
		auto const used_bytes = Uinteger_7Bits_Encoded::Convert_To_Bytes(length, p);
		RUNTIME_ASSERT(used_bytes == prefix_length);
	}

	template<uinteger n> friend void Serialize(Packet_Builder<n> &, byte const * array, uinteger);
	template<uinteger n> friend void Flush(Packet_Builder<n> &);


private:
	uinteger prefix_length = 0;
	uinteger length = 0;
	byte buffer[10 + N]; //10 is the max size needed to 7bit encode a uint64
};

template<uinteger n> inline
void Serialize(Packet_Builder<n>& stream, byte const * const array, uinteger const length)
{
	stream.Write_Bytes(array, length);
}

template<uinteger n> inline
void Flush(Packet_Builder<n> & stream)
{
	stream.Prepend_Length();
}



#endif//BASE_SERIALIZATION_UTILS_HPP_INCLUDED