using Microsoft.MixedReality.Toolkit.UI;
using Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public static class BehaviourEventManager
    {
        public static event Action<string> SendEventDown;
        public static Dictionary<string, GameObject> GoIds;
        public static Dictionary<string, FlowBehaviour> GuidToBehaviourMapping;
        public static States DefaultInteractableStates;

        static BehaviourEventManager()
        {
            GoIds = new Dictionary<string, GameObject>();
            GuidToBehaviourMapping = new Dictionary<string, FlowBehaviour>();
        }

        public static BehaviourEvent CreateNewBehaviourEvent(string name, string go1, string go2, List<string> chain)
        {
            GameObject g1 = GetGoFromGuid(go1);
            if (g1)
            {
                BehaviourEvent bEvent = g1.AddComponent<BehaviourEvent>();
                if (bEvent)
                {
                    bEvent._FlowBehaviour.TypeOfTrigger = name;
                    bEvent._FlowBehaviour.TargetObjectId = go2;
                    bEvent._FlowBehaviour.NextBehaviorList = chain;
                }

                g1.GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent._FlowBehaviour);
                GetGoFromGuid(go2).GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent._FlowBehaviour);

                return bEvent;
            }
            return null;
        }

        //public BehaviourEvent AddChain(FlowBehaviour b1, FlowBehaviour b2)
        //{
        //    FlowBehaviour temp = b1;

        //    while (temp.NextBehaviorList != null)
        //    {
        //        temp = temp.next;
        //    }

        //    temp.SetChain(b2);
        //    return b1;
        //}

        // needs to check second gameobject
        public static void DeleteBehaviourEvent(string go1, string go2, BehaviourEvent bEvent)
        {
            GameObject g1 = GetGoFromGuid(go1);
            ObjectIsInteractable interactScript = g1.GetComponent<ObjectIsInteractable>();

            if (interactScript == null)
                return;

            interactScript.RemoveInteractableEvent(bEvent._FlowBehaviour, go2);
        }

        /// <summary>
        /// Registers when a BehaviourEvent has been called - sends up to server communicator
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="go1"></param>
        /// <param name="go2"></param>
        /// <param name="chain"></param>
        public static void ListenToEvents(string eventName, string go1, string go2, List<string> chain)
        {
            // sends updated coordinates
        }

        public static ObjectIsInteractable MakeObjectInteractable(GameObject go)
        {
            ObjectIsInteractable oisI = go.AddComponent<ObjectIsInteractable>();

            string temp = oisI.GetGuid();

            GoIds.Add(temp, go);

            return oisI;
        }

        public static GameObject GetGoFromGuid(string guid)
        {
            GameObject temp;

            GoIds.TryGetValue(guid, out temp);

            Debug.Log(temp);
            return temp;
        }

    }
}
