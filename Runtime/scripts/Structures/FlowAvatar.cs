//using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Contrib;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowAvatar
    {
        [JsonIgnore]
        public bool currentAvatarIsMe = false;

        [SerializeField]
        public static SerializableDictionary<string, FlowAvatar> idToAvatarMapping = new SerializableDictionary<string, FlowAvatar>();

        public bool CanBeModified { get => _canBeModified; set => _canBeModified = value; }

        [JsonProperty("Id")]
        // Is this actually the user id from the server or just a random
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
                    if (idToAvatarMapping.ContainsKey(Id))
                    {
                        if (idToAvatarMapping[Id]._AttachedGameObject == null)
                        {
                            UnityEngine.Object prefabReference = Resources.Load("prefabs/Avatar");
                            if (prefabReference == null)
                            {
                                Debug.Log("cannot load prefab");
                            }
                            idToAvatarMapping[Id]._AttachedGameObject = GameObject.Instantiate(prefabReference) as GameObject;
                            
                            
                        }
                        _AttachedGameObject = idToAvatarMapping[Id]._AttachedGameObject;
                    }

                    // The game object doesn't exist, but it should by this point
                    // Can happen when a client receives a create object request when another user created an object
                    else
                    {
                        UnityEngine.Object prefabReference = Resources.Load("prefabs/Avatar");
                        if (prefabReference == null)
                        {
                            Debug.Log("cannot load prefab");
                        }
                        _AttachedGameObject = GameObject.Instantiate(prefabReference) as GameObject;
                        //Debug.Log("we are running on update!!!!!");
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

        // [JsonIgnore]
        // public Vector3 Scale
        // {
        //     get => new Vector3(S_x, S_y, S_z);
        //     set
        //     {
        //         S_x = value.x;
        //         S_y = value.y;
        //         S_z = value.z;
        //     }
        // }

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

        [SerializeField]
        /* private string _Prefab;

        public string Prefab
        {
            get { return _Prefab; }
            set { _Prefab = value; }
        } */

        // public static void DestroyObject(string idOfObjectToDestroy)
        public static void DestroyAvatar(string idOfAvatarToDestroy)
        {
            try
            {
                GameObject avatarToDestroy = idToAvatarMapping[idOfAvatarToDestroy].AttachedGameObject;
                idToAvatarMapping.Remove(idOfAvatarToDestroy);
                UnityEngine.Object.Destroy(avatarToDestroy);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        // public FlowAvatar(string name, Vector3 position, Quaternion rotation, Color color, string ObjectPrefab)
        public FlowAvatar(/*string name,*/ Transform head /*, Transform lHand, Transform rHand */)
        {
            // Debug.Log("prefab is " + Prefab);
            // this.Name = name;
            currentAvatarIsMe = true;
            this.Position = head.position; 
            this.Rotation = head.rotation;
            
            // head.gameObject.AddComponent<FlowAvatar_Monobehaviour>();
            // FlowAvatar_Monobehaviour monoBehaviour = head.gameObject.GetComponent<FlowAvatar_Monobehaviour>(); /* TBD */

            
            // this.Scale = head.scale;
            // this.Position = position;
            // this.Rotation = rotation;
            //this.Scale = scale;
            // this.ObjectColor = color;

            // Not sure if it should be idToGameObjectMapping ...
            idToAvatarMapping.Add(Id, this);
            AttachedGameObject.transform.hasChanged = false;
            AttachedGameObject.AddComponent<FlowAvatar_Monobehaviour>();

            FlowAvatar_Monobehaviour monoBehaviour = AttachedGameObject.GetComponent<FlowAvatar_Monobehaviour>();
            monoBehaviour.underlyingFlowAvatar = this;
            monoBehaviour.head = head.gameObject; // Cannot implicitly convert type 'UnityEngine.Transform' to 'UnityEngine.GameObject'
            // AttachedGameObject.name = name;
            // AttachedGameObject.layer = 9;
            idToAvatarMapping[Id]._AttachedGameObject.transform.SetParent(GameObject.Find("Main Camera").transform);
            

            //monoBehav (John)

            // AttachedGameObject.AddComponent<FlowAvatar_Monobehaviour>();
            //ObjectManipulator om = AttachedGameObject.AddComponent<ObjectManipulator>();
            // om.OnManipulationEnded += SendGORefToParam;
            //AttachedGameObject.AddComponent<NearInteractionGrabbable>();
            //Rigidbody ObjectRigidBody = AttachedGameObject.AddComponent<Rigidbody>();
            // ObjectRigidBody.useGravity = false;
            // ObjectRigidBody.isKinematic = true;
            
            

            // monoBehaviour.underlyingFlowObject = this;
        } 

        [JsonConstructor]
        // public FlowAvatar(string id, float x, float y, float z, float q_x, float q_y, float q_z, float q_w, float s_x, float s_y, float s_z, float r, float g, float b, float a, string name, string prefab)
        public FlowAvatar(string id, float x, float y, float z, float q_x, float q_y, float q_z, float q_w/* , float s_x, float s_y, float s_z, float r, float g, float b, float a, string name *//* , string prefab */)
        {
            Id = id;
            // this.Prefab = prefab;
            X = x;
            Y = y;
            Z = z;
            Q_x = q_x;
            Q_y = q_y;
            Q_z = q_z;
            Q_w = q_w;
            // S_x = s_x;
            // S_y = s_y;
            // S_z = s_z;
            // R = r;
            // G = g;
            // B = b;
            // A = a;
            // Name = name;
            /*
            
            "MessageType":"CreateAvatar","FlowAvatar":{"Id":"86920863-828b-4fb0-8007-bdd27db167ad","Name":null,"X":0,"Y":0,"Z":0,"Q_x":0,"Q_y":0,"Q_z":0,"Q_w":1,"S_x":1,"S_y":1,"S_z":1,"R":0,"G":0,"B":0,"A":0},"WasSuccessful":true,
            "AvatarList":[{"Id":"edcce338-e8d7-4f29-8e8c-587b29b0d4a1","Name":null,"X":0,"Y":0,"Z":0,"Q_x":0,"Q_y":0,"Q_z":0,"Q_w":1,"S_x":1,"S_y":1,"S_z":1,"R":0,"G":0,"B":0,"A":0},
            {"Id":"119f0875-47d9-424d-856c-658b1a45e7ee","Name":null,"X":0,"Y":0,"Z":0,"Q_x":0,"Q_y":0,"Q_z":0,"Q_w":1,"S_x":1,"S_y":1,"S_z":1,"R":0,"G":0,"B":0,"A":0},
            {"Id":"86920863-828b-4fb0-8007-bdd27db167ad","Name":null,"X":0,"Y":0,"Z":0,"Q_x":0,"Q_y":0,"Q_z":0,"Q_w":1,"S_x":1,"S_y":1,"S_z":1,"R":0,"G":0,"B":0,"A":0}]}
            
            */

            if (idToAvatarMapping.ContainsKey(id))
            {
                idToAvatarMapping[id].UpdateObjectLocally(this);
            }
            else // Create game object if it doesn't exist
            {
                idToAvatarMapping.Add(Id, this);
                //AttachedGameObject.name = name;
                // AttachedGameObject.AddComponent<FlowAvatar_Monobehaviour>();
                // AttachedGameObject.AddComponent<ObjectManipulator>();
                // AttachedGameObject.AddComponent<NearInteractionGrabbable>();
                AttachedGameObject.transform.hasChanged = false;
                
                //AttachedGameObject.layer = 9;

                //var monoBehaviour = AttachedGameObject.GetComponent<FlowAvatar_Monobehaviour>();
                //monoBehaviour.underlyingFlowObject = this;
                //var ObjectRigidBody = AttachedGameObject.AddComponent<Rigidbody>();
                //ObjectRigidBody.useGravity = false;
                //ObjectRigidBody.isKinematic = true;
            }
        }

        /// <summary>
        /// saves the current FlowAvatar properties to a Unity GameObject
        /// </summary>
        /// <param name="gameObject">GameObject that the information is to be saved to</param>
        public void SavePropertiesToGameObject(GameObject gameObject)
        {
            // TODO: add all properties to the conversion
            gameObject.gameObject.transform.position = new Vector3(X, Y, Z);
            gameObject.gameObject.transform.rotation = new Quaternion(Q_x, Q_y, Q_z, Q_w);
            //gameObject.gameObject.transform.localScale = new Vector3(S_x, S_y, S_z);
        }

        /// <summary>
        /// Copy all new values into this object
        /// </summary>
        /// <param name="newValues">The values that should be copied over into this object</param>
        public void UpdateObjectGlobally(FlowAvatar newValues)
        {
            if (AttachedGameObject.transform.hasChanged == true)
            {
                PropertyCopier<FlowAvatar, FlowAvatar>.Copy(newValues, this);

                Operations.UpdateAvatar(this, ConfigurationSingleton.SingleInstance.CurrentUser, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {/* Debug.Log(e.message);*/ });

                AttachedGameObject.transform.hasChanged = false;
            }
        }

        private void UpdateObjectLocally(FlowAvatar newValues)
        {
            if (idToAvatarMapping[newValues.Id].CanBeModified == false)
            {
                PropertyCopier<FlowAvatar, FlowAvatar>.Copy(newValues, this);
            }
        }

        public void CheckIn()
        {
            if (CanBeModified == true)
            {
                Operations.CheckinObject(Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
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
                Operations.CheckoutObject(Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
                    {
                        // On successful checkout
                        if (e.message.WasSuccessful == true)
                        {
                            _canBeModified = true;
                        }
                    });
            }
        }

        public static void RemoveAllAvatarFromScene()
        {
            // TODO: Remove the FlowAvatar_Monobehavior component from the main camera to avoid stacking
            // foreach (FlowAvatar flowAvatar in idToAvatarMapping.Values)
            // if it's the client's FA - only remove the component from main cam
            // for everyone else - delete the attach
            foreach (FlowAvatar flowAvatar in idToAvatarMapping.Values)
            {

                Debug.Log(JsonUtility.ToJson(flowAvatar));
                UnityEngine.Object.DestroyImmediate(flowAvatar.AttachedGameObject);
            }
            FlowAvatar.idToAvatarMapping = new SerializableDictionary<string, FlowAvatar>();
        }
    }
}