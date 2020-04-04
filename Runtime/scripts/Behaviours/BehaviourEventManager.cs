using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class BehaviourEventManager : MonoBehaviour
    {
        public event Action<string> SendEventDown;
        public Dictionary<Guid, GameObject> GoIds;
        public States DefaultInteractableStates;
        
        void Awake()
        {
            GoIds = new Dictionary<Guid, GameObject>();
        }

        public BehaviourEvent CreateNewBehaviourEvent(string name, Guid go1, Guid go2, BehaviourEvent chain)
        {
            GameObject g1 = GetGoFromGuid(go1);
            if (g1)
            {
                BehaviourEvent bEvent = g1.AddComponent<BehaviourEvent>();
                if (bEvent)
                {
                    bEvent.SetName(name);
                    bEvent.SetSecondObject(go2);
                    bEvent.SetChain(chain);
                }

                g1.GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);
                GetGoFromGuid(go2).GetComponent<ObjectIsInteractable>().AddInteractableEvent(bEvent);

                return bEvent;
            }
            return null;
        }

        public BehaviourEvent AddChain(BehaviourEvent b1, BehaviourEvent b2)
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
        public void DeleteBehaviourEvent(Guid go1, Guid go2, BehaviourEvent bEvent)
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
        public void ListenToEvents(string eventName, Guid go1, Guid go2, BehaviourEvent chain)
        {
            // sends updated coordinates
        }

        public ObjectIsInteractable MakeObjectInteractable(GameObject go)
        {
            ObjectIsInteractable oisI = go.AddComponent<ObjectIsInteractable>();

            Guid temp = oisI.GetGuid();

            GoIds.Add(temp, go);

            return oisI;
        }

        public GameObject GetGoFromGuid(Guid guid)
        {
            GameObject temp;

            GoIds.TryGetValue(guid, out temp);

            Debug.Log(temp);
            return temp;
        }

    }
}
