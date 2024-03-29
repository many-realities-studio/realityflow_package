using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Scripts;
using UnityEditor;
using UnityEngine;
using GraphProcessor;

namespace RealityFlow.Plugin.Editor
{
    /// <summary>
    /// A new editor window to handle changing a unity object's parameters
    /// </summary>
    public class VSGraphSettings : EditorWindow
    {
        private static VSGraphSettings window;

        public string VSGraphName { get; private set; }

        public static void OpenWindow()
        {
            window = (VSGraphSettings)GetWindow(typeof(VSGraphSettings));
            window.minSize = new Vector2(200, 200);
            window.Show();
        }

        private void OnGUI()
        {
            DrawSettings();
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            VSGraphName = EditorGUILayout.TextField(VSGraphName);
            EditorGUILayout.EndHorizontal();

            if (VSGraphName == null)
            {
                EditorGUILayout.HelpBox("This visual scripting graph needs a [Name] before it can be created.", MessageType.Warning);
            }

            else
            {
                if (GUILayout.Button("Create", GUILayout.Height(30)))
                {
                    Debug.Log("Hit create");

                    FlowVSGraph graph = new FlowVSGraph(VSGraphName);
                    Debug.Log(graph.Name);
                    graph.Name = VSGraphName;
                    Debug.Log(graph);
                    Debug.Log(JsonUtility.ToJson(graph));

                    Operations.CreateVSGraph(graph, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => Debug.Log(e.message));

                    window.Close();
                }
            }
        }
    }
}