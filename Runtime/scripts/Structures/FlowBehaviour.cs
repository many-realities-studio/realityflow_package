using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace RealityFlow.Plugin.Scripts
{
    /// <summary>
    /// The purpose of this class is to hold all the information that is necessary to define a behaviour.
    /// The information from this class gets serialized and sent to the server.
    /// </summary>
    public class FlowBehaviour
    {
        [JsonProperty("Name")]
        public string Name { get; set; } // The unique ID of the project

        [JsonProperty("Id")]
        public string Id { get; set; } // Description of the project

        [JsonProperty("TriggerObjectID")]
        public string FirstObject { get; set; } // The last time this project was modified

        [JsonProperty("TargetObjectID")]
        public string SecondObject { get; set; } // Name of the project

        [JsonProperty("BehaviourChain")]
        public FlowBehaviour BehaviourChain { get; set; } // Name of the project


        public FlowBehaviour(string name, string id, string firstObject, string secondObject, FlowBehaviour chain)
        {
            Name = name;
            Id = id;
            FirstObject = firstObject;
            SecondObject = secondObject;
            BehaviourChain = chain;
        }
  
    }
}
