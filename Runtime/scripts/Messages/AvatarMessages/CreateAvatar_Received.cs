using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    public class CreateAvatar_Received : ReceivedMessage
    {
        [JsonProperty("flowObject")]
        public FlowTObject flowObject { get; set; }

        public CreateAvatar_Received(FlowTObject flowObject) : base(typeof(CreateAvatar_Received))
        {
            this.flowObject = flowObject;
            this.MessageType = "CreateAvatar";
        }
    }
}