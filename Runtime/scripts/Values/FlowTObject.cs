using UnityEngine;
using RealityFlow.Plugin.Scripts;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowTObject : FlowValue
    {
        //public int[] Triangles;
        public string FlowId;
        public float X;
        public float Y;
        public float Z;
        public float Q_x;
        public float Q_y;
        public float Q_z;
        public float Q_w;
        public float S_x;
        public float S_y;
        public float S_z;
        public string Name;
        //public Vector2[] Uv;
        //public byte[] Texture;
        //public int TextureHeight;
        //public int TextureWidth;
        //public int TextureFormat;
        //public int MipmapCount;
        public Color Color; // Not serializable - look at old v1 code to find how

        [System.NonSerialized]
        public static int idCount = 0;
        [System.NonSerialized]
        public Transform transform;
        [System.NonSerialized]
        public Mesh mesh;

        public FlowTObject()
        {

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

        public FlowTObject(GameObject obj)
        {
            transform = obj.transform;
            X = transform.localPosition.x;
            Y = transform.localPosition.y;
            Z = transform.localPosition.z;
            Q_x = transform.localRotation.x;
            Q_y = transform.localRotation.y;
            Q_z = transform.localRotation.z;
            Q_w = transform.localRotation.w;
            S_x = transform.localScale.x;
            S_y = transform.localScale.y;
            S_z = transform.localScale.z;
            // mesh = 
            //triangles = mesh.triangles;
            //uv = mesh.uv;
            //vertices = mesh.vertices;
            //name = obj.name;
            //material = obj.GetComponent<MeshRenderer>().material;
            //color = material.color;


            // if (material.mainTexture != null)
            // {
            //     texture = ((Texture2D)material.mainTexture).GetRawTextureData();
            //     textureHeight = ((Texture2D)material.mainTexture).height;
            //     textureWidth = ((Texture2D)material.mainTexture).width;
            //     textureFormat = (int)((Texture2D)material.mainTexture).format;
            //     mipmapCount = ((Texture2D)material.mainTexture).mipmapCount;
            // }
            //type = "BoxCollider";
        }
        public void Read()
        {
            Y = transform.localPosition.y;
            X = transform.localPosition.x;
            Z = transform.localPosition.z;
            Q_x = transform.localRotation.x;
            Q_y = transform.localRotation.y;
            Q_z = transform.localRotation.z;
            Q_w = transform.localRotation.w;
            S_x = transform.localScale.x;
            S_y = transform.localScale.y;
            S_z = transform.localScale.z;
            //color = material.color;
            //vertices = mesh.vertices;
            //uv = mesh.uv;
            //triangles = mesh.triangles;
        }

        //public void Read(GameObject go)
        //{
        //    x = go.transform.localPosition.x;
        //    y = go.transform.localPosition.y;
        //    z = go.transform.localPosition.z;
        //    q_x = go.transform.localRotation.x;
        //    q_y = go.transform.localRotation.y;
        //    q_z = go.transform.localRotation.z;
        //    q_w = go.transform.localRotation.w;
        //    s_x = go.transform.localScale.x;
        //    s_y = go.transform.localScale.y;
        //    s_z = go.transform.localScale.z;
        //    material = go.GetComponent<MeshRenderer>().material;
        //    color = material.color;
        //    //Mesh newMesh = go.GetComponent<MeshFilter>().mesh;
        //    //vertices = newMesh.vertices;
        //    //uv = newMesh.uv;
        //    //triangles = newMesh.triangles;
        //    //name = go.name;
        //    //type = "BoxCollider";

        //}

        //public void Copy(FlowTObject source)
        //{
        //    x = source.x;
        //    y = source.y;
        //    z = source.z;
        //    q_x = source.q_x;
        //    q_y = source.q_y;
        //    q_z = source.q_z;
        //    q_w = source.q_w;
        //    s_x = source.s_x;
        //    s_y = source.s_y;
        //    s_z = source.s_z;
        //    _id = source._id;
        //    color = source.color;
        //    //vertices = source.mesh.vertices;
        //    //uv = source.mesh.uv;
        //    //triangles = source.mesh.triangles;
        //    //name = source.name;
        //    //type = "BoxCollider";
        //}

        //public bool Equals(FlowTObject fo)
        //{
        //    if (x != fo.x || y != fo.y || z != fo.z ||
        //       q_x != fo.q_x || q_y != fo.q_y || q_z != fo.q_z ||
        //       s_x != fo.s_x || s_y != fo.s_y || s_z != fo.s_z ||
        //       color != fo.color)
        //        return false;

        //    return true;
        //}

        public void Update()
        {
            //Vector3 newPos = new Vector3(x, y, z);
            //transform.localPosition = newPos;
            //Quaternion newRot = new Quaternion(q_x, q_y, q_z, q_w);
            //transform.localRotation = newRot;
            //Vector3 newScale = new Vector3(s_x, s_y, s_z);
            //transform.localScale = newScale;
            //material.color = color;
            //mesh.vertices = vertices;
            //mesh.uv = uv;
            //mesh.triangles = triangles;
            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();
        }

        public void RegisterTransform()
        {
            //FlowProject.activeProject.transformsById.Add(_id, this);
        }

        public FlowTObject(string _id)
        {
            flowId = _id;
            if (_id == null)
            {
                _id = idCount.ToString() + "t";
            }
        }

        //public FlowTObject(float _q_x, float _q_y, float _q_z, float _q_w)
        //{
        //    q_x = _q_x;
        //    q_y = _q_y;
        //    q_z = _q_z;
        //    q_w = _q_w;
        //}

        //public FlowTObject(float _x, float _y, float _z, string _id)
        //{
        //    x = _x;
        //    y = _y;
        //    z = _z;
        //}

        //public FlowTObject(float _s_x, float _s_y, float _s_z)
        //{
        //    s_x = _s_x;
        //    s_y = _s_y;
        //    s_z = _s_z;
        //}


    }
}
