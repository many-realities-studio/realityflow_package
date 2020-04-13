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

        public JoinRoom_Received(FlowProject flowProject) : base(typeof(JoinRoom_Received))
        {
            this.flowProject = flowProject;
            this.MessageType = "JoinRoom";
        }
    }
}
