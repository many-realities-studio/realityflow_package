//using RealityFlow.Plugin.Scripts.Events;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Scripts;
using UnityEditor;
using UnityEngine;

namespace RealityFlow.Plugin.Editor
{
    /// <summary>
    /// A new editor window to handle changing a unity object's parameters
    /// </summary>
    public async class ObjectSettings : EditorWindow
    {
        private static ObjectSettings window;

        public string ObjectName { get; private set; }
        public Vector3 ObjectPosition { get; private set; }
        public Vector3 ObjectScale { get; private set; } = new Vector3(1, 1, 1);
        public Vector4 ObjectRotation { get; private set; } = new Vector4(0, 0, 0, 1);
        public string ObjectPrefab { get; private set; }

        // TODO: What does this do?
        public static void OpenWindow()
        {
            window = (ObjectSettings)GetWindow(typeof(ObjectSettings));
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
            ObjectName = EditorGUILayout.TextField(ObjectName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectPosition = EditorGUILayout.Vector3Field("Position", ObjectPosition);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectScale = EditorGUILayout.Vector3Field("Scale", ObjectScale);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectRotation = EditorGUILayout.Vector4Field("Rotation", ObjectRotation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectPrefab = EditorGUILayout.TextField("Prefab", ObjectPrefab);
            EditorGUILayout.EndHorizontal();

            if (ObjectName == null)
            {
                EditorGUILayout.HelpBox("This object needs a [Name] before it can be created.", MessageType.Warning);
            }
            else if (ObjectPrefab == null)
            {
                EditorGUILayout.HelpBox("This object needs a [Prefab] before it can be created", MessageType.Warning);
            }

            // TODO: Add extra else if case to check if the name already exists in the project
            else
            {
                if (GUILayout.Button("Create", GUILayout.Height(30)))
                {
                    FlowTObject createdGameObject = new FlowTObject(ObjectName, ObjectPosition, new Quaternion(ObjectRotation.x, ObjectRotation.y, ObjectRotation.z, ObjectRotation.w), ObjectScale, new Color(), ObjectPrefab);

                    Operations.CreateObject(createdGameObject, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => Debug.Log(e.message));

                    window.Close();
                }
            }
        }
    }
}