using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts;
using RealityFlow.Plugin.Contrib;
//using Packages.realityflow_package.Runtime.scripts.Managers;
using System;
using UnityEngine;
using GraphProcessor;

namespace RealityFlow.Plugin.Scripts
{
    [System.Serializable]
    public class FlowVSGraph : BaseGraph
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public string Id; //{ get => _id; set => _id = value; }

        // [SerializeField]
        // private string _id = Guid.NewGuid().ToString();

        // public FlowVSGraph(string n) : base(){
        public FlowVSGraph(string n){
            this.Name = n;
            // this.Id = _id;
            this.Id = Guid.NewGuid().ToString();
            // base.AddNode(BaseNode.CreateFromType<FloatNode> (new Vector2 ()));
        }

        [JsonConstructor]
        // public FlowVSGraph(string id, string name) : base(){
        public FlowVSGraph(string id, string name) {
            Name = name;
            Id = id;
        }

    }

}