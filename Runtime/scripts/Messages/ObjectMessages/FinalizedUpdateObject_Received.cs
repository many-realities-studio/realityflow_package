using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages
{
    public class FinalizedUpdateObject_Received : ReceivedMessage
    {
        [JsonProperty("flowObject")]
        public FlowTObject flowObject { get; set; }
        
        // Definition of event type (What gets sent to the subscribers
        public delegate void FinalizedUpdateObjectRecieved_EventHandler(object sender, FinalizedUpdateObjectMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static FinalizedUpdateObjectRecieved_EventHandler _ReceivedEvent;

        public FinalizedUpdateObject_Received(FlowTObject flowObject)
        {
            this.flowObject = flowObject;
        }

        public static event FinalizedUpdateObjectRecieved_EventHandler ReceivedEvent
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
            FinalizedUpdateObject_Received response = MessageSerializer.DesearializeObject<FinalizedUpdateObject_Received>(message);
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
                _ReceivedEvent.Invoke(this, new FinalizedUpdateObjectMessageEventArgs(this));
            }
        }
    }

    public class FinalizedUpdateObjectMessageEventArgs : EventArgs
    {
        public FinalizedUpdateObject_Received message { get; set; }

        public FinalizedUpdateObjectMessageEventArgs(FinalizedUpdateObject_Received message)
        {
            this.message = message;
        }
    }
}
