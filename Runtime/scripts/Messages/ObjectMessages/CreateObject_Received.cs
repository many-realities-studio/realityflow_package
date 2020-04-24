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
    public class CreateObject_Received : ReceivedMessage
    {
        [JsonProperty("flowObject")]
        public FlowTObject flowObject { get; set; }

        public CreateObject_Received(FlowTObject flowObject) : base(typeof(CreateObject_Received))
        {
            this.flowObject = flowObject;
            this.MessageType = "CreateObject";
        }
    }
}
