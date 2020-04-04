using Packages.realityflow_package.Runtime.scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Packages.realityflow_package.Editor
{
    [CustomEditor(typeof(FlowObject_Monobehaviour))]
    public class FlowTObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FlowObject_Monobehaviour myFlowObject = (FlowObject_Monobehaviour)target;

            if(GUILayout.Button("Checkin Object"))
            {
                myFlowObject.underlyingFlowObject.CheckIn();
            }

            if (GUILayout.Button("Checkout Object"))
            {
                myFlowObject.underlyingFlowObject.CheckOut();
            }

            //if(GUILayout.Button("Checkin / Checkout object"))
            //{
            //    if(myFlowObject.checkedOut == false)
            //    {
            //        if(myFlowObject == null)
            //        {
            //            Debug.Log("myFlowObject is null");
            //        }
            //        else if(myFlowObject.underlyingFlowObject == null)
            //        {
            //            Debug.Log("underlying object is null");
            //        }

            //        Operations.CheckoutObject(myFlowObject.underlyingFlowObject.Id, ConfigurationSingleton.CurrentProject.Id, (_, e) =>
            //        {
            //            Debug.Log("Checkout Success: " + e.message.WasSuccessful);
            //            myFlowObject.checkedOut = e.message.WasSuccessful;
            //        });
            //    }
            //    else
            //    {
            //        Operations.CheckinObject(myFlowObject.underlyingFlowObject.Id, ConfigurationSingleton.CurrentProject.Id, (_, e) =>
            //        {
            //            Debug.Log("Checkout Success: " + e.message.WasSuccessful);
            //            myFlowObject.checkedOut = !e.message.WasSuccessful;
            //        });
            //    }
            //}
            DrawDefaultInspector();
        }
    }
}
