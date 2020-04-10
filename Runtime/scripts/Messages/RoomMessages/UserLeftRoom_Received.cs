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
    public class UserLeftRoom_Received : ReceivedMessage
    {
        [JsonProperty("Message")]
        public string leftRoomMessage { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void UserLeftRoomReceived_EventHandler(object sender, UserLeftRoomMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static UserLeftRoomReceived_EventHandler _ReceivedEvent;

        public UserLeftRoom_Received(string Message)
        {
            this.leftRoomMessage = Message;
            this.MessageType = "UserLeftRoom";

        }

        public static event UserLeftRoomReceived_EventHandler ReceivedEvent
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
            UserLeftRoom_Received response = MessageSerializer.DesearializeObject<UserLeftRoom_Received>(message);
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
                _ReceivedEvent.Invoke(this, new UserLeftRoomMessageEventArgs(this));
            }
        }
    }

    public class UserLeftRoomMessageEventArgs : EventArgs
    {
        public UserLeftRoom_Received message { get; set; }

        public UserLeftRoomMessageEventArgs(UserLeftRoom_Received message)
        {
            this.message = message;
        }
    }
}
