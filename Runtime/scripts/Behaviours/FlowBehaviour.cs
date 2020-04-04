using Behaviours;
using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    public class FlowBehaviour
    {
        public string TypeOfTrigger;
        public Guid Id;
        public Guid TriggerObjectId;
        public Guid TargetObjectId;

        public List<Guid> NextBehaviorList;

        BaseFlowAction _FlowAction;

        public event Action<string, Guid, Guid, List<Guid>> EventCalled;

        public FlowBehaviour(string typeOfTrigger, Guid id, Guid triggerObjectId, Guid targetObjectId, List<Guid> nextBehavior, BaseFlowAction FlowAction)
        {
            TypeOfTrigger = typeOfTrigger;
            Id = id;
            TriggerObjectId = triggerObjectId;
            TargetObjectId = targetObjectId;
            NextBehaviorList = nextBehavior;
            _FlowAction = FlowAction;

            BehaviourEventManager.SendEventDown += OnCallDown;
            EventCalled += BehaviourEventManager.ListenToEvents;

        }

        public void EventTrigger()
        {
            OnCallDown(TypeOfTrigger);
        }

        /// <summary>
        /// Invokes event for this behaviour - subscribed to by BEM
        /// </summary>
        public void CallBehaviourEvent()
        {
            EventCalled.Invoke(TypeOfTrigger, TriggerObjectId, TargetObjectId, NextBehaviorList);
        }

        /// <summary>
        /// Determines logic based on scriptName to decide on what should be done on event call
        /// </summary>
        /// <param name="scriptName"></param>
        private void OnCallDown(string scriptName)
        {
            GameObject obj = BehaviourEventManager.GetGoFromGuid(TargetObjectId);

            CallBehaviourEvent();

            // figure out how to add specific code to each script here
            switch (scriptName)
            {
                case "Teleport":
                    //TODO reference FlowTObject dictionary to get reference to game object and update position from there
                    // take in input from colliding object that determines teleport coordinates (teleport nodes)
                    Vector3 coords = obj.GetComponent<TeleportCoordinates>().GetCoordinates();
                    //transform.position = coords;
                    // Set teleport rest for 5 seconds
                    return;
                case "Click":
                    // if object received click input, then trigger chained event
                    foreach (Guid behaviourGuid in NextBehaviorList)
                    {
                        BehaviourEventManager.GuidToBehaviourMapping[behaviourGuid].EventTrigger();
                        Debug.Log("execute chained event");
                    }
                    return;
                case "SnapZone":
                    // take in more info than teleport, but basically acts as a teleport within the other object
                    if (obj.GetComponent<TeleportCoordinates>().IsSnapZone())
                    {
                        FlowTObject currentGameObject = ;
                        currentGameObject.Position= obj.transform.position + obj.GetComponent<TeleportCoordinates>().GetCoordinates();
                        currentGameObject.Scale= obj.GetComponent<TeleportCoordinates>().GetScale();
                        currentGameObject.Rotation = obj.GetComponent<TeleportCoordinates>().GetRotation();
                        // set snap zone rest until leaves snap zone
                    }
                    return;
                case "Enable":
                    // enable second object and all related scripts
                    obj.SetActive(true);
                    return;
                case "Disable":
                    // disable second object and all related scripts
                    obj.SetActive(false);
                    return;
                default:
                    Debug.LogError("BehaviourEvent.OnCallDown returned no matching script name for " + scriptName);
                    return;

            }
        }
    }
}
