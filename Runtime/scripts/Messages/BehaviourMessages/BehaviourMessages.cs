using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Behaviours;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    /// <summary>
    /// Create behaviour request message format 
    /// Receive: <see cref="CreateBehaviour_Received"/>
    /// </summary>
    public class CreateBehaviour_SendToServer : BaseMessage
    {
        [JsonProperty("FlowBehaviour")]
        FlowBehaviour flowBehaviour { get; set; }

        [JsonProperty("FlowUser")]
        FlowUser flowUser { get; set; }

        [JsonProperty("ProjectId")]
        string projectId { get; set; }

        [JsonProperty("ObjectId")]
        string objectId { get; set; }

        public CreateBehaviour_SendToServer(FlowBehaviour behaviour, FlowUser flowUser, string projectId, string objectId)
        {
            this.flowBehaviour = behaviour;
            this.flowUser = flowUser;
            this.projectId = projectId;
            this.objectId = objectId;

            this.MessageType = "CreateBehaviour";
        }
    }

    /// <summary>
    /// Delete project request message format 
    /// Receive: <see cref="DeleteProject_Received"/>
    /// </summary>
    public class DeleteBehaviour_SendToServer : BaseMessage
    {
        [JsonProperty("FlowProject")]
        FlowProject flowProject { get; set; }

        [JsonProperty("FlowUser")]
        FlowUser flowUser { get; set; }

        public DeleteBehaviour_SendToServer(FlowProject flowProject, FlowUser flowUser)
        {
            this.flowProject = flowProject;
            this.flowUser = flowUser;
        }
    }

    /// <summary>
    /// Open Behaviour request message format 
    /// <see cref="OpenBehaviour_Received"/>
    /// </summary>
    public class OpenBehaviour_SendToServer : BaseMessage
    {
        [JsonProperty("")]
        string projectId { get; set; }

        [JsonProperty("FlowUser")]
        FlowUser flowUser { get; set; }

        public OpenBehaviour_SendToServer(string projectId, FlowUser flowUser)
        {
            this.projectId = projectId;
            this.flowUser = flowUser;
        }
    }

  
}
