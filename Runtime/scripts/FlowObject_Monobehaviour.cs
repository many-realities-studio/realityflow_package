using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    [ExecuteAlways]
    [Serializable]
    public class FlowObject_Monobehaviour : MonoBehaviour
    {
        public Color latestColor;
        public FlowTObject underlyingFlowObject;

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
            /*if(underlyingFlowObject != null && gameObject.GetComponent<Renderer>().material.color != latestColor)
            {
                latestColor = gameObject.GetComponent<Renderer>().material.color;
                underlyingFlowObject.UpdateObjectGlobally(underlyingFlowObject);
            }*/
        }
    }
}