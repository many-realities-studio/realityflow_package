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
                             UnityEngine.Object prefabReference = Resources.Load("prefabs/VRWhiteBoard");
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
            AttachedGameObject.transform.GetChild(2).GetComponent<RealityFlowGraphView>().InitializeGraph(this);
            // base.AddNode(BaseNode.CreateFromType<FloatNode> (new Vector2 ()));
        }

        [JsonConstructor]
        // public FlowVSGraph(string id, string name) : base(){
        public FlowVSGraph(string id, string name) {
            Name = name;
            Id = id;
            WhiteboardManager.AddNewGraphToDict(this);

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
            }
        }

        public void UpdateFlowVSGraphGlobally(FlowVSGraph newValues)
        {
            if (AttachedGameObject.transform.hasChanged == true)
            {
                bool tempCanBeModified = this.CanBeModified;
                CopyFromOtherGraph(newValues);
                this.CanBeModified = tempCanBeModified;

                if (CanBeModified == true)
                {
                    Operations.UpdateVSGraph(this, ConfigurationSingleton.SingleInstance.CurrentUser, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {/* Debug.Log(e.message);*/ });
                }

                AttachedGameObject.transform.hasChanged = false;
            }
        }

        private void UpdateFlowVSGraphLocally(FlowVSGraph newValues)
        {
            if (idToVSGraphMapping[newValues.Id].CanBeModified == false)
            {
                bool tempCanBeModified = this.CanBeModified;
                CopyFromOtherGraph(newValues);
                this.CanBeModified = tempCanBeModified;
            }
        }

        public void CopyFromOtherGraph(FlowVSGraph input){
            this.Name = input.name;
            this.serializedNodes = input.serializedNodes;
            this.edges = input.edges;
            this.groups = input.groups;
            this.stackNodes = input.stackNodes;
            this.pinnedElements = input.pinnedElements;
            this.exposedParameters = input.exposedParameters;
            this.stickyNotes = input.stickyNotes;
            this.position = input.position;
            this.scale= input.scale;
        }

    }
}