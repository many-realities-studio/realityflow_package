using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    // https://docs.unity3d.com/ScriptReference/SerializeField.html
    [Serializable]
    public class ConfigurationSingleton : ScriptableObject
    {
        private static ConfigurationSingleton _SingleInstance = null;
        public static ConfigurationSingleton SingleInstance 
        { 
            get => _SingleInstance is null ? _SingleInstance = new ConfigurationSingleton() : _SingleInstance; 
            set => _SingleInstance = value; 
        }
 
        [SerializeField]
        private FlowProject s_currentProject;

        [SerializeField]
        private FlowUser s_currentUser;

        public FlowProject CurrentProject { get => s_currentProject; set => s_currentProject = value; }
        public FlowUser CurrentUser { get => s_currentUser; set => s_currentUser = value; }
    }
}
