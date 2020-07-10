using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    [ExecuteAlways]
    [Serializable]
    public class FlowObject_Monobehaviour : MonoBehaviour
    {
        public FlowTObject underlyingFlowObject;

        private void OnEnable()
        {
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
        }
    }
}