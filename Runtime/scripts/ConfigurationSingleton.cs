﻿using RealityFlow.Plugin.Scripts;
using System;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Packages.realityflow_package.Runtime.scripts
{
    // https://docs.unity3d.com/ScriptReference/SerializeField.html
    [Serializable]
    public class ConfigurationSingleton : ScriptableObject
    {
        private static ConfigurationSingleton _SingleInstance = null;
        public static void SetConfigurationSingletonUser(FlowUser fu) {
            _SingleInstance = ScriptableObject.CreateInstance<ConfigurationSingleton>();
            _SingleInstance.s_currentUser = fu;
        }
        public static ConfigurationSingleton SingleInstance
        {
            get {
                if(_SingleInstance is null) {
                    _SingleInstance = (ConfigurationSingleton)JsonConvert.DeserializeObject(EditorPrefs.GetString("currentProject"), typeof(ConfigurationSingleton));
                } 
                return _SingleInstance;
            } set
            { 
                EditorPrefs.SetString("currentProject", 
                JsonConvert.SerializeObject(value, 
                    new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                _SingleInstance = value; 
            }
        }

        [SerializeField]
        private FlowProject s_currentProject;

        [SerializeField]
        private FlowUser s_currentUser;

        public FlowProject CurrentProject { get => s_currentProject; set => s_currentProject = value; }
        public FlowUser CurrentUser { get => s_currentUser; set => s_currentUser = value; }
    }
}