using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using GraphProcessor;

namespace Packages.realityflow_package.Runtime.scripts.Messages.VSGraphMessages
{
    public class RunVSGraph_Received : ReceivedMessage
    {
        [JsonProperty("vsGraphId")]
        public string vsGraphId { get; set; }

        public RunVSGraph_Received(string vsGraphId) : base(typeof(RunVSGraph_Received))
        {
            this.vsGraphId = vsGraphId;
        }
    }
}