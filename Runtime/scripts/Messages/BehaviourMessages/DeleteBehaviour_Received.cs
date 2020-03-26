using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Behaviours;
using UnityEngine;
using RealityFlow.Plugin.Scripts;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UIElements;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    public class DeleteBehaviour_Received : ConfirmationMessage_Received
    {

        [JsonProperty("BehaviourId")]
        public String BehaviourId { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void DeleteBehaviourReceived_EventHandler(object sender, DeleteBehaviourEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static DeleteBehaviourReceived_EventHandler _ReceivedEvent;
        public static event DeleteBehaviourReceived_EventHandler ReceivedEvent
        {
            add
            {
                if (_ReceivedEvent == null || !_ReceivedEvent.GetInvocationList().Contains(value))
                {
                    _ReceivedEvent += value;
                }
            }
            remove
            {
                _ReceivedEvent -= value;
            }
        }

        public DeleteBehaviour_Received(string message, bool wasSuccessful)
        {
            this.MessageType = "DeleteBehaviour";
            this.WasSuccessful = wasSuccessful;
        }

        /// <summary>
        /// Parse message and convert it to the appropriate data type
        /// </summary>
        /// <param name="message">The message to be parsed</param>
        public static void ReceiveMessage(string message)
        {
            var p2 = MessageSerializer.DesearializeObject<DeleteBehaviour_Received>(message);
            Debug.Log("Message received: " + p2.ToString());
            p2.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new DeleteBehaviourEventArgs(this));
            }
        }
    }

    public class DeleteBehaviourEventArgs : EventArgs
    {
        public DeleteBehaviour_Received message { get; set; }

        public DeleteBehaviourEventArgs(DeleteBehaviour_Received message)
        {
            this.message = message;
        }
    }
}
