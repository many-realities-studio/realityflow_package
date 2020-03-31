﻿using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class BehaviourEventManager : MonoBehaviour
    {
        public event Action<string> SendEventDown;
        public Dictionary<string, GameObject> GoIds = null;
        public States DefaultInteractableStates;

        void Awake()
        {
            if(GoIds == null)
            {
                GoIds = new Dictionary<string, GameObject>();
            }
            
        }

        public void Initialize()
        {
            GoIds = new Dictionary<string, GameObject>();
           // DefaultInteractableStates = ScriptableObject.CreateInstance<States>();
        }

        public BehaviourEvent CreateNewBehaviourEvent(string name, string go1, string go2, BehaviourEvent chain)
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
        public void DeleteBehaviourEvent(string  go1, string go2, BehaviourEvent bEvent)
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
        public void ListenToEvents(string eventName, string go1, string go2, BehaviourEvent chain)
        {
            // sends updated coordinates
        }

        public ObjectIsInteractable MakeObjectInteractable(GameObject go, string objectId)
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

        public GameObject GetGoFromGuid(string guid)
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
