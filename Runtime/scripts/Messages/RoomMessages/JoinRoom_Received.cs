using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages
{
    public class JoinRoom_Received : ReceivedMessage
    {
        [JsonProperty("flowProject")]
        public FlowProject flowProject { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void JoinRoomReceived_EventHandler(object sender, JoinRoomMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static JoinRoomReceived_EventHandler _ReceivedEvent;

        public JoinRoom_Received(FlowProject flowProject)
        {
            this.flowProject = flowProject;
            this.MessageType = "JoinRoom";
        }

        public static event JoinRoomReceived_EventHandler ReceivedEvent
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
            JoinRoom_Received response = MessageSerializer.DesearializeObject<JoinRoom_Received>(message);
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
                _ReceivedEvent.Invoke(this, new JoinRoomMessageEventArgs(this));
            }
        }
    }

    public class JoinRoomMessageEventArgs : EventArgs
    {
        public JoinRoom_Received message { get; set; }

        public JoinRoomMessageEventArgs(JoinRoom_Received message)
        {
            this.message = message;
        }
    }
}
