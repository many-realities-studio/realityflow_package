using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages
{
    public class CreateProject_Received : ConfirmationMessage_Received
    {

        [JsonProperty("FlowProject")]
        public FlowProject flowProject { get; set; }

        // Definition of event type (What gets sent to the subscribers
        public delegate void CreateProjectReceived_EventHandler(object sender, CreateProjectMessageArgs eventArgs);

        // The object that handles publishing/subscribing
        private static CreateProjectReceived_EventHandler _ReceivedEvent;

        public CreateProject_Received(FlowProject flowProject, bool wasSuccessful)
        {
            this.flowProject = flowProject;
            this.MessageType = "CreateProject";
            this.WasSuccessful = wasSuccessful;
        }

        public static event CreateProjectReceived_EventHandler ReceivedEvent
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
            ConfirmationMessage_Received response = MessageSerializer.DesearializeObject<CreateProject_Received>(message);
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
                _ReceivedEvent.Invoke(this, new CreateProjectMessageArgs(this));
            }
        }
    }

    public class CreateProjectMessageArgs : EventArgs
    {
        public CreateProject_Received message;

        public CreateProjectMessageArgs(CreateProject_Received message)
        {
            this.message = message;
        }
    }
}
