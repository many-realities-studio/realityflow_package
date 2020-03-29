using UnityEngine;
using RealityFlow.Plugin.Scripts;
using System.Runtime.Serialization;
using System.Reflection;
using Packages.realityflow_package.Runtime.scripts;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowTObject : FlowValue
    {
        public static Dictionary<string, FlowTObject> idToGameObjectMapping = new Dictionary<string, FlowTObject>();

        public bool CanBeModified { get => _canBeModified; set => _canBeModified = value; }

        [JsonProperty("Id")]
        public string Id { get => _id; set => _id = value; }

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
                    if (idToGameObjectMapping.ContainsKey(Id))
                    {
                        _AttachedGameObject = idToGameObjectMapping[Id]._AttachedGameObject;
                    }

                    // The game object doesn't exist, but it should by this point 
                    // Can happen when a client receives a create object request when another user created an object
                    else
                    {
                        //UnityEngine.Object prefabReference = Resources.Load("Assets/resources/cube.prefab");
                        //_AttachedGameObject = GameObject.Instantiate(prefabReference) as GameObject;
                        _AttachedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }
                }
                return _AttachedGameObject;
            }

            set { _AttachedGameObject = value; }
        }

        private Color _ObjectColor;
        private bool _outgoingRequest;

        [SerializeField]
        private string _id = Guid.NewGuid().ToString();

        [JsonIgnore]
        public Color ObjectColor
        {
            get => _ObjectColor;
            set
            {
                _ObjectColor = new Color(value.r, value.g, value.b, value.a);
                R = value.r;
                G = value.g;
                B = value.b;
                A = value.a;
            }
        }

        [SerializeField]
        private float _X;

        [SerializeField]
        private bool _canBeModified;

        [JsonProperty("X")]
        public float X
        {
            get
            {
                _X = AttachedGameObject.transform.localPosition.x;
                return _X;
            }
            set
            {
                _X = value;
                AttachedGameObject.transform.localPosition = new Vector3(value, Y, Z);
            }
        }

        [JsonProperty("Y")]
        public float Y
        {
            get => AttachedGameObject.transform.localPosition.y;
            set => AttachedGameObject.transform.localPosition = new Vector3(X, value, Z);
        }

        [JsonProperty("Z")]
        public float Z
        {
            get => AttachedGameObject.transform.localPosition.z;
            set => AttachedGameObject.transform.localPosition = new Vector3(X, Y, value);
        }

        [JsonProperty("Q_x")]
        public float Q_x
        {
            get => AttachedGameObject.transform.localRotation.x;
            set => AttachedGameObject.transform.localRotation = new Quaternion(value, Q_y, Q_z, Q_w);
        }

        [JsonProperty("Q_y")]
        public float Q_y
        {
            get => AttachedGameObject.transform.localRotation.y;
            set => AttachedGameObject.transform.localRotation = new Quaternion(Q_x, value, Q_z, Q_w);
        }

        [JsonProperty("Q_z")]
        public float Q_z
        {
            get => AttachedGameObject.transform.localRotation.z;
            set => AttachedGameObject.transform.localRotation = new Quaternion(Q_x, Q_y, value, Q_w);
        }

        [JsonProperty("Q_w")]
        public float Q_w
        {
            get => AttachedGameObject.transform.localRotation.w;
            set => AttachedGameObject.transform.localRotation = new Quaternion(Q_x, Q_y, Q_z, value);
        }

        [JsonProperty("S_x")]
        public float S_x
        {
            get => AttachedGameObject.transform.localScale.x;
            set => AttachedGameObject.transform.localScale = new Vector3(value, S_y, S_z);
        }

        [JsonProperty("S_y")]
        public float S_y
        {
            get => AttachedGameObject.transform.localScale.y;
            set => AttachedGameObject.transform.localScale = new Vector3(S_x, value, S_z);
        }

        [JsonProperty("S_z")]
        public float S_z
        {
            get => AttachedGameObject.transform.localScale.z;
            set => AttachedGameObject.transform.localScale = new Vector3(S_x, S_y, value);
        }

        [JsonProperty("R")]
        public float R
        {
            get => ObjectColor.r;
            set
            {
                if (_ObjectColor == null)
                    _ObjectColor = new Color();
                _ObjectColor.r = value;
            }
        }

        [JsonProperty("G")]
        public float G
        {
            get => ObjectColor.g;
            set
            {
                if (_ObjectColor == null)
                    _ObjectColor = new Color();
                _ObjectColor.g = value;
            }
        }

        [JsonProperty("B")]
        public float B
        {
            get => ObjectColor.b;
            set
            {
                if (_ObjectColor == null)
                    _ObjectColor = new Color();
                _ObjectColor.b = value;
            }
        }

        [JsonProperty("A")]
        public float A
        {
            get => ObjectColor.a;
            set
            {
                if (_ObjectColor == null)
                    _ObjectColor = new Color();
                _ObjectColor.a = value;
            }
        }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonIgnore]
        public Vector3 Position
        {
            get => new Vector3(X, Y, Z);
            set
            {
                X = value.x;
                Y = value.y;
                Z = value.z;
            }
        }
        [JsonIgnore]
        public Vector3 Scale
        {
            get => new Vector3(S_x, S_y, S_z);
            set
            {
                S_x = value.x;
                S_y = value.y;
                S_z = value.z;
            }
        }

        [JsonIgnore]
        public Quaternion Rotation
        {
            get => new Quaternion(Q_x, Q_y, Q_z, Q_w);
            set
            {
                Q_x = value.x;
                Q_y = value.y;
                Q_z = value.z;
                Q_w = value.w;
            }
        }

        private string _Prefab;

        public string Prefab
        {
            get { return _Prefab; }
            set { _Prefab = value; }
        }


        public static void DestroyObject(string idOfObjectToDestroy)
        {
            try
            {
                GameObject objectToDestroy = idToGameObjectMapping[idOfObjectToDestroy].AttachedGameObject;
                idToGameObjectMapping.Remove(idOfObjectToDestroy);
                UnityEngine.Object.Destroy(objectToDestroy);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        //private void OnValidate()
        //{
        //   Transform t;
        //   this.TryGetComponent<Transform>(out t);

        //    if(t.hasChanged == true)
        //    {
        //        Position = t.position;
        //        Rotation = t.rotation;
        //        Scale = t.localScale;
        //    }

        //    Operations.UpdateObject(this, ConfigurationSingleton.CurrentUser, ConfigurationSingleton.CurrentProject.Id, (_, e) => Debug.Log("Update object received"));
        //}

        public FlowTObject(string name, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            this.Name = name;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.ObjectColor = color;
            this.Prefab = "Cube";

            idToGameObjectMapping.Add(Id, this);
            AttachedGameObject.transform.hasChanged = false;
            AttachedGameObject.name = name;

            AttachedGameObject.AddComponent<FlowObject_Monobehaviour>();
            var monoBehaviour = AttachedGameObject.GetComponent<FlowObject_Monobehaviour>();

            monoBehaviour.underlyingFlowObject = this;
        }

        [JsonConstructor]
        public FlowTObject(string id, float x, float y, float z, float q_x, float q_y, float q_z, float q_w, float s_x, float s_y, float s_z, float r, float g, float b, float a, string name)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
            Q_x = q_x;
            Q_y = q_y;
            Q_z = q_z;
            Q_w = q_w;
            S_x = s_x;
            S_y = s_y;
            S_z = s_z;
            R = r;
            G = g;
            B = b;
            A = a;
            Name = name;

            this.Prefab = "Cube";

            if(idToGameObjectMapping.ContainsKey(id))
            {
                idToGameObjectMapping[id].UpdateObjectLocally(this);
            }
            else // Create game object if it doesn't exist
            {
                idToGameObjectMapping.Add(Id, this);
                AttachedGameObject.name = name;
                AttachedGameObject.AddComponent<FlowObject_Monobehaviour>();
                AttachedGameObject.transform.hasChanged = false;

                var monoBehaviour = AttachedGameObject.GetComponent<FlowObject_Monobehaviour>();
                monoBehaviour.underlyingFlowObject = this;
            }
        }

        /// <summary>
        /// saves the current flowTObject properties to a Unity GameObject
        /// </summary>
        /// <param name="gameObject">GameObject that the information is to be saved to</param>
        public void SavePropertiesToGameObject(GameObject gameObject)
        {
            // TODO: add all properties to the conversion
            gameObject.gameObject.transform.position = new Vector3(X, Y, Z);
            gameObject.gameObject.transform.rotation = new Quaternion(Q_x, Q_y, Q_z, Q_w);
            gameObject.gameObject.transform.localScale = new Vector3(S_x, S_y, S_z);
        }

        /// <summary>
        /// Copy all new values into this object
        /// </summary>
        /// <param name="newValues">The values that should be copied over into this object</param>
        public void UpdateObjectGlobally(FlowTObject newValues)
        {
            if (AttachedGameObject.transform.hasChanged == true)
            {
                bool tempCanBeModified = this.CanBeModified;
                PropertyCopier<FlowTObject, FlowTObject>.Copy(newValues, this);
                this.CanBeModified = tempCanBeModified;

                if (CanBeModified == true)
                {
                    Operations.UpdateObject(this, ConfigurationSingleton.CurrentUser, ConfigurationSingleton.CurrentProject.Id, (_, e) => { Debug.Log(e.message); });
                }

                AttachedGameObject.transform.hasChanged = false;
            }
        }

        private void UpdateObjectLocally(FlowTObject newValues)
        {
            bool tempCanBeModified = this.CanBeModified;
            PropertyCopier<FlowTObject, FlowTObject>.Copy(newValues, this);
            this.CanBeModified = tempCanBeModified;
        }

        public void CheckIn()
        {
            if (CanBeModified == true)
            {
                Operations.CheckinObject(Id, ConfigurationSingleton.CurrentProject.Id, (_, e) =>
                {
                    // On successful checkin
                    if (e.message.WasSuccessful == true)
                    {
                        _canBeModified = false;
                        Debug.Log("Setting to false");
                    }
                    else
                    {
                        _canBeModified = true;
                        Debug.Log("Setting to true");
                    }
                    Debug.Log(e.message.WasSuccessful);
                    Debug.Log("Can be modified2 = " + CanBeModified);
                });
            }
        }

        public void CheckOut()
        {
            if (CanBeModified == false)
            {
                Operations.CheckoutObject(Id, ConfigurationSingleton.CurrentProject.Id, (_, e) =>
                    {
                // On successful checkout
                if (e.message.WasSuccessful == true)
                        {
                            _canBeModified = true;
                            Debug.Log("Setting to true");
                        }
                        else
                        {
                            _canBeModified = false;
                            Debug.Log("Setting to false");
                        }
                        Debug.Log(e.message.WasSuccessful);
                        Debug.Log("Can be modified1 = " + _canBeModified);
                    }); 
            }
        }
        
        public static void RemoveAllObjectsFromScene()
        {
            foreach (FlowTObject flowObject in idToGameObjectMapping.Values)
            {
                UnityEngine.Object.DestroyImmediate(flowObject.AttachedGameObject);
                FlowTObject.idToGameObjectMapping = new Dictionary<string, FlowTObject>();
            }
        }

        //public bool Equals(FlowTObject fo)
        //{
        //    if (x != fo.x || y != fo.y || z != fo.z ||
        //       q_x != fo.q_x || q_y != fo.q_y || q_z != fo.q_z ||
        //       s_x != fo.s_x || s_y != fo.s_y || s_z != fo.s_z ||
        //       color != fo.color)
        //        return false;

        //    return true;
        //}

        public void RegisterTransform()
        {
            //FlowProject.activeProject.transformsById.Add(_id, this);
        }
    }
}
