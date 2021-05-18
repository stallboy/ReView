typedef unsigned long long uinteger;

#include "ReView.interop.hpp"
#include <windows.h>


#pragma comment(lib,"Ws2_32.lib")


class Socket
{
public:
	SOCKET handle;

	unsigned int Resolve_Hostname(const char* name)
	{
		auto host_info = gethostbyname(name);
 
		return (unsigned int)*(host_info->h_addr_list[0]);
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
		inAddress.sin_addr.s_addr = Resolve_Hostname(name);
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


void main()
{
	Socket socket("localhost", 5000);
	RPC_Client_Proxy_IReView_HierarchicalTimelineLog<Socket> client(socket);
	client.AddGenericItem(0, 0, 0, "foo");
}