using Packages.realityflow_package.Runtime.scripts;
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

            if (GUILayout.Button("Checkin Object"))
            {
                myFlowObject.underlyingFlowObject.CheckIn();
            }

            if (GUILayout.Button("Checkout Object"))
            {
                myFlowObject.underlyingFlowObject.CheckOut();
            }

            DrawDefaultInspector();
        }
    }
}