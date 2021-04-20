using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using GraphProcessor;

namespace Packages.realityflow_package.Runtime.scripts.Messages.VSGraphMessages
{
    public class CreateVSGraph_Received : ReceivedMessage
    {
        [JsonProperty("flowVSGraph")]
        public FlowVSGraph flowVSGraph { get; set; }

        public CreateVSGraph_Received(FlowVSGraph flowVSGraph) : base(typeof(CreateVSGraph_Received))
        {
            this.flowVSGraph = flowVSGraph;
            this.MessageType = "CreateVSGraph";
        }
    }
}