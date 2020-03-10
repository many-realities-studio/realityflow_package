using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to provide a wrapper for websockets that
    /// allows for the use of websockets with Unity. The reason this is necessary is because 
    /// Unity cannot be interacted with from within a event (the event that gets fired when
    /// a message is received over a websocket connection)
    /// </summary>
    [ExecuteAlways]
    public class FlowWebsocket : MonoBehaviour
    {
        /// <summary>
        /// A message parser is the part of the program that interprets and brings
        /// actions to the messages being received
        /// </summary>
        /// <param name="message"></param>
        public delegate void MessageParser(string message);
        public MessageParser messageParser;

        public static List<String> ReceivedMessages = new List<string>();
        public static WebSocketSharp.WebSocket websocket;

        /// <summary>
        /// Link up the message parser and establish connection to the desired URL
        /// </summary>
        /// <param name="url"></param>
        public FlowWebsocket(string url)
        {
            messageParser = ReceivedMessageParser.Parse;
            Connect(url);
        }

        /// <summary>
        /// Connect to the desired server over a websocket connection
        /// </summary>
        /// <param name="url">URL address of server that should be connected to</param>
        public void Connect(string url)
        {
            //string url = "ws://echo.websocket.org";
            Debug.Log("Establishing websocket connection to " + url);
            websocket = new WebSocketSharp.WebSocket(url);
            //websocket.OnMessage += (sender, e) => Debug.Log("Received message: " + e.Data.ToString());
            websocket.OnMessage += (sender, e) => ActionOnReceiveMessage(e.Data.ToString());
            websocket.Connect();

            Debug.Log("Connection established with " + url);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">Message to send</param>
        public new void SendMessage(string message)
        {
            Debug.Log("Sending message: " + message);
            websocket.Send(message);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">Type of message that should be sent (Needs to inherit from base message)</typeparam>
        /// <param name="message">The message object that should be sent</param>
        public void SendMessage<T>(T message) where T : BaseMessage
        {
            MemoryStream memoryStream = MessageSerializer.SerializeMessage<T>(message);
            //websocket.Send(memoryStream, (int)memoryStream.Length);
            //string messageToSend = JsonUtility.ToJson(message);

            memoryStream.Position = 0;
            var sr = new StreamReader(memoryStream);
            string sentMessage = sr.ReadToEnd();
            Debug.Log("Sending message: " + sentMessage);

            websocket.Send(sentMessage);

            //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\" + typeof(T).ToString() + ".json", sentMessage);

            // Deserialization
            //CreateObject_SendToServer response = MessageSerializer.DesearializeObject<CreateObject_SendToServer>(sentMessage);

            //Debug.Log(response.ToString());
        }

        //public void SendMessage(FlowEvent flowEvent)
        //{
        //    string messageToSend = JsonUtility.ToJson(flowEvent);

        //    Debug.Log("Sending message: " + messageToSend);
        //    websocket.Send(messageToSend);
        //}
        
        /// <summary>
        /// This is the callback function for when a message is received over a websocket connection
        /// </summary>
        /// <param name="message"></param>
        private void ActionOnReceiveMessage(string message)
        {
            Debug.Log("Event handler received message: " + message);
            ReceivedMessages.Add(message);
        }

        /// <summary>
        /// Iterates through the received messages and performs the desired action.
        /// This function is utilized by EditorCoroutines (A necessary package to run this project)
        /// This function gets called once per frame (On update)
        /// </summary>
        /// <returns></returns>
        public IEnumerator ReceiveMessage()
        {
            foreach (string message in ReceivedMessages)
            {
                Debug.Log("Received message: " + message);
                messageParser(message);
                //GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            ReceivedMessages.RemoveAll((o) => true); // Remove everything from the list

            yield return null;
        }
    }
}
