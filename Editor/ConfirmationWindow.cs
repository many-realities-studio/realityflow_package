//using RealityFlow.Plugin.Scripts.Events;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealityFlow.Plugin.Editor
{
    public class ConfirmationWindow : EditorWindow
    {
        private static ConfirmationWindow window;

        // TODO: What does this do?
        public static void OpenWindow()
        {
            window = (ConfirmationWindow)GetWindow(typeof(ConfirmationWindow));
            window.minSize = new Vector2(200, 50);
            window.maxSize = new Vector2(200, 50);
            window.Show();
        }

        private void OnGUI()
        {
            DrawConfirm();
        }

        private void DrawConfirm()
        {
            GUILayout.Label("Do you want to keep a \nlocal copy of this project?");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm", GUILayout.Height(20)))
            {
                string path2 = "Assets/resources/projects/test.txt";

                StreamWriter writer = new StreamWriter(path2, true);
                writer.Close();

                window.Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(20)))
            {
                window.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}