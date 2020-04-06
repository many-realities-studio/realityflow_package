using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;

namespace RealityFlow.Plugin.Scripts
{
    /// <summary>
    /// The purpose of this class is to hold all the information that is necessary to define a behaviour.
    /// The information from this class gets serialized and sent to the server.
    /// </summary>
    public class FlowBehaviour
    {
        [JsonProperty("TypeOfTrigger")]
        public string TypeOfTrigger { get; set; } // The behaviour type

        [JsonProperty("Id")]
        public string Id { get; set; } // The ID of this behaviour

        [JsonProperty("TriggerObjectId")]
        public string TriggerObjectId { get; set; } // The first object ID - typically the trigger object

        [JsonProperty("TargetObjectId")]
        public string TargetObjectId { get; set; } // The second object ID - typically the target object

        [JsonProperty("NextBehaviour")]
        public List<string> NextBehaviour { get; set; } // The chain behaviour

        [JsonProperty("FlowAction")]
        public FlowAction flowAction { get; set; }

        public FlowBehaviour(string typeOfTrigger, string id, string triggerObjectId, string targetObjectId, List<string> nextBehaviour, FlowAction flowAction)
        {
            TypeOfTrigger = typeOfTrigger;
            Id = id;
            TriggerObjectId = triggerObjectId;
            TargetObjectId = targetObjectId;
            NextBehaviour = nextBehaviour;
            this.flowAction = flowAction;
        }
    }
}
