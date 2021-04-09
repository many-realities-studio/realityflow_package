using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    public class UpdateAvatar_Received : ReceivedMessage
    {
        [JsonProperty("flowAvatar")]
        public FlowAvatar flowAvatar { get; set; }

        public UpdateAvatar_Received(FlowAvatar flowAvatar) : base(typeof(UpdateAvatar_Received))
        {
            this.flowAvatar = flowAvatar;
        }
    }
}