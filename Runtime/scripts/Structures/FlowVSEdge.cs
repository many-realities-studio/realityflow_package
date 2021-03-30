using Newtonsoft.Json;
using System.Collections.Generic;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Contrib;
using System.Linq;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System;
using UnityEngine;
using GraphProcessor;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowVSEdge : SerializableEdge
    {

        public void SetInputNodeGUID(string guid){
            base?.inputNodeGUID = guid;
        }

        public void SetInputNodeGUID(string guid){
            base?.outputNodeGUID = guid;
        }

        public void SetOwner(BaseGraph graph){
            base?.owner = graph;
        }
    }
}