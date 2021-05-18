typedef unsigned long long uinteger;

#include "ReView.interop.hpp"
#include <windows.h>
#include <iostream>
#include <tchar.h>

#pragma comment(lib,"Ws2_32.lib")


class Socket
{
public:
	SOCKET handle;

	unsigned int Resolve_Hostname(const char* name)
	{
		auto host_info = gethostbyname(name);
 
		return *reinterpret_cast<unsigned int*>(host_info->h_addr_list[0]);
	}

	Socket(const char* name, unsigned short port)
	{
		WSADATA wsaData = {0};
		auto const result = WSAStartup(MAKEWORD(2, 2), &wsaData);
		handle = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		const BOOL tcpNoDelay = FALSE;
		setsockopt( handle, IPPROTO_TCP, TCP_NODELAY, Cast_Pointer<const char*>(&tcpNoDelay), sizeof(tcpNoDelay) );
		sockaddr_in inAddress; 
		inAddress.sin_family = AF_INET;
		unsigned int addr_number = Resolve_Hostname(name);
		inAddress.sin_addr.s_addr = addr_number;
		inAddress.sin_port = htons(port);
		auto const res = connect( handle, Cast_Pointer<const SOCKADDR*>(&inAddress), sizeof(inAddress) );
	}

public:
	void Send(char const * array, unsigned int length)
	{
		send(handle, (char const*) array, length, 0);
	}

	void Receive(char * array, unsigned int length)
	{
		recv(handle, (char*) array, length, 0);
	}
};


template <>
void Serialize(Socket& stream, char const * array, uinteger length)
{
	stream.Send( array, (unsigned int)length );
}


template <>
void Deserialize(Socket& stream, char * array, uinteger length)
{
	stream.Receive( array, (unsigned int)length );
}

int _tmain(int argc, _TCHAR* argv[])
{
	Socket socket("localhost", 5000);
	RPC_Client_Proxy_IReView_HierarchicalTimelineLog<Socket> client(socket);
	printf("a");
	
	client.AddTrack(-1, 0, "Foo");
	client.AddItem(0, 1, 0, "Test");
	client.AppendLog(1, 0, 1000, "Log");
	printf("b");
	std::cin.ignore(1);
}