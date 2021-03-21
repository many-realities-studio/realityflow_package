using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    [ExecuteAlways]
    [Serializable]
    public class FlowVSGraph_Monobehaviour : MonoBehaviour
    {
        public FlowVSGraph underlyingFlowVSGraph;

        private void OnEnable()
        {
        }

        private void OnValidate()
        {
            //Debug.Log("OnValidate");
        }

        public void Update()
        {
            if (underlyingFlowVSGraph != null)
            {
                underlyingFlowVSGraph.UpdateFlowVSGraphGlobally(underlyingFlowVSGraph);
            }
        }
    }
}