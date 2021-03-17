using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using GraphProcessor; // TODO: Fix reference

namespace Packages.realityflow_package.Runtime.scripts.Messages.VSGraphMessages
{
    public class CreateVSGraph_Received : ReceivedMessage
    {
        [JsonProperty("flowVSGraph")]
        public BaseGraph flowVSGraph { get; set; }
        // public FlowTObject flowObject { get; set; }

        public CreateVSGraph_Received(BaseGraph flowVSGraph) : base(typeof(CreateVSGraph_Received))
        {
            this.flowVSGraph = flowVSGraph;
            this.MessageType = "CreateVSGraph";
        }
    }
}