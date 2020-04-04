using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages
{
    public class CheckinObject_Received : ConfirmationMessage_Received
    {
        [JsonProperty("ObjectID")]
        public string ObjectID;

        public delegate void CheckinObjectReceived_EventHandler(object sender, CheckinObjectEventArgs eventArgs);

        public CheckinObject_Received(string objectID)
        {
            ObjectID = objectID;
            this.MessageType = "CheckinObject";
        }

        // The object that handles publishing/subscribing
        private static CheckinObjectReceived_EventHandler _ReceivedEvent;

        public static event CheckinObjectReceived_EventHandler ReceivedEvent
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
            CheckinObject_Received response = MessageSerializer.DesearializeObject<CheckinObject_Received>(message);
            response.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new CheckinObjectEventArgs(this));
            }
        }
    }

    public class CheckinObjectEventArgs
    {
        public CheckinObject_Received message;

        public CheckinObjectEventArgs(CheckinObject_Received message)
        {
            this.message = message;
        }
    }
}
