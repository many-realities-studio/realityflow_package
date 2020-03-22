﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Behaviours
{
    /// <summary>
    /// Defines necesary parts of behaviour events for interactable objects
    /// </summary>
    public class BehaviourEvent : MonoBehaviour
    {
        private BehaviourEventManager BEM;
        [SerializeField]
        private string behaviourName = "default";

        [SerializeField]
        private Guid firstObject;
        [SerializeField]
        private Guid secondObject;
        [SerializeField]
        public BehaviourEvent chainedEvent;

        public event Action<string, Guid, Guid, BehaviourEvent> EventCalled;

        #region Monobehaviour Functions

        /// <summary>
        /// Calls initialization functions
        /// </summary>
        private void Awake()
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
            BEM = FindObjectOfType<BehaviourEventManager>();
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
            Debug.Log("here!!");

            GameObject obj = BEM.GetGoFromGuid(secondObject);

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
                        Debug.Log("execute chained event");
                        chainedEvent.EventTrigger();
                    }
                    return;
                case "SnapZone":
                    // take in more info than teleport, but basically acts as a teleport within the other object
                    if (obj.GetComponent<TeleportCoordinates>().IsSnapZone())
                    {
                        transform.position = obj.transform.position + obj.GetComponent<TeleportCoordinates>().GetCoordinates();
                        transform.localScale = obj.GetComponent<TeleportCoordinates>().GetScale();
                        transform.rotation = obj.GetComponent<TeleportCoordinates>().GetRotation();
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