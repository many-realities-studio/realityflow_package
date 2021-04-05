using Newtonsoft.Json;
using System.Collections.Generic;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Contrib;
using System.Linq;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System;
using System.Runtime.Serialization;
using UnityEngine;
using GraphProcessor;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowVSGraph : BaseGraph
    {
        [SerializeField]
        public static SerializableDictionary<string, FlowVSGraph> idToVSGraphMapping = new SerializableDictionary<string, FlowVSGraph>();

        [SerializeField]
        public SerializableDictionary<string, string> paramIdToObjId = new SerializableDictionary<string, string>();

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
                        UnityEngine.Object prefabReference = Resources.Load("prefabs/FlowVSGraph");
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
        public FlowVSGraph(string id, string name, List<JsonElement> SerializedNodes, List<SerializableEdge> Edges, 
                          List<Group> Groups, List<BaseStackNode> StackNodes, List<PinnedElement> PinnedElements, 
                          string ExposedParameters, List<StickyNote> StickyNotes, Vector3 Position,
                          Vector3 Scale, string ParamIdToObjId) {
            Name = name;
            Id = id;
            this.serializedNodes = SerializedNodes;
            edges = Edges;

            foreach(SerializableEdge edge in edges){
                edge.owner = (BaseGraph)this;
            }

            groups = Groups;
            stackNodes = StackNodes;
            pinnedElements = PinnedElements;

            // Use UpdateExposedParameter(string guid, object value) to update existing params if possible, use window button to check if serialized value from server
            //  is sufficient. If not, can use serialized value from newly created params maybe?
            // Recreate exposed parameters from scratch but preserve their GUIDs and type

            // TODO: Somehow get attached object data and reinsert to each parameter

            // var definition = new[] { new { name = "" } };
            // var paramList = JsonConvert.DeserializeAnonymousType(ExposedParameters, definition);
            // var exposedParamList = JsonConvert.DeserializeObject<dynamic[]>(ExposedParameters);
            var definition = new[] { new { name = "", guid = "", serializedValue = new { serializedType = "", serializedName = "", serializedValue = "" }, type = "" } };
            var exposedParamList = JsonConvert.DeserializeAnonymousType(ExposedParameters, definition);

            // Delete Exposed parameters that shouldn't exist anymore as the server does not know of their existence
            foreach (ExposedParameter expParam in exposedParameters)
            {
                foreach (var param in exposedParamList.ToList())
                {
                    if (expParam.guid.Equals(param.guid))
                    {
                        break;
                    }

                    this.RemoveExposedParameter(expParam.guid);
                }
            }

            paramIdToObjId = JsonUtility.FromJson<SerializableDictionary<string, string>>(ParamIdToObjId);
            Debug.Log("Id to Id dictionary after deserialization: " + paramIdToObjId.ToString());

            // Update graph to add new Exposed Parameters and update still existing ones.
            foreach (var param in exposedParamList.ToList())
            {
                ExposedParameter paramBuilder;
                Debug.Log(param);
                // First check if each exposed parameter exists in the graph already
                if ((paramBuilder = GetExposedParameterFromGUID(param.guid)) != null)
                {
                    // this.UpdateExposedParameter(param.guid, new SerializableObject());
                    // param.serializedValue = new SerializableObject(setValue,typeof(object),null);

                    // Update the value of an exposed parameter by setting the serialized values within its SerializableObject using data received from the server
                    if (Type.GetType(param.type).ToString().Equals("UnityEngine.GameObject"))
                    {
                        // Get the object we want to attach to the parameter from the FlowTObject dictionary
                        string newObjGuid = paramIdToObjId[param.guid];
                        FlowTObject updatedFlowTObject = FlowTObject.idToGameObjectMapping[newObjGuid];
                        GameObject newAttachedGameObj = updatedFlowTObject.AttachedGameObject;

                        // paramBuilder.serializedValue.serializedType = param.serializedValue.serializedType;
                        paramBuilder.serializedValue.serializedName = param.serializedValue.serializedName;
                        paramBuilder.serializedValue.value = newAttachedGameObj;
                    }
                    else
                    {
                        paramBuilder.serializedValue.serializedType = param.serializedValue.serializedType;
                        paramBuilder.serializedValue.serializedName = param.serializedValue.serializedName;
                        paramBuilder.serializedValue.serializedValue = param.serializedValue.serializedValue;
                        paramBuilder.serializedValue.OnAfterDeserialize();
                    }

                    // switch(Type.GetType(param.type).ToString())
		            // {
                    //     case "UnityEngine.Color":
                    //         var def = new { r = new float(), g = new float(), b = new float(), a = new float() };
                    //         var colorObj = JsonConvert.DeserializeAnonymousType(param.serializedValue.serializedValue, def);

                    //         Color newColor = new Color (colorObj.r, colorObj.g, colorObj.b, colorObj.a);

                    //         this.AddExposedParameter(param.name, Type.GetType(param.type), newColor);
                    //         break;
                    //     default:
                    //         Debug.Log("Could not identify exposed parameter type in VSGraph constructor");
                    //         break;
                    // }
                }
                else // Exposed parameter does not yet exist, we will have to create it using received data
                {
                    // this.AddExposedParameter(param.name, Type.GetType(param.type), param.serializedValue.serializedValue);
                    //string paramtype = Type.GetType(param.type).ToString();

                    // TODO: This will need to be fixed to work with objects as a special case.
                    if (Type.GetType(param.type).ToString().Equals("UnityEngine.GameObject"))
                    {
                        // Get the object we want to attach to the parameter from the FlowTObject dictionary
                        string newObjGuid = paramIdToObjId[param.guid];
                        FlowTObject updatedFlowTObject = FlowTObject.idToGameObjectMapping[newObjGuid];
                        GameObject newAttachedGameObj = updatedFlowTObject.AttachedGameObject;

                        exposedParameters.Add(new ExposedParameter{
                            guid = param.guid,
                            name = param.name,
                            type = param.type,
                            settings = new ExposedParameterSettings(),
                            serializedValue = new SerializableObject(newAttachedGameObj,typeof(GameObject),null)
                        });
                    }
                    else
                    {
                        SerializableObject emptyObj = (SerializableObject)FormatterServices.GetUninitializedObject(typeof(SerializableObject));
                        emptyObj.serializedType = param.serializedValue.serializedType;
                        emptyObj.serializedName = param.serializedValue.serializedName;
                        emptyObj.serializedValue = param.serializedValue.serializedValue;
                        emptyObj.OnAfterDeserialize();

                        exposedParameters.Add(new ExposedParameter{
                            guid = param.guid,
                            name = param.name,
                            type = param.type,
                            settings = new ExposedParameterSettings(),
                            serializedValue = emptyObj
                        });
                    }
                }
            }

            // This turned out to be pointless:
            // foreach(var serializedNode in serializedNodes.ToList()) // put this in check to see if graph is already in dictionary
            // {
            //     Debug.Log("Comparing node types: " + Type.GetType(serializedNode.type) + " and GraphProcessor.ParameterNode");
            //     Debug.Log(Type.GetType(serializedNode.type).Equals("GraphProcessor.ParameterNode"));
            //     // Check if the node is a ParameterNode.
            //     if (Type.GetType(serializedNode.type).ToString().Equals("GraphProcessor.ParameterNode"))
            //     {
            //         var definition = new { parameterGUID = "" };
            //         Debug.Log("Found a parameter node in serializedNodes");
            //         // ParameterNode nodeJsonData = JsonUtility.FromJson<ParameterNode>(serializedNode.jsonDatas);
            //         var foundNodeParameterGUID = JsonConvert.DeserializeAnonymousType(serializedNode.jsonDatas, definition);
            //         // First check to see if exposed parameter already exists in the graph.
            //         // Debug.Log("node jsondatas: " + nodeJsonData.ToString());
            //         // Debug.Log("serializedNode.jsonDatas: " + serializedNode.jsonDatas);
            //         Debug.Log("can we access parameter GUID? " + foundNodeParameterGUID.parameterGUID); // YES
                    
                    
            //         // ExposedParameter checkParam = this.GetExposedParameterFromGUID()

            //         // this.AddExposedParameter (paramList[paramListIndex].name, Type.GetType(paramList[paramListIndex].type), value); // Finish upper TODO
            //         // paramListIndex++;
            //     }
            // }
            //exposedParameters = ExposedParameters;
            
            stickyNotes = StickyNotes;
            position = Position;
            scale = Scale;
            // WhiteboardManager.AddNewGraphToDict(this);

            if (idToVSGraphMapping.ContainsKey(id))
            {
                idToVSGraphMapping[id].UpdateFlowVSGraphLocally(this);
            }
            else // Create graph object if it doesn't exist
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

            Debug.Log("serializedNodes as list in jsonconstructor:");
            foreach (var serializedNode in serializedNodes.ToList())
            {
                Debug.Log(serializedNode);
                var baseNodeType = Type.GetType(serializedNode.type);
                Debug.Log("Type of node: " + baseNodeType);
            }

            Deserialize();

            // Disable nodes correctly before removing them:
			// if (nodes != null)
			// {
			// 	foreach (var node in nodes)
			// 		node.DisableInternal();
			// }

			// nodes.Clear();

			// foreach (var serializedNode in serializedNodes.ToList())
			// {
			// 	var node = GraphProcessor.JsonSerializer.DeserializeNode(serializedNode) as BaseNode;
			// 	if (node == null)
			// 	{
			// 		serializedNodes.Remove(serializedNode);
			// 		continue ;
			// 	}
			// 	AddNode(node);
			// 	nodesPerGUID[node.GUID] = node;
			// }

            // Debug.Log("this.serializedNodes as list in jsonconstructor:");
            // foreach (var serializedNode in this.serializedNodes.ToList())
            // {
            //     Debug.Log(serializedNode);
            // }


            // Debug.Log("Edges as list in jsonconstructor:");
            // foreach (var edge in edges.ToList())
            // {
            //     Debug.Log(edge);
            // }
            // Debug.Log("VSGraph after json constructor: " + JsonUtility.ToJson(this));
            // Debug.Log("BaseGraph after json constructor: " + JsonUtility.ToJson((BaseGraph)this));
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