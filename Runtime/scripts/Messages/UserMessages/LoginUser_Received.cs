using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Messages.UserMessages
{
    /// <summary>
    /// Handle parsing and relaying message information
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
    /// </summary>
    public class LoginUser_Received : ConfirmationMessage_Received
    {
        /// <summary>
        /// in the format of (ProjectId, Name)
        /// </summary>
        [JsonProperty("Projects")]
        public List<FlowProject> Projects { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void LoginReceived_EventHandler(object sender, LoginUserMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static LoginReceived_EventHandler _ReceivedEvent;

        public LoginUser_Received(bool wasSuccessful)
        {
            this.MessageType = "LoginUser";
            this.WasSuccessful = wasSuccessful;
        }

        public static event LoginReceived_EventHandler ReceivedEvent
        {
            add
            {
               if(_ReceivedEvent == null || !_ReceivedEvent.GetInvocationList().Contains(value))
                {
                    _ReceivedEvent += value;
                }
            }
            remove
            {
                _ReceivedEvent -= value;
            }
        }

        /// <summary>
        /// Parse message and convert it to the appropriate data type
        /// </summary>
        /// <param name="message">The message to be parsed</param>
        public static void ReceiveMessage(string message)
        {
            var p2 = MessageSerializer.DesearializeObject<LoginUser_Received>(message);

            Debug.Log("Message received: " + p2.ToString());
            p2.RaiseEvent();
            //ConfirmationMessage_Received response = JsonUtility.FromJson<ConfirmationMessage_Received>(message);
            
            //response?.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new LoginUserMessageEventArgs(this));
            }
        }
    }

    /// <summary>
    /// The event arguments that get passed back to the callback functions as the second parameter
    /// </summary>
    public class LoginUserMessageEventArgs : EventArgs
    {
        public LoginUser_Received message { get; set; }

        public LoginUserMessageEventArgs(LoginUser_Received message)
        {
            this.message = message;
        }
    }
}
