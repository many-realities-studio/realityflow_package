using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

namespace Behaviours
{
    /// <summary>
    /// Defines necesary parts of behaviour events for interactable objects
    /// </summary>
    public class BehaviourEvent : MonoBehaviour
    {
        private BehaviourEventManager BEM;

        public string behaviourName = "default";

        public Guid firstObject;

        public Guid secondObject;

        public BehaviourEvent chainedEvent;

        public event Action<string, Guid, Guid, BehaviourEvent> EventCalled;

        #region Monobehaviour Functions

        /// <summary>
        /// Calls initialization functions
        /// </summary>
        private void Start()
        {
            firstObject = gameObject.GetComponent<ObjectIsInteractable>().GetGuid();
            GetBehaviourEventManager();
        }

        #endregion // Monobehaviour Functions

        #region Custom Functions

        /// <summary>
        /// Finds Behaviour Event Manager and assigns it to local variable for later reference
        /// </summary>
        private void GetBehaviourEventManager()
        {
            BEM = FindObjectOfType<EventSystem>().GetComponent<BehaviourEventManager>();
            BEM.SendEventDown += OnCallDown;
            EventCalled += BEM.ListenToEvents;
        }

        /// <summary>
        /// Invokes event for this behaviour - subscribed to by BEM
        /// </summary>
        public void CallBehaviourEvent()
        {
            EventCalled.Invoke(behaviourName, firstObject, secondObject, chainedEvent);
        }

        /// <summary>
        /// Determines logic based on scriptName to decide on what should be done on event call
        /// </summary>
        /// <param name="scriptName"></param>
        private void OnCallDown(string scriptName)
        {
            if (scriptName != behaviourName)
                return;

           // if (BEM.GetGoFromGuid(secondObject) == null)
            GameObject obj = BEM.GetGoFromGuid(secondObject);
            Debug.Log("HELLO");
            Debug.Log(obj);

            CallBehaviourEvent();

            // figure out how to add specific code to each script here
            switch (scriptName)
            {
                case "Teleport":
                    // take in input from colliding object that determines teleport coordinates (teleport nodes)
                    Vector3 coords = obj.GetComponent<TeleportCoordinates>().GetCoordinates();
                    transform.position = coords;
                    // Set teleport rest for 5 seconds
                    return;
                case "Click":
                    // if object received click input, then trigger chained event
                    if (chainedEvent)
                    {
                        chainedEvent.EventTrigger();
                    }
                    return;
                case "Snap Zone":
                    // take in more info than teleport, but basically acts as a teleport within the other object
                    if (obj.GetComponent<TeleportCoordinates>().IsSnapZone())
                    {
                        transform.position = obj.GetComponent<TeleportCoordinates>().GetCoordinates();
                        transform.localScale = obj.GetComponent<TeleportCoordinates>().GetScale();
                        transform.rotation = obj.GetComponent<TeleportCoordinates>().GetRotation();
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

        #endregion // Custom Functions

        #region Setters and Getters

        public void SetName(string name)
        {
            behaviourName = name;
        }

        public string GetName()
        {
            return behaviourName;
        }

        public void SetSecondObject(Guid go2)
        {
            secondObject = go2;
        }

        public Guid GetSecondObject()
        {
            return secondObject;
        }
        public void SetfirstObject(Guid go2)
        {
            firstObject = go2;
        }

        public Guid GetFirstObject()
        {
            return firstObject;
        }
        public void SetChain(BehaviourEvent chain)
        {
            chainedEvent = chain;
        }

        public void EventTrigger()
        {
            OnCallDown(behaviourName);
        }

        #endregion // Setters and Getters
    }
}
