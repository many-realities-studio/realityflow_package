using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Managers
{
    public class NewObjectManager
    {
        public static Dictionary<string, GameObject> idToGameObjectMapping = new Dictionary<string, GameObject>();
        public static GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        public static void CreateObjectInUnity(FlowTObject flowObject)
        {
            GameObject newGameObject = new GameObject();

            flowObject.SavePropertiesToGameObject(newGameObject);
            idToGameObjectMapping.Add(flowObject.flowId, newGameObject);
        }

        public static void EditObject(FlowTObject newValues)
        {
            //GameObject unityGameObject = idToGameObjectMapping[newValues.id];

            GameObject unityGameObject = idToGameObjectMapping[newValues.flowId];

            newValues.SavePropertiesToGameObject(unityGameObject);

            //unityGameObject.transform.position = Sphere.transform.position;

            //GameObject flowGameObject = Sphere;

            // Sets all properties of the unityGameObject to the new values defined 
            // by the flowGameObject
            //PropertyInfo[] properties = typeof(GameObject).GetProperties();
            //foreach (PropertyInfo property in properties)
            //{
            //    if(property.PropertyType == typeof(Transform))
            //    {
            //        property.SetValue(flowGameObject.transform.position, unityGameObject.transform.position);
            //    }
            //}
        }
    }
}
