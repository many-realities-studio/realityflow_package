﻿using Newtonsoft.Json;
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
        FlowBehaviour FlowBehaviour { get; set; }

        [JsonProperty("ProjectId")]
        string ProjectId { get; set; }


        public CreateBehaviour_SendToServer(FlowBehaviour behaviour, string projectId)
        {
            this.FlowBehaviour = behaviour;
            this.ProjectId = projectId;

            this.MessageType = "CreateBehaviour";
        }
    }

    /// <summary>
    /// Delete project request message format 
    /// Receive: <see cref="DeleteBehaviour_Received"/>
    /// </summary>
    public class DeleteBehaviour_SendToServer : BaseMessage
    {
        [JsonProperty("ProjectId")]
        string ProjectId { get; set; }

        [JsonProperty("BehaviourId")]
        string BehaviourId { get; set; }

        [JsonProperty("FlowBehaviour")]
        FlowBehaviour FlowBehaviour { get; set; }


        public DeleteBehaviour_SendToServer(FlowBehaviour flowBehaviour, string behaviourId, string projectId)
        {
            this.ProjectId = projectId;
            this.BehaviourId = behaviourId;
            this.FlowBehaviour = flowBehaviour;
        }
    }



    /// <summary>
    /// Update behaviour request message format 
    /// Receive: <see cref="UpdateBehaviour_Received"/>
    /// </summary>
    public class UpdateBehaviour_SendToServer : BaseMessage
    {
        [JsonProperty("FlowBehaviour")]
        FlowBehaviour FlowBehaviour { get; set; }

        [JsonProperty("ProjectId")]
        string ProjectId { get; set; }


        public UpdateBehaviour_SendToServer(FlowBehaviour behaviour, string projectId)
        {
            this.FlowBehaviour = behaviour;
            this.ProjectId = projectId;

            this.MessageType = "UpdateBehaviour";
        }
    }
}
