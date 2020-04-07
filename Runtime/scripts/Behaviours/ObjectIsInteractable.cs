using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.EventSystems;

namespace Behaviours
{
    /// <summary>
    /// Determines whether the object is interactable and logs which objects and events it is interactable with
    /// </summary>
    public class ObjectIsInteractable : MonoBehaviour, IPointerClickHandler
    {
        private Dictionary<string, bool> interactableWith;
        private Dictionary<BehaviourEvent, string> interactableEvents;

        public event Action SetEventTrigger;

        private Interactable interactableScript;

        private string CurrentEvent = null;

        private string objectId;





        #region Monobehaviour Methods

        /// <summary>
        /// Returns assigned guid
        /// </summary>
        public string GetGuid()
        {
            return objectId;
        }

        /// <summary>
        /// Initializes private variables
        /// </summary>
        public void Awake()
        {
            /*objectId = Guid.NewGuid();
            interactableWith = new Dictionary<Guid, bool>();
            interactableEvents = new Dictionary<BehaviourEvent, string>();
            interactableScript = gameObject.AddComponent<Interactable>();

            //interactableScript.Profiles[0].Target = gameObject;
            //interactableScript.Profiles[0].Themes.Add(new Theme());
            interactableScript.IsEnabled = true;
            interactableScript.States = FindObjectOfType<BehaviourEventManager>().DefaultInteractableStates;
            interactableScript.OnClick.AddListener(() => OnSelect());
            */
        }

        #endregion // Monobehaviour Methods

        #region Trigger Methods

        /// <summary>
        /// Determines logic based on user's click
        /// </summary>
        public void OnSelect()
        {
            Debug.Log("clicked");
            if (interactableEvents.ContainsValue("Click"))
            {
                SetEventTrigger.Invoke();
            }
        }

        //private void OnMouseDown()
        //{
        //    Debug.Log("clicked");
        //    if (interactableEvents.ContainsValue("Click"))
        //    {
        //        SetEventTrigger.Invoke();
        //    }
        //}


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerPressRaycast.gameObject == gameObject)
            {
                Debug.Log("clicked");
                if (interactableEvents.ContainsValue("Click"))
                {
                    SetEventTrigger.Invoke();
                }
            }
        }

        /// <summary>
        /// Determines logic upon colliding with another object
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (CurrentEvent != null)
            {
                return;
            }
            if (interactableEvents.ContainsValue("Teleport"))
            {
                SetEventTrigger.Invoke();
                CurrentEvent = "Teleport";
            }
            else if (interactableEvents.ContainsValue("SnapZone"))
            {
                SetEventTrigger.Invoke();
                CurrentEvent = "SnapZone";
            }
        }

        public void Initialize(string objectId)
        {
            this.objectId = objectId;
            interactableWith = new Dictionary<string, bool>();
            interactableEvents = new Dictionary<BehaviourEvent, string>();

            if (!(SystemInfo.deviceType == DeviceType.Desktop || SystemInfo.deviceType == DeviceType.Handheld))
            {
                interactableScript = gameObject.AddComponent<Interactable>();

                //interactableScript.Profiles[0].Target = gameObject;
                //interactableScript.Profiles[0].Themes.Add(new Theme());
                interactableScript.IsEnabled = true;
                //interactableScript.States = BehaviourEventManager.DefaultInteractableStates;
                interactableScript.OnClick.AddListener(() => OnSelect());
            }
            else
            {
                if (Camera.main.gameObject.GetComponent<PhysicsRaycaster>() == null)
                {
                    Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
                }
            }
        }

        /// <summary>
        /// Allows all scripts to function normally once again
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionExit(Collision collision)
        {
            CurrentEvent = null;
        }

        #endregion // Trigger Methods

        #region Custom Methods

        /// <summary>
        /// Adds Interactable Objects to dictionary of objects interactable with current GameObject
        /// </summary>
        /// <param name="interactables"></param>
        public void InteractableWith(List<string> interactables)
        {
            for (int i = 0; i < interactables.Count; i++)
            {
                if (!interactableWith.ContainsKey(interactables[i]))
                    interactableWith.Add(interactables[i], true);
            }
        }

        /// <summary>
        /// Adds Interactable Events to dictionary of events interactable with current GameObject
        /// </summary>
        /// <param name="events"></param>
        public void AddInteractableEvents(List<BehaviourEvent> events)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (!interactableEvents.ContainsKey(events[i]))
                {
                    SetEventTrigger += events[i].EventTrigger;
                    interactableEvents.Add(events[i], events[i].GetName());
                }
            }
        }


        public void AddInteractableEvent(BehaviourEvent e)
        {
            if(interactableEvents == null)
            {
                interactableEvents = new Dictionary<BehaviourEvent, string>();
            }
            
            if (!interactableEvents.ContainsKey(e))
            {
                SetEventTrigger += e.EventTrigger;
                interactableEvents.Add(e, e.GetName());
            }
        }

        /// <summary>
        /// Remove a GameObject from the dictionary of interactable objects
        /// </summary>
        /// <param name="go"></param>
        public void RemoveInteractableObject(string go)
        {
            if (interactableWith.ContainsKey(go))
            {
                interactableWith.Remove(go);
            }
        }

        /// <summary>
        /// Disables an interactable object in the dictionary
        /// </summary>
        /// <param name="go"></param>
        public void TurnOffInteractableObject(string go)
        {
            if (interactableWith.ContainsKey(go))
            {
                interactableWith.Remove(go);
                interactableWith.Add(go, false);
            }
        }

        /// <summary>
        /// Removes an event from the dictionary of interactable events
        /// </summary>
        /// <param name="bEvent"></param>
        /// <param name="go2"></param>
        public void RemoveInteractableEvent(BehaviourEvent bEvent, string go2)
        {
            if (interactableEvents.ContainsKey(bEvent))
            {
                if (go2 != null)
                {
                    if (bEvent.GetSecondObject() == go2)
                    {
                        interactableEvents.Remove(bEvent);
                        GameObject g2 = BehaviourEventManager.GetGoFromGuid(go2);
                        if (g2.GetComponent<ObjectIsInteractable>())
                        {
                            g2.GetComponent<ObjectIsInteractable>().RemoveInteractableEvent(bEvent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes an event from the dictionary of interactable events (dependent GameObject)
        /// </summary>
        /// <param name="bEvent"></param>
        public void RemoveInteractableEvent(BehaviourEvent bEvent)
        {
            if (interactableEvents.ContainsKey(bEvent))
            {
                interactableEvents.Remove(bEvent);
            }
        }

        /// <summary>
        /// Turns off an event in the dictionary
        /// </summary>
        /// <param name="bEvent"></param>
        public void TurnOffInteractableEvent(BehaviourEvent bEvent)
        {
            if (interactableEvents.ContainsKey(bEvent))
            {
                interactableEvents.Remove(bEvent);
                interactableEvents.Add(bEvent, null);
            }
        }

        #endregion // Custom Methods

        #region Interactablility Bools

        /// <summary>
        /// Determines whether the GameObject is interactable with this GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool IsInteractableWith(string go)
        {
            if (interactableWith.ContainsKey(go))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if the event is interactable with this GameObject
        /// </summary>
        /// <param name="bEvents"></param>
        /// <returns></returns>
        public bool IsInteractableWithEvent(BehaviourEvent bEvents)
        {
            if (interactableEvents.ContainsKey(bEvents))
                return true;

            return false;
        }

        #endregion // Interactability Bools
    }

}
