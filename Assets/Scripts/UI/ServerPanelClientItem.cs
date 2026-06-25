using Sanicball.Logic;
using SanicballCore.Server;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class ServerPanelClientItem : MonoBehaviour
    {
        private ServClient client;
        public ServClient Client { 
            get => client;
            set
            {
                client = value;
                nameOutput.text = client.Name;
            }
        }
        public Text nameOutput;
        public void Kick()
        {
            if(MatchManager.instance.messenger is not OnlineMatchMessenger)
            {
                return;
            }
            if(MatchManager.instance.LocalClientGuid != Client.Guid)
            {
                NetworkManager.server.Kick(Client, "you got da boot lmao");
            }
        }
    }
}