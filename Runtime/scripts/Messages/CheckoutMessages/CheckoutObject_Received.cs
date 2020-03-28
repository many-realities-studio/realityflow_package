using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages
{
    public class CheckoutObject_Received : ConfirmationMessage_Received
    {
        [JsonProperty("ObjectID")]
        public string ObjectID;

        public CheckoutObject_Received(string objectID)
        {
            ObjectID = objectID;
            this.MessageType = "CheckoutObject";
        }

        // Definition of event type (What gets sent to the subscribers)
        public delegate void CheckoutObjectReceived_EventHandler(object sender, CheckoutObjectEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static CheckoutObjectReceived_EventHandler _ReceivedEvent;

        public static event CheckoutObjectReceived_EventHandler ReceivedEvent
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
            CheckoutObject_Received response = MessageSerializer.DesearializeObject<CheckoutObject_Received>(message);
            response.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new CheckoutObjectEventArgs(this));
            }
        }
    }

    public class CheckoutObjectEventArgs : EventArgs
    {
        public CheckoutObject_Received message;

        public CheckoutObjectEventArgs(CheckoutObject_Received message)
        {
            this.message = message;
        }
    }
}
