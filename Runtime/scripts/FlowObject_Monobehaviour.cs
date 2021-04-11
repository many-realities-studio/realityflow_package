using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace Packages.realityflow_package.Runtime.scripts
{
    [ExecuteAlways]
    [Serializable]
    public class FlowObject_Monobehaviour : MonoBehaviour
    {
        public Rigidbody objectRigidbody;
        public Color latestColor;
        public FlowTObject underlyingFlowObject;

        private bool constraintsFrozen;
        //private IMixedRealityPointer _pointer;  
        private void start()
        {
            // handler = gameObject.GetComponent<ObjectManipulator>();
            // handler.OnManipulationStarted.AddListener(HandleOnManipulationStarted);
            // handler.OnManipulationEnded.AddListener(HandleOnManipulationEnded);
        }
        private void HandleOnManipulationStarted(ManipulationEventData eventData)
        {
            //_pointer = eventData.Pointer;
            Debug.Log("Manipulation start");
            underlyingFlowObject.CheckOut();
        }
        private void HandleOnManipulationEnded(ManipulationEventData eventData)
        {
            Debug.Log("Manipulation end");
            underlyingFlowObject.CheckIn();
        }
        /*private void Start()
        {
            if(gameObject.GetComponent<Renderer>()!=null)
				latestColor = gameObject.GetComponent<Renderer>().material.color;
			else
			{
				latestColor = gameObject.AddComponent<Renderer>().material.color;
			}
        }*/

        private void OnEnable()
        {
            // objectRigidbody = gameObject.GetComponent<Rigidbody>();
            // objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            constraintsFrozen = true;
        }

        private void OnValidate()
        {
            //Debug.Log("OnValidate");
        }

        public void Update()
        {
            if (underlyingFlowObject != null)
            {
                underlyingFlowObject.UpdateObjectGlobally(underlyingFlowObject);
            }

            if (underlyingFlowObject.CanBeModified == true && constraintsFrozen)
            {
                Debug.LogError("NodeView can be edited but the constraints are frozen. We now unfreeze them.");
                constraintsFrozen = false;
                objectRigidbody.constraints = RigidbodyConstraints.None;
            }
            
            if (underlyingFlowObject.CanBeModified == false && constraintsFrozen == false)
            {
                Debug.LogError("NodeView can not be edited but the constraints are not yet frozen. We now freeze them.");
                constraintsFrozen = true;
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            /*if(underlyingFlowObject != null && gameObject.GetComponent<Renderer>().material.color != latestColor)
            {
                latestColor = gameObject.GetComponent<Renderer>().material.color;
                underlyingFlowObject.UpdateObjectGlobally(underlyingFlowObject);
            }*/
        }
    }
}