using Microsoft.MixedReality.Toolkit.UI;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public static class BehaviourEventManager
    {
        public static event Action<string> SendEventDown;
        public static Dictionary<string, GameObject> GoIds = null;
        public static States DefaultInteractableStates;
        public static Dictionary<string, FlowBehaviour> BehaviourList;
        public static string PreviousBehaviourId = null;

        public static void Initialize()
        {
            GoIds = new Dictionary<string, GameObject>();
            BehaviourList = new Dictionary<string, FlowBehaviour>();
            //DefaultInteractableStates = ScriptableObject.CreateInstance<States>();
        }

        public static void Clear()
        {
            GoIds = null;
            BehaviourList = null;
        }


        /// <summary>
        /// Adds the FlowBehaviour to the BehaviourList and makes each object Interactable
        /// </summary>
        /// <param name="flowBehaviour"></param>
        public static void CreateNewBehaviour(FlowBehaviour flowBehaviour)
        {

            // Add the behaviour to the list of behaviours 
            BehaviourEventManager.BehaviourList.Add(flowBehaviour.Id, flowBehaviour);
            
            // Make both objects interactable
            ObjectIsInteractable oIsIFirst = FindAndMakeInteractable(flowBehaviour.TriggerObjectId);
            ObjectIsInteractable oIsISecond = FindAndMakeInteractable(flowBehaviour.TargetObjectId);


            if (oIsIFirst == null || oIsISecond == null)
            {
                Debug.LogWarning("There is a missing gameobject. Failed to make Interaction.");
                return;
            }
        }


        /// <summary>
        /// Finds the gameobject associated with objectId, and adds an ObjectIsInteractable component to it.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static ObjectIsInteractable FindAndMakeInteractable(string objectId)
        {
            if (FlowTObject.idToGameObjectMapping.TryGetValue(objectId, out FlowTObject foundObject))
            {
                Debug.Log("Found object " + foundObject.Id + " ... making interactable");
                ObjectIsInteractable oIsI = MakeObjectInteractable(foundObject.AttachedGameObject, objectId);
                return oIsI;
            }

            else
            {
                Debug.Log("Cannot make object interactable. Object not found in project.");
                return null;
            }
        }

        /// <summary>
        /// Adds the childFlowBehaviourId into the parent's NextBehaviour list
        /// </summary>
        /// <param name="childFlowBehaviourId"></param>
        /// <param name="parentFlowBehaviourId"></param>
        public static void LinkBehaviours(string childFlowBehaviourId, string parentFlowBehaviourId)
        {
            if(BehaviourList.TryGetValue(parentFlowBehaviourId, out FlowBehaviour parentFlowBehaviour))
            {
                parentFlowBehaviour.NextBehaviour.Add(childFlowBehaviourId);
            }

            else
            {
                Debug.LogWarning("Unable to find ParentBehaviour.");
            }
        }



        public static void UpdateBehaviour(FlowBehaviour flowBehaviour)
        {

        }





        public static BehaviourEvent CreateNewBehaviourEvent(string name, string id, string go1, string go2, BehaviourEvent chain)
        {
            GameObject g1 = GetGoFromGuid(go1);
            if (g1)
            {
                BehaviourEvent bEvent = g1.AddComponent<BehaviourEvent>();
                if (bEvent)
                {
                    bEvent.SetName(name);
                    bEvent.SetFirstObject(go1);
                    bEvent.SetSecondObject(go2);
                   // bEvent.SetChain(chain);
                    bEvent.Id = id;
                }

                g1.GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);
                GetGoFromGuid(go2).GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);

                return bEvent;
            }
            return null;
        }

        public static  BehaviourEvent AddChain(BehaviourEvent b1, BehaviourEvent b2)
        {
            BehaviourEvent temp = b1;

            while (temp.chainedEvent != null)
            {
                temp = temp.chainedEvent;
            }

            temp.SetChain(b2);
            return b1;
        }

        // needs to check second gameobject
        public static void DeleteBehaviourEvent(string  go1, string go2, BehaviourEvent bEvent)
        {
            GameObject g1 = GetGoFromGuid(go1);
            ObjectIsInteractable interactScript = g1.GetComponent<ObjectIsInteractable>();

            if (interactScript == null)
                return;

            interactScript.RemoveInteractableEvent(bEvent, go2);
        }


        /// <summary>
        /// Registers when a BehaviourEvent has been called - sends up to server communicator
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="go1"></param>
        /// <param name="go2"></param>
        /// <param name="chain"></param>
        public static void ListenToEvents(string eventName, string go1, string go2, BehaviourEvent chain)
        {
            // sends updated coordinates
        }

        public static ObjectIsInteractable MakeObjectInteractable(GameObject go, string objectId)
        {
            ObjectIsInteractable oisI = go.GetComponent<ObjectIsInteractable>();
    
            if(oisI == null)
            {
                oisI = go.AddComponent<ObjectIsInteractable>();
                oisI.Initialize(objectId);
            }
            
            string temp = oisI.GetGuid();

            if (!GoIds.ContainsKey(temp))
            {
                GoIds.Add(temp, go);
            }
            
            return oisI;
        }

        public static GameObject GetGoFromGuid(string guid)
        {
            GameObject temp;

            if(GoIds == null)
            {
                GoIds = new Dictionary<string, GameObject>();
            }
            

            GoIds.TryGetValue(guid, out temp);

            Debug.Log(temp);
            return temp;
        }

    }
}
