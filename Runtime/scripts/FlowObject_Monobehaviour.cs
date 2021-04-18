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

        private void start()
        {
        }

        private void OnEnable()
        {
            constraintsFrozen = true;
        }

        private void OnValidate()
        {
        }

        public void Update()
        {
            if (underlyingFlowObject != null)
            {
                underlyingFlowObject.UpdateObjectGlobally(underlyingFlowObject);
            }

            if (underlyingFlowObject.CanBeModified == true && constraintsFrozen)
            {
                constraintsFrozen = false;
                objectRigidbody.constraints = RigidbodyConstraints.None;
            }
            
            if (underlyingFlowObject.CanBeModified == false && constraintsFrozen == false)
            {
                constraintsFrozen = true;
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}