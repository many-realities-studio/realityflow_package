using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace RealityFlow.Plugin.Scripts
{
    /// <summary>
    /// The purpose of this class is to hold all the information that is necessary to define a project.
    /// The information from this class gets serialized and sent to the server.
    /// Anything marked with a [DataMember] annotation is marked to be serialized and
    /// sent to the server.
    /// </summary>
    [Serializable]
    public class FlowProject
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private string _description;

        [SerializeField]
        private int _dateModified;

        [SerializeField]
        private string _projectName;

        [SerializeField]
        private IEnumerable<FlowTObject> _objectList;

        [JsonProperty("Id")]
        public string Id { get => _id; set => _id = value; } // The unique ID of the project

        [JsonProperty("Description")]
        public string Description { get => _description; set => _description = value; } // Description of the project

        [JsonProperty("DateModified")]
        public int DateModified { get => _dateModified; set => _dateModified = value; } // The last time this project was modified

        [JsonProperty("ProjectName")]
        public string ProjectName { get => _projectName; set => _projectName = value; } // Name of the project

        [JsonProperty("_ObjectList")]
        public IEnumerable<FlowTObject> _ObjectList { get => _objectList; set => _objectList = value; }

        public FlowProject(string flowId, string description, int dateModified, string projectName/*, IEnumerable<FlowTObject> ObjectList*/)
        {
            Id = flowId;
            Description = description;
            DateModified = dateModified;
            ProjectName = projectName;
            //_ObjectList = ObjectList;
        }

        //public int _uid;
        //public string description;
        //public int created;
        //public int dateModified;
        //public string projectName;
        //public List<FlowTObject> transforms;
        //[System.NonSerialized]
        //public Dictionary<string, FlowTObject> transformsById;
        //[System.NonSerialized]
        //public List<int> collaborators;
        //public List<int> activeConnections;
        //public bool initialized = false;
        ////public List<ProjectCommand> history;

        //[System.NonSerialized]
        //public List<GameObject> objs = new List<GameObject>();

        //[System.NonSerialized]
        //public static FlowProject activeProject;

        //public FlowProject(string id)
        //{
        //    _id = id;
        //}

        //public FlowProject()
        //{

        //}

        //public void initialize()
        //{
        //    activeProject = this;
        //    if (!initialized)
        //    {
        //        initialized = true;
        //        transformsById = new Dictionary<string, FlowTObject>();
        //        if (transforms == null)
        //            transforms = new List<FlowTObject>();
        //        for (int g = 0; g < transforms.Count; g++)
        //        {
        //            transformsById.Add(transforms[g]._id, transforms[g]);
        //        }
        //    }
        //    //FlowObject.registerObject();
        //}

        //public void RegObj()
        //{
        //    FlowObject.registerObject();
        //}
    }
}
