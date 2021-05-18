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
#ifndef BASE_SERIALIZATION_HPP_INCLUDED
#define BASE_SERIALIZATION_HPP_INCLUDED

#include <string>


//from casts.hpp
template <typename Destination_Type, typename Source_Type>
Destination_Type Cast_Pointer(Source_Type const * source)
{
	return Destination_Type(source);
}
template <typename Destination_Type, typename Source_Type>
Destination_Type Lossless_Cast(Source_Type const & source)
{
	return (Destination_Type) source; //TODO:
}






//this needs to be overridden for a specific stream type
template <typename S>
void Serialize(S & stream, char const * array, uinteger length)
{
	COMPILETIME_ASSERT(false, "Serializing through unsupported stream! have you specialized for this type? and if so, have you included the right header?");
}


template <typename S>
void Serialize_7Bit_Encoded_Uinteger(S & stream, uinteger value)
{
	//TODO: re-implement with a 10 bytes array and a single call to Serialize(char*)
	for (;;)
	{
		if (value < 0x80)
		{
			Serialize( stream, Lossless_Cast<char>(value) );
			return;
		}
		else
		{
			Serialize( stream, Lossless_Cast<char>(value & 0x7F | 0x80) );
			value = value >> 7;
		}
	}
}

//this can be overridden to use a custom encoding for array length, e.g. fixed length 32bits
template <typename S>
void Serialize_Array_Length(S & stream, uinteger value)
{
	Serialize( stream, Lossless_Cast<unsigned int>(value) );
	//Serialize_7Bit_Encoded_Uinteger(stream, value);
}

template <typename S, typename T>
void Serialize(S & stream, T const * array, uinteger length)
{
	//NOTE: this is not serializing the length, as that is expected to have been already serialized when first serializing the container
	for (uinteger i = 0; i < length; i++)
	{
		Serialize(stream, array[i]);
	}
}

template <typename S>
void Serialize(S & stream, char const * string)
{
	uinteger const length = std::char_traits<char>::length(string);
	Serialize_Array_Length(stream, length);

	Serialize(stream, Cast_Pointer<char const *>(string), length);
}

template <typename S>
void Serialize(S & stream, std::string const & string)
{
	std::string const & str = string;

	Serialize_7Bit_Encoded_Uinteger( stream, str.size() );

	Serialize(  stream, Cast_Pointer<char const *>(str.data() ), str.size()  );
}

template <typename S, typename T>
void Serialize(S & stream, T const & value)
{
	Serialize( stream, Cast_Pointer<char const *>(&value), sizeof(T) );
}



//a template specialization to control the type returned by the T Deserialize(Stream) form
template <typename T>	struct Serialization_Trait					{	typedef T Returnable_Type;				};
template <>				struct Serialization_Trait<char const *>	{	typedef std::string Returnable_Type;	};



//this needs to be overridden for a specific stream type
template <typename S>
void Deserialize(S & stream, char * array, uinteger length)
{
	COMPILETIME_ASSERT(false, "Serializing through unsupported stream! have you specialized for this type? and if so, have you included the right header?");
}

//TODO: this function should either go away or the Serialization_Trait should be generated as well (even thought raw strings are probably the only use case)
template <typename T, typename S>
typename Serialization_Trait<T>::Returnable_Type Deserialize(S & stream)
{
	Serialization_Trait<T>::Returnable_Type value;
	Deserialize(stream, value);
	return value;
}

template <typename T, typename S>
void Deserialize(S & stream, T & value)
{
	//TODO: this is risky. this would only works if the type is really a pod
	Deserialize( stream, Cast_Pointer<char *>(&value), sizeof(T) );
}


template <typename S>
void Deserialize(S & stream, std::string & value)
{
	uinteger length = Deserialize_7Bit_Encoded_Uinteger(stream);
	std::string& str = value;
	str.resize(length);
	Deserialize(  stream, Cast_Pointer<char *>( &str.at(0) ), length  );
}

template <typename S>
uinteger Deserialize_7Bit_Encoded_Uinteger(S & stream)
{
	uinteger value = 0;
	for (int i=0 ; ; i++)
	{
		char const block = Deserialize<char>(stream);
		value |= uinteger(block & 0x7F) << 7*i;
		if ( !(block & 0x80) ) return value;
	}
}

//this can be overridden to use a custom encoding for array length, e.g. fixed length 32bits
template <typename S>
uinteger Deserialize_Array_Length(S & stream)
{
	return Deserialize_7Bit_Encoded_Uinteger(stream);
}

template <typename S>
void Flush(S & stream)
{
}



#endif//BASE_SERIALIZATION_HPP_INCLUDED