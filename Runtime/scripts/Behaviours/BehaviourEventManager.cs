using System;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class BehaviourEventManager : MonoBehaviour
    {
        public event Action<string> SendEventDown;
        public Dictionary<Guid, GameObject> GoIds=new Dictionary<Guid, GameObject>();
        public static List<BehaviourEvent> blist = new List<BehaviourEvent>();

        void Start()
        {
            GoIds = new Dictionary<Guid, GameObject>();
        }

        public BehaviourEvent CreateNewBehaviourEvent(string name, Guid go1, Guid go2, BehaviourEvent chain)
        {
            GameObject g1 = GetGoFromGuid(go1);
            BehaviourEvent bEvent = g1.GetComponent<BehaviourEvent>();

            if(bEvent == null)
                bEvent = g1?.AddComponent<BehaviourEvent>();

            if (bEvent)
            {
                bEvent.SetfirstObject(go1);
                bEvent.SetName(name);
                bEvent.SetSecondObject(go2);
                bEvent.SetChain(chain);
            }

            return bEvent;
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

        public Guid MakeObjectInteractable(GameObject go)
        {
            ObjectIsInteractable oisI = go.GetComponent<ObjectIsInteractable>();

            if(oisI == null)
                oisI = go.AddComponent<ObjectIsInteractable>();

            oisI.objectId = Guid.NewGuid();
            Guid temp = oisI.GetGuid();
            Debug.Log(go);
            Debug.Log(temp);
            GoIds.Add(temp, go);
            return temp;
        }

        public GameObject GetGoFromGuid(Guid guid)
        {
            GameObject temp;

            GoIds.TryGetValue(guid, out temp);
            return temp;
        }

    }
}
