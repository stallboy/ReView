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
#ifndef BASE_INTEROP_HPP_INCLUDED
#define BASE_INTEROP_HPP_INCLUDED

#include "linear_serialization.hpp"
#include <vector>
#include <unordered_map>

struct Matrix3D
{
};

struct Vector3D
{
};

struct Color
{
};

//common base for all RPC stubs
template <typename T>
class RPC_Client_Proxy
{
protected:
	RPC_Client_Proxy(T& stream) : stream(stream){}

	void Begin()
	{
		if (prefix_buffer.size() > 0)
		{
			Serialize( stream, prefix_buffer.data(), prefix_buffer.size() );
		}
	}
	void Flip() { Flush(stream); }
	void End() { }

public:
	template<typename T>
	void Set_prefix(T const & prefix)
	{
		prefix_buffer.resize( sizeof(T) );
		memcpy( prefix_buffer.data(), &prefix, sizeof(T) );
	}


	protected:
	T& stream;
	std::vector<unsigned char> prefix_buffer;
};

template <typename S, typename T>
class RPC_Server_Proxy : public RPC_Client_Proxy<S>
{
protected:
	RPC_Server_Proxy(S& stream, T* obj) : RPC_Client_Proxy(stream), obj(obj){}

protected:
	void On_Invalid_Method_Index(unsigned char method_index)
	{
		UNREFERENCED_PARAM(method_index);
		FATAL_ERROR("unexpected method index");
	}

	void Receive_Call() =0;

protected:
	void Begin()
	{
		//TODO: assert we can receive the expected prefix
	}

	void Flip()
	{
		//nothing to be done here
	}

	void End()
	{
		Flush(stream);
	}

	protected:
	T* const obj;
};

class RPC_Server_Proxy_Base
{
public:
	virtual void Receive_Call() =0;
};

//a class that can be used to coordinate multiple proxies sharing the same channel
template<typename S>
class RPC_Server_Proxies_Multiplexer
{
public:
	//RPC_Server_Proxies_Multiplexer(S& stream){} : stream(stream){}
	
	void Add_Proxy(unsigned char channel_id, RPC_Server_Proxy_Base * proxy)
	{
		proxies[channel_id] = proxy;
	}

	void Receive_Calls()
	{
		while (true)
		{
			Receive_Call();
		}
	}

protected:
	void Receive_Call()
	{
		auto const channel_id = Deserialize<byte>(stream);

		proxies[channel_id].Receive_Call();
	}
	
protected:
	std::unordered_map<unsigned char, RPC_Server_Proxy_Base*> proxies;
	S& stream;
};

#endif//BASE_INTEROP_HPP_INCLUDED