using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    [ExecuteAlways]
    [Serializable]
    public class FlowAvatar_Monobehaviour : MonoBehaviour
    {
        public FlowAvatar underlyingFlowObject;
        public GameObject head, lHand, rHand;

        void Start(){
            GameObject ps = GameObject.FindGameObjectWithTag("Player"); 
            head = GameObject.FindGameObjectWithTag("MainCamera");
        }

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
                // Tell the server our transform.position of the hands & head
            }
        }
    }
}