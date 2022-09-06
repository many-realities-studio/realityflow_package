using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System.Collections.Generic;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    public class CreateAvatar_Received : ReceivedMessage
    {
        [JsonProperty("flowAvatar")]
        public FlowAvatar flowAvatar { get; set; }

        [JsonProperty("AvatarList")]
        public List<FlowAvatar> AvatarList { get; set; }

        public CreateAvatar_Received(FlowAvatar flowAvatar, bool wasSuccessful) : base(typeof(CreateAvatar_Received))
        {
            this.flowAvatar = flowAvatar;
            this.MessageType = "CreateAvatar";
        }
    }
}