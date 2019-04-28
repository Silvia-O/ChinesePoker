using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public interface IApplication
    {
        // void OnConnect(ClientPeer client);

        void OnDisconnect(ClientPeer client);

        void OnReceive(ClientPeer client, SocketMsg msg);
    }
}
