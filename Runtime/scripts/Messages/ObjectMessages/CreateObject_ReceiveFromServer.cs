using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages
{
    [DataContract]
    public class CreateObject_ReceiveFromServer : ReceivedMessage
    {
        [DataMember]
        public FlowTObject flowObject { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void CreateObjectReceived_EventHandler(object sender, CreateObjectMessageEventArgs eventArgs);

        // The object that handles publishing/subscribing
        private static CreateObjectReceived_EventHandler _ReceivedEvent;
        public static event CreateObjectReceived_EventHandler ReceivedEvent
        {
            add
            {
                lock (_ReceivedEvent)
                {
                    _ReceivedEvent -= value;
                    _ReceivedEvent += value;
                }
            }
            remove
            {
                lock (_ReceivedEvent)
                {
                    _ReceivedEvent -= value;
                }
            }
        }

        /// <summary>
        /// Parse message and convert it to the appropriate data type
        /// </summary>
        /// <param name="message">The message to be parsed</param>
        public static void ReceiveMessage(string message)
        {
            CreateObject_ReceiveFromServer response = UnityEngine.JsonUtility.FromJson<CreateObject_ReceiveFromServer>(message);
            response.RaiseEvent();
        }

        /// <summary>
        /// Publish to the event to notify all subscribers to this message
        /// </summary>
        public override void RaiseEvent()
        {
            if (_ReceivedEvent != null)
            {
                _ReceivedEvent.Invoke(this, new CreateObjectMessageEventArgs(this));
            }
        }
    }

    public class CreateObjectMessageEventArgs : EventArgs
    {
        public CreateObject_ReceiveFromServer message { get; set; }

        public CreateObjectMessageEventArgs(CreateObject_ReceiveFromServer message)
        {
            this.message = message;
        }
    }
}
