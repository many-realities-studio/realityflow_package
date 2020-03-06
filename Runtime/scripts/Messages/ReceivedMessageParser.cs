using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
//using RealityFlow.Plugin.Scripts.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to parse messages that are received from the server
    /// into their respective representations in C#
    /// </summary>
    public class ReceivedMessageParser
    {
        public static Dictionary<string, BaseMessage.ParseMessage> messageRouter = new Dictionary<string, BaseMessage.ParseMessage>();

        /// <summary>
        /// Links all message types to their respective function. Note that this can be improved 
        /// such that each message object adds themself to the dictionary
        /// </summary>
        static ReceivedMessageParser()
        {            
            // Object Messages
            messageRouter.Add("CreateObject", CreateObject_Received.ReceiveMessage);
            messageRouter.Add("DeleteObject", DeleteObject_Received.ReceiveMessage);
            messageRouter.Add("UpdateObject", UpdateObject_Received.ReceiveMessage);
            messageRouter.Add("FinalizedUpdateObject", FinalizedUpdateObject_Received.ReceiveMessage);

            // Project Messages
            messageRouter.Add("CreateProject", CreateProject_Received.ReceiveMessage);
            messageRouter.Add("DeleteProject", DeleteProject_Received.ReceiveMessage);
            messageRouter.Add("GetAllUserProject", GetAllUserProjects_Received.ReceiveMessage);
            messageRouter.Add("OpenProject", OpenProject_Received.ReceiveMessage);

            // Room Messages
            messageRouter.Add("JoinRoom", JoinRoom_Received.ReceiveMessage);

            // User Messages
            messageRouter.Add("Login", LoginUser_Received.ReceiveMessage);
            messageRouter.Add("Logout", LogoutUser_Received.ReceiveMessage);
            messageRouter.Add("RegisterUser", RegisterUser_Received.ReceiveMessage);
        }

        /// <summary>
        /// Request to parse a message from a server into its respective c# representation
        /// </summary>
        /// <param name="message">The message from the server to parse</param>
        public static void Parse(string message)
        {
            string messageType = GetMessageType(message);
            messageRouter[messageType](message); // Perform the action associated with the message
        }

        /// <summary>
        /// Retrieve the message type from a message.
        /// </summary>
        /// <param name="messageToConvert">The message that should be converted into its C# representation</param>
        /// <returns></returns>
        public static string GetMessageType(string messageToConvert)
        {
            var p2 = MessageSerializer.DesearializeObject<BaseMessage>(messageToConvert);

            Debug.Log("Getting message type: " + p2.MessageType);

            return p2.MessageType;
        }
    }
}
