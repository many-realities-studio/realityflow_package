using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages
{
    /// <summary>
    /// Leave project received message format 
    /// </summary>
    public class LeaveProject_Received : ConfirmationMessage_Received
    {
        // Definition of event type (What gets sent to the subscribers
        public delegate void LeaveProjectReceived_EventHandler(object sender, LeaveProjectMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static LeaveProjectReceived_EventHandler _ReceivedEvent;

        public LeaveProject_Received()
        {
            this.MessageType = "LeaveProject";
        }

        public static event LeaveProjectReceived_EventHandler ReceivedEvent
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

        /// <summary>
        /// Parse message and convert it to the appropriate data type
        /// </summary>
        /// <param name="message">The message to be parsed</param>
        public static void ReceiveMessage(string message)
        {
            LeaveProject_Received response = MessageSerializer.DesearializeObject<LeaveProject_Received>(message);
            response.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new LeaveProjectMessageEventArgs(this));
            }
        }
    }

    public class LeaveProjectMessageEventArgs : EventArgs
    {
        public LeaveProject_Received message { get; set; }

        public LeaveProjectMessageEventArgs(LeaveProject_Received message)
        {
            this.message = message;
        }
    }
}
