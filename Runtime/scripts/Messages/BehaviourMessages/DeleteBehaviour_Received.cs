using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Behaviours;
using UnityEngine;
using RealityFlow.Plugin.Scripts;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UIElements;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    public class DeleteBehaviour_Received : ConfirmationMessage_Received
    {
        [JsonProperty("BehaviourId")]
        public List<string> BehaviourIds { get; set; }

        public DeleteBehaviour_Received(string message, bool wasSuccessful) : base(wasSuccessful)
        {
            this.MessageType = "DeleteBehaviour";
            this.WasSuccessful = wasSuccessful;
        }
    }
}
