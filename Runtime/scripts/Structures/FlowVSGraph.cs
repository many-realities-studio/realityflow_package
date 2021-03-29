using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Contrib;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System;
using UnityEngine;
using GraphProcessor;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowVSGraph : BaseGraph
    {
        [SerializeField]
        public static SerializableDictionary<string, FlowVSGraph> idToVSGraphMapping = new SerializableDictionary<string, FlowVSGraph>();

        public bool CanBeModified { get => _canBeModified; set => _canBeModified = value; }

        [SerializeField]
        private bool _canBeModified;

        public bool IsUpdated { get => _isUpdated; set => _isUpdated = value; }

        [SerializeField]
        private bool _isUpdated;

        [SerializeField]
        public string Name;

        [SerializeField]
        public string Id; //{ get => _id; set => _id = value; }

        [JsonIgnore]
        private GameObject _AttachedGameObject = null;

        [JsonIgnore]
        public GameObject AttachedGameObject
        {
            get
            {
                if (_AttachedGameObject == null)
                {
                    // The game object already exists
                    if (idToVSGraphMapping.ContainsKey(Id))
                    {
                        if (idToVSGraphMapping[Id]._AttachedGameObject == null)
                        {
                             UnityEngine.Object prefabReference = Resources.Load("prefabs/FlowVSGraph");
                            //GameObject prefabReference = Resources.Load("prefabs/VRWhiteBoard");
                            if (prefabReference == null)
                            {
                                Debug.Log("cannot load prefab");
                            }
                             idToVSGraphMapping[Id]._AttachedGameObject = GameObject.Instantiate(prefabReference) as GameObject;
                            //idToVSGraphMapping[Id]._AttachedGameObject = Instantiate(prefabReference);
                        }

                        _AttachedGameObject = idToVSGraphMapping[Id]._AttachedGameObject;
                    }

                    // The game object doesn't exist, but it should by this point
                    // Can happen when a client receives a create object request when another user created an object
                    else
                    {
                        UnityEngine.Object prefabReference = Resources.Load(Prefab);
                        if (prefabReference == null)
                        {
                            Debug.Log("cannot load prefab");
                        }
                        _AttachedGameObject = GameObject.Instantiate(prefabReference) as GameObject;
                    }
                }
                return _AttachedGameObject;
            }

            set { _AttachedGameObject = value; }
        }

        
        [JsonIgnore]
        private string _Prefab;

        public string Prefab
        {
            get { return _Prefab; }
            set { _Prefab = value; }
        }

        // [SerializeField]
        // private string _id = Guid.NewGuid().ToString();

        // public FlowVSGraph(string n) : base(){
        public FlowVSGraph(string n){
            this.Name = n;
            Debug.Log("prefab is " + Prefab);
            // this.Id = _id;
            this.Id = Guid.NewGuid().ToString();

            idToVSGraphMapping.Add(Id, this);

            AttachedGameObject.AddComponent<FlowVSGraph_Monobehaviour>();
            FlowVSGraph_Monobehaviour monoBehaviour = AttachedGameObject.GetComponent<FlowVSGraph_Monobehaviour>();

            monoBehaviour.underlyingFlowVSGraph = this;
            this.name = (this.Name + " - " + this.Id);
            // AttachedGameObject.transform.GetChild(2).GetComponent<RealityFlowGraphView>().InitializeGraph(this);
            // base.AddNode(BaseNode.CreateFromType<FloatNode> (new Vector2 ()));
        }

        [JsonConstructor]
        // public FlowVSGraph(string id, string name) : base(){
        public FlowVSGraph(string id, string name) {
            Name = name;
            Id = id;
            // WhiteboardManager.AddNewGraphToDict(this);

            if (idToVSGraphMapping.ContainsKey(id))
            {
                idToVSGraphMapping[id].UpdateFlowVSGraphLocally(this);
            }
            else // Create game object if it doesn't exist
            {
                idToVSGraphMapping.Add(Id, this);
                AttachedGameObject.name = name;
                AttachedGameObject.AddComponent<FlowVSGraph_Monobehaviour>();
                //AttachedGameObject.transform.hasChanged = false;

                var monoBehaviour = AttachedGameObject.GetComponent<FlowVSGraph_Monobehaviour>();
                monoBehaviour.underlyingFlowVSGraph = this;
                this.name = (this.Name + " - " + this.Id);
                // AttachedGameObject.transform.GetChild(2).GetComponent<RealityFlowGraphView>().InitializeGraph(this);
            }
        }

        public void UpdateFlowVSGraphGlobally(FlowVSGraph newValues)
        {
            if (IsUpdated == true)
            {
                Debug.LogError("Update VSGraph flag successfully set!!!!");
                // Debug.LogError("this Nodes before copy: " + JsonUtility.ToJson(this.serializedNodes));
                bool tempCanBeModified = this.CanBeModified;
                Debug.LogError(JsonUtility.ToJson(newValues));
                GraphPropertyCopier<FlowVSGraph, FlowVSGraph>.Copy(newValues, this);
                // Debug.LogError("this Nodes after copy: " + JsonUtility.ToJson(this.serializedNodes));
                this.CanBeModified = tempCanBeModified;

                if (CanBeModified == true)
                {
                    Operations.UpdateVSGraph(this, ConfigurationSingleton.SingleInstance.CurrentUser, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {/* Debug.Log(e.message);*/ });
                }

                _isUpdated = false;
            }
        }

        private void UpdateFlowVSGraphLocally(FlowVSGraph newValues)
        {
            // if (idToVSGraphMapping[newValues.Id].CanBeModified == false)
            // {
                bool tempCanBeModified = this.CanBeModified;
                GraphPropertyCopier<FlowVSGraph, FlowVSGraph>.Copy(newValues, this);
                this.CanBeModified = tempCanBeModified;
            // }
        }

        public static void DestroyVSGraph(string idOfObjectToDestroy)
        {
            try
            {
                GameObject objectToDestroy = idToVSGraphMapping[idOfObjectToDestroy].AttachedGameObject;
                idToVSGraphMapping.Remove(idOfObjectToDestroy);
                UnityEngine.Object.Destroy(objectToDestroy);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public static void RemoveAllGraphsFromScene()
        {
            foreach (FlowVSGraph flowVSGraph in idToVSGraphMapping.Values)
            {
                UnityEngine.Object.DestroyImmediate(flowVSGraph.AttachedGameObject);
            }
            FlowVSGraph.idToVSGraphMapping = new SerializableDictionary<string, FlowVSGraph>();
        }

        public void CheckIn()
        {
            if (CanBeModified == true)
            {
                Operations.CheckinVSGraph(Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
                {
                    // On successful checkin
                    if (e.message.WasSuccessful == true)
                    {
                        _canBeModified = false;
                    }
                });
            }
        }

        public void CheckOut()
        {
            if (CanBeModified == false)
            {
                Operations.CheckoutVSGraph(Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
                    {
                        // On successful checkout
                        if (e.message.WasSuccessful == true)
                        {
                            _canBeModified = true;
                        }
                    });
            }
        }

        // Deprecated function
        public void CopyFromOtherGraph(FlowVSGraph input){
            Debug.LogError("Input in copy function: " + JsonUtility.ToJson(input));
            Name = input.name;
            serializedNodes = input.serializedNodes;
            edges = input.edges;
            groups = input.groups;
            stackNodes = input.stackNodes;
            pinnedElements = input.pinnedElements;
            exposedParameters = input.exposedParameters;
            stickyNotes = input.stickyNotes;
            position = input.position;
            scale= input.scale;
        }

    }
}