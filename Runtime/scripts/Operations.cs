using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
//using RealityFlow.Plugin.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to provide a wrapper for the UnityPlugin,
    /// allowing for easy use with the networking tools
    /// </summary>
    public static class Operations
    {
        private static FlowWebsocket _flowWebsocket;
        public static FlowWebsocket FlowWebsocket { get => _flowWebsocket; }

        public static void Login(string username, string password, Login_Recieved.LoginReceived_EventHandler callbackFunction)
        {
            Login_SendToServer loginMessage = new Login_SendToServer(username, password);
            FlowWebsocket.SendMessage(loginMessage);

            Login_Recieved.ReceivedEvent += callbackFunction;
        }

        //public static void exampleCallbackFunction(object sender, ConfirmationMessageEventArgs eventArgs)
        //{
        //    if(eventArgs.message.WasSuccessful)
        //    {
        //        // Login Succeeded! :D
        //    }
        //    else
        //    {
        //        // Login Failed :(
        //    }
        //}

        //public static void exampleLinkingFunction()
        //{
        //    Login("username", "password", exampleCallbackFunction);
        //}
        
        public static void Logout()
        {
            Logout_SendToServer logoutMessage = new Logout_SendToServer();

            //flowWebsocket.SendMessage(logoutMessage);
        }

        public static void ConnectToServer(string url)
        {
            _flowWebsocket = new FlowWebsocket(url);
        }
    }
}
