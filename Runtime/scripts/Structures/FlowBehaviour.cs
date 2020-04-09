using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using Behaviours;

namespace RealityFlow.Plugin.Scripts
{
    /// <summary>
    /// The purpose of this class is to hold all the information that is necessary to define a behaviour.
    /// The information from this class gets serialized and sent to the server.
    /// </summary>
    public class FlowBehaviour
    {
        [JsonProperty("TypeOfTrigger")]
        public string TypeOfTrigger { get; set; } // The trigger type

        [JsonProperty("Id")]
        public string Id { get; set; } // The ID of this behaviour

        [JsonProperty("TriggerObjectId")]
        public string TriggerObjectId { get; set; } // The first object ID - typically the trigger object

        [JsonProperty("TargetObjectId")]
        public string TargetObjectId { get; set; } // The second object ID - typically the target object

        [JsonProperty("NextBehaviour")]
        public List<string> NextBehaviour { get; set; } // The chain behaviour


        public string BehaviourName { get; set; } // the behaviour name

        public event Action<string, string, string, List<string>> EventCalled;

        [JsonProperty("Action")] 
        public FlowAction flowAction { get; set; }

        // public delegate void ParseMessage(string message); // Definition of a parse message method

        // public static Dictionary<string, ParseMessage> messageRouter = new Dictionary<string, ParseMessage>();


        [JsonConstructor] 
        public FlowBehaviour(string typeOfTrigger, string id, string triggerObjectId, string targetObjectId, List<string> nextBehaviour, FlowAction flowAction)
        {
            TypeOfTrigger = typeOfTrigger;
            Id = id;
            TriggerObjectId = triggerObjectId;
            TargetObjectId = targetObjectId;
            NextBehaviour = nextBehaviour;
            this.flowAction = flowAction;
            EventCalled += BehaviourEventManager.ListenToEvents;
            BehaviourName = typeOfTrigger;
            if (TypeOfTrigger.Equals("Immediate"))
            {
                BehaviourName = this.flowAction.ActionType;
            }
        }


        public FlowBehaviour(string typeOfTrigger, string id, string triggerObjectId, string targetObjectId, FlowAction flowAction)
        {
            TypeOfTrigger = typeOfTrigger;
            Id = id;
            TriggerObjectId = triggerObjectId;
            TargetObjectId = targetObjectId;
            NextBehaviour = new List<string>();
            this.flowAction = flowAction;
        }



        public void EventTrigger()
        {
            OnCallDown(BehaviourName);
        }


        /// <summary>
        /// Invokes event for this behaviour - subscribed to by BEM
        /// </summary>
        public void CallBehaviourEvent()
        {
            EventCalled.Invoke(BehaviourName, TriggerObjectId, TargetObjectId, NextBehaviour);
        }


        public string GetBehaviourName()
        {
            string flowBehaviourName = TypeOfTrigger;

            if (TypeOfTrigger.Equals("Immediate"))
            {
                flowBehaviourName = flowAction.ActionType;
            }

            return flowBehaviourName;
        }


        /// <summary>
        /// Determines logic based on scriptName to decide on what should be done on event call
        /// </summary>
        /// <param name="scriptName"></param>
        private void OnCallDown(string scriptName)
        {
            Debug.Log("here!!");

            GameObject triggerObject = BehaviourEventManager.GetGoFromGuid(TriggerObjectId);
            GameObject targetObject = BehaviourEventManager.GetGoFromGuid(TargetObjectId);

            CallBehaviourEvent();

            // figure out how to add specific code to each script here
            switch (scriptName)
            {
                case "Teleport":
                    // take in input from colliding object that determines teleport coordinates (teleport nodes)
                    Vector3 coords = targetObject.GetComponent<TeleportCoordinates>().GetCoordinates();
                    triggerObject.transform.position = coords;
                    // Set teleport rest for 5 seconds
                    return;
                case "Click":
                    // if object received click input, then trigger chained event
                    if (NextBehaviour.Count > 0)
                    {
                        foreach(string chainedBehaviourId in NextBehaviour)
                        {
                            if(BehaviourEventManager.BehaviourList.TryGetValue(chainedBehaviourId, out FlowBehaviour chainedBehaviour))
                            {
                                chainedBehaviour.EventTrigger();
                            }
                            
                            Debug.Log("execute chained events");
                        }
                    }
                    return;
                case "SnapZone":
                    // take in more info than teleport, but basically acts as a teleport within the other object
                    if (targetObject.GetComponent<TeleportCoordinates>().IsSnapZone)
                    {
                        triggerObject.transform.position = targetObject.transform.position + targetObject.GetComponent<TeleportCoordinates>().GetCoordinates();
                        triggerObject.transform.localScale = targetObject.GetComponent<TeleportCoordinates>().GetScale();
                        triggerObject.transform.rotation = targetObject.GetComponent<TeleportCoordinates>().GetRotation();
                        // set snap zone rest until leaves snap zone
                    }
                    return;
                case "Enable":
                    // enable second object and all related scripts
                    targetObject.SetActive(true);
                    return;
                case "Disable":
                    // disable second object and all related scripts
                    targetObject.SetActive(false);
                    return;
                default:
                    Debug.LogError("BehaviourEvent.OnCallDown returned no matching script name for " + scriptName);
                    return;

            }

        }
    }
}
