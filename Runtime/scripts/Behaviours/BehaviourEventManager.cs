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
        public static Dictionary<Guid, GameObject> GoIds;
        public static Dictionary<Guid, FlowBehaviour> GuidToBehaviourMapping;
        public static States DefaultInteractableStates;

        static BehaviourEventManager()
        {
            GoIds = new Dictionary<Guid, GameObject>();
            GuidToBehaviourMapping = new Dictionary<Guid, FlowBehaviour>();
        }

        public static BehaviourEvent CreateNewBehaviourEvent(string name, Guid go1, Guid go2, List<Guid> chain)
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

                g1.GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);
                GetGoFromGuid(go2).GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);

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
        public static void DeleteBehaviourEvent(Guid go1, Guid go2, BehaviourEvent bEvent)
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
        public static void ListenToEvents(string eventName, Guid go1, Guid go2, List<Guid> chain)
        {
            // sends updated coordinates
        }

        public static ObjectIsInteractable MakeObjectInteractable(GameObject go)
        {
            ObjectIsInteractable oisI = go.AddComponent<ObjectIsInteractable>();

            Guid temp = oisI.GetGuid();

            GoIds.Add(temp, go);

            return oisI;
        }

        public static GameObject GetGoFromGuid(Guid guid)
        {
            GameObject temp;

            GoIds.TryGetValue(guid, out temp);

            Debug.Log(temp);
            return temp;
        }

    }
}
