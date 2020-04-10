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
    public class FlowWebsocket
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

        public bool IsConnected { get; private set; }

        /// <summary>
        /// Link up the message parser and establish connection to the desired URL
        /// </summary>
        /// <param name="url"></param>
        public FlowWebsocket(string url, FlowUser flowUser)
        {
            messageParser = ReceivedMessageParser.Parse;
            Connect(url, flowUser.Username, flowUser.Password);
        }

        /// <summary>
        /// Connect to the desired server over a websocket connection
        /// </summary>
        /// <param name="url">URL address of server that should be connected to</param>
        public void Connect(string url, string username, string password)
        {
            Debug.Log("Establishing websocket connection to " + url);
            try
            {
                websocket = new WebSocketSharp.WebSocket(url);
                //websocket.OnMessage += (sender, e) => Debug.Log("Received message: " + e.Data.ToString());
                websocket.OnMessage += (sender, e) => ActionOnReceiveMessage(e.Data.ToString());
                websocket.SetCredentials(username, password, true);
                websocket.Connect();


                IsConnected = true;
            }
            catch (Exception e)
            {
                IsConnected = false;
                Debug.LogError("Failed to establish connection to: " + url + ", " + e);
            }


            if (websocket.ReadyState == WebSocketState.Open)
            {
                Debug.Log("Connection is open with: " + url); 
            }
            else
            {
                Debug.LogError("Failed to open connection with: " + url);
            }
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
            string sentMessage = MessageSerializer.SerializeMessage(message);

            try
            {
                Debug.Log("Sending message: " + sentMessage);

                websocket.Send(sentMessage);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send message: " + sentMessage + " " + e);
            }

            //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\" + typeof(T).ToString() + ".json", sentMessage);

            // Deserialization (for debbugging purposes)
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
            try
            {
                foreach (string message in ReceivedMessages)
                {
                    Debug.Log("Received message: " + message);
                    messageParser(message);
                    //GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            ReceivedMessages.RemoveAll((o) => true); // Remove everything from the list

            yield return null;
        }

        internal void Disconnect()
        {
            websocket.Close();
            websocket = null;
        }
    }
}
