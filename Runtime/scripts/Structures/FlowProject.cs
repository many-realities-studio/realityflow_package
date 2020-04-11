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
    public class FlowProject
    {
        [JsonProperty("Id")]
        public string Id { get; set; } // The unique ID of the project

        [JsonProperty("Description")]
        public string Description { get; set; } // Description of the project

        [JsonProperty("DateModified")]
        public int DateModified { get; set; } // The last time this project was modified

        [JsonProperty("ProjectName")]
        public string ProjectName { get; set; } // Name of the project

        [JsonProperty("_ObjectList")]
        public IEnumerable<FlowTObject> _ObjectList { get; set; }

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
