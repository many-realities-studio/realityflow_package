//using RealityFlow.Plugin.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using RealityFlow.Plugin.Scripts;
using Packages.realityflow_package.Runtime.scripts;

namespace RealityFlow.Plugin.Editor
{
    /// <summary>
    /// A new editor window to handle changing a unity object's parameters
    /// </summary>
    public class ObjectSettings : EditorWindow
    {
        static ObjectSettings window;

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
            //DrawSettings((ObjectData)RealityFlowWindow.ObjectInfo);
        }

        void DrawSettings()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            ObjectName = EditorGUILayout.TextField(ObjectName);
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("Mesh");
            //if(createdGameObject.TryGetComponent<MeshFilter>(out MeshFilter meshComponent))
            //{
            //    meshComponent.mesh = (Mesh)EditorGUILayout.ObjectField(meshComponent.mesh, typeof(Mesh), false);
            //}
            //else
            //{
            //    GUILayout.Label("Object has no mesh filter component");
            //    EditorGUILayout.HelpBox("This object needs a [Mesh] before it can be created.", MessageType.Warning);
            //}
            //EditorGUILayout.EndHorizontal();

            // Textures are currently not supported on the live server due to lag issues
            //-----------------------------------------------------------------------------------------------------
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label("Texture");
            // objData.texture = (Texture2D)EditorGUILayout.ObjectField(objData.texture, typeof(Texture2D), false);
            // EditorGUILayout.EndHorizontal();
            //-----------------------------------------------------------------------------------------------------

            EditorGUILayout.BeginHorizontal();
            ObjectPosition = EditorGUILayout.Vector3Field("Position", ObjectPosition);
            EditorGUILayout.EndHorizontal();

            //createdGameObject.transform.rotation = EditorGUILayout.Vector3Field("Rotation", createdGameObject.transform.rotation);

            EditorGUILayout.BeginHorizontal();
            ObjectScale = EditorGUILayout.Vector3Field("Scale", ObjectScale);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectRotation = EditorGUILayout.Vector4Field("Rotation", ObjectRotation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            ObjectPrefab = EditorGUILayout.TextField("Prefab", ObjectPrefab);
                /*EditorGUILayout.ObjectField("Prefab to search for", ChosenPrefab, typeof(GameObject), false) as GameObject;*/
            EditorGUILayout.EndHorizontal();

            //objData.color = EditorGUILayout.ColorField("Color", objData.color);

            //if (objData.mesh == null)
            //{
            //    EditorGUILayout.HelpBox("This object needs a [Mesh] before it can be created.", MessageType.Warning);
            //}
            /*else*/
            if (ObjectName == null)
            {
                EditorGUILayout.HelpBox("This object needs a [Name] before it can be created.", MessageType.Warning);
            }
            else if(ObjectPrefab == null)
            {
                EditorGUILayout.HelpBox("This object needs a [Prefab] before it can be created", MessageType.Warning);
            }
            // TODO: Add extra else if case to check if the name already exists in the project
            else
            {
                if (GUILayout.Button("Create", GUILayout.Height(30)))
                {
                    FlowTObject createdGameObject = new FlowTObject(ObjectName, ObjectPosition, new Quaternion(ObjectRotation.x, ObjectRotation.y, ObjectRotation.z, ObjectRotation.w), ObjectScale, new Color(), ObjectPrefab);
                    
                    Operations.CreateObject(createdGameObject, /*FlowNetworkManagerEditor.currentUser,*/ ConfigurationSingleton.CurrentProject.Id, (_, e) => Debug.Log(e.message));
                    
                    //SetupObjectManager();
                    //SaveObjectData(objData);
                    window.Close();
                }
            }
        }

        //void SetupObjectManager()
        //{
        //    // string prefabPath; //path to the base prefab
        //    string s = "ObjManager";

        //    // Check if there is already an object manager
        //    // if not, create one
        //    GameObject manager = GameObject.FindGameObjectWithTag(s);
        //    bool managerExists = true;

        //    if (manager == null)
        //    {
        //        manager = new GameObject("ObjManager");
        //        manager.tag = s;
        //        managerExists = false;
        //    }

        //    // If the manager was just created add the necessary components
        //    if (managerExists == false)
        //    {
        //        manager.AddComponent(typeof(ObjectManager));
        //        //manager.AddComponent(typeof(FlowNetworkManager));
        //        //manager.GetComponent<FlowNetworkManager>().LocalServer = Config.LOCAL_HOST;
        //        //manager.GetComponent<FlowNetworkManager>().mainGameCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //        manager.AddComponent(typeof(DoOnMainThread));
        //    }
        //}

        //public static void SaveObjectData(ObjectData objectData)
        //{

        //    FlowTObject obj = new FlowTObject();

        //    obj.vertices = objectData.mesh.vertices;
        //    obj.uv = objectData.mesh.uv;
        //    obj.triangles = objectData.mesh.triangles;
        //    obj.x = objectData.position.x;
        //    obj.y = objectData.position.y;
        //    obj.z = objectData.position.z;


        //    Quaternion rot = Quaternion.Euler(objectData.rotation);
        //    obj.q_x = rot.x;
        //    obj.q_y = rot.y;
        //    obj.q_z = rot.z;
        //    obj.q_w = rot.w;

        //    obj.s_x = objectData.scale.x;
        //    obj.s_y = objectData.scale.y;
        //    obj.s_z = objectData.scale.z;

        //    obj.type = "BoxCollider";
        //    obj.name = objectData.objectName;
        //    obj.color = objectData.color;

        //    // Textures are currently not supported on the live server due to lag
        //    //-------------------------------------------------------------------------
        //    // obj.texture = RealityFlowWindow.ObjectInfo.texture.GetRawTextureData();
        //    // obj.textureHeight = RealityFlowWindow.ObjectInfo.texture.height;
        //    // obj.textureWidth = RealityFlowWindow.ObjectInfo.texture.width;
        //    // obj.textureFormat = (int)RealityFlowWindow.ObjectInfo.texture.format;
        //    // obj.mipmapCount = RealityFlowWindow.ObjectInfo.texture.mipmapCount;
        //    //-------------------------------------------------------------------------

        //    ObjectCreationEvent createObject = new ObjectCreationEvent();
        //    createObject.Send(obj);
        //}
    }
}

