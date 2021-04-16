// using Packages.realityflow_package.Runtime.scripts;
// using UnityEditor;
// using UnityEngine;

// namespace Packages.realityflow_package.Editor
// {
//     [CustomEditor(typeof(FlowVSGraph_Monobehaviour))]
//     public class FlowVSGraphEditor : UnityEditor.Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             FlowVSGraph_Monobehaviour myFlowVSGraph = (FlowVSGraph_Monobehaviour)target;

//             if (GUILayout.Button("Checkin Graph"))
//             {
//                 myFlowVSGraph.underlyingFlowVSGraph.CheckIn();
//             }

//             if (GUILayout.Button("Checkout Graph"))
//             {
//                 myFlowVSGraph.underlyingFlowVSGraph.CheckOut();
//             }

//             DrawDefaultInspector();
//         }
//     }
// }