﻿using Newtonsoft.Json;
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
        public string Name { get; set; } // The behaviour type

        [JsonProperty("Id")]
        public string Id { get; set; } // The ID of this behaviour

        [JsonProperty("Trigger")]
        public string FirstObject { get; set; } // The first object ID - typically the trigger object

        [JsonProperty("Target")]
        public string SecondObject { get; set; } // The second object ID - typically the target object

        [JsonProperty("FlowBehaviour")]
        public FlowBehaviour BehaviourChain { get; set; } // The chain behaviour

        [JsonProperty("ChainOwner")]
        public string ChainOwner { get; set; } // The object that owns this behaviour


        public FlowBehaviour(string name, string id, string firstObject, string secondObject, FlowBehaviour behaviourChain, string chainOwner)
        {
            Name = name;
            Id = id;
            FirstObject = firstObject;
            SecondObject = secondObject;
            BehaviourChain = behaviourChain;
            ChainOwner = chainOwner;
        }
    }
}
