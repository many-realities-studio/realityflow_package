//using RealityFlow.Plugin.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using RealityFlow.Plugin.Scripts;
using System.Threading.Tasks;
//using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
//using Packages.realityflow_package.Runtime.scripts;
namespace Contrib.APIeditor
{
    [CustomEditor(typeof(graphqlAPI))]
    public class graphqlClient_Editor : Editor
    {
        
        private graphqlAPI graphql_Var;
        
        private void OnEnable()
        {
            //public GameObject graphqlObj = GameObject.FindWithTag("GraphQL");
            // We automatically have a reference to a "graphAPI" object, named "target"
            // inside the OnInspectorGUI method. This isn't usable until we cast it to an appropriatly typed variable
            // (graphqlAPI) target; 
            graphql_Var = (graphqlAPI) GameObject.FindWithTag("GraphQL").GetComponent<graphqlAPI>();
        }
        //graphql_Var = GameObject.Find("GraphQLClient").GetComponent<graphql_api>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            // QUERY
            if (GUILayout.Button("Get Users")){
                GetUsers();
            }

            EditorGUILayout.Space();
        }
        
        public async void GetUsers(){
            // To use "UnityWebRequest" we need to be using UnityEngine.Networking;
            UnityWebRequest request = await graphql_Var.graphql_api.Post("GetUsers", GraphApi.Query.Type.Query);
        }


        public async Task<string> CreateBehaviour(FlowBehaviour behaviour, string projectId, List<string> behavioursToLinkTo)
        {
            string _behaviour;
            string message;

            if(behavioursToLinkTo.Count == 0){
                _behaviour = "[]";
            }else
            {
                _behaviour = behavioursToLinkTo[0];
            }
            //Gets the needed query from the Api Reference
            GraphApi.Query createBehaviour = graphql_Var.graphql_api.GetQueryByName("CreateBehaviour", GraphApi.Query.Type.Mutation);
            //Converts the JSON object to an argument string and sets the queries argument
            int opt = (behaviour.flowAction.ActionType == "Teleport") ? 1 : 2;
            Debug.Log("TRRRRRRUE! : " + opt);
            switch(opt) // Teleport and SnapZone check.
            {
                case 1:
                    // To shorten code
                    var sub = behaviour.flowAction.teleportCoordinates;
                    // {"teleportCoordinates":{"coordinates":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0,"w":1,"eulerAngles":{"x":0,"y":0,"z":0}},"scale":{"x":0,"y":0,"z":0},"IsSnapZone":false},"Id":"845ba106-0b8c-4a5d-a1fb-417588a8ae4c","ActionType":"Teleport"}
                    createBehaviour.SetArgs(new{Id = behaviour.Id, TypeOfTrigger = behaviour.TypeOfTrigger, TriggerObjectId = behaviour.TriggerObjectId, TargetObjectId = behaviour.TargetObjectId,
                                            ProjectId = projectId, NextBehaviour = _behaviour, Action = new{ teleportCoordinates = new{coordinates = new{x = (int)sub.coordinates.x, y = (int)sub.coordinates.y, z = (int)sub.coordinates.z},
                                                                                                            rotation = new{x = (int)sub.rotation.x, y = (int)sub.rotation.y, z = (int)sub.rotation.z, w = (int)sub.rotation.w, 
                                                                                                            eulerAngles = new{x = (int)sub.rotation.eulerAngles.x, y = (int)sub.rotation.eulerAngles.y, z = (int)sub.rotation.eulerAngles.z}},
                                                                                                            scale = new{x = (int)sub.scale.x, y = (int)sub.scale.y, z = (int)sub.scale.z}, IsSnapZone = sub.IsSnapZone},
                                                                                                            Id = behaviour.flowAction.Id, ActionType = behaviour.flowAction.ActionType}});
                    break;
                // Debug.Log("HMMMMMMMM...!");
                //Debug.Log("YOOOO FAISAL eulerAngles(y): " + sub.rotation.eulerAngles.y);
                // Debug.Log("YOOOO FAISAL Euler(z): " + sub.eulerAngles.z);
                case 2:
                    createBehaviour.SetArgs(new{Id = behaviour.Id, TypeOfTrigger = behaviour.TypeOfTrigger, TriggerObjectId = behaviour.TriggerObjectId, TargetObjectId = behaviour.TargetObjectId,
                                                ProjectId = projectId, NextBehaviour = _behaviour, Action = new{Id = behaviour.flowAction.Id, ActionType = behaviour.flowAction.ActionType}});
                    Debug.Log("TWO $ THE WIN!");
                    break;
            }
            //Performs Post request to server
            UnityWebRequest request = await graphql_Var.graphql_api.Post(createBehaviour);
            string json1 = request.downloadHandler.text;
        
            Match match = Regex.Match(json1, @"""Id"":""(\S+)""");
            message = match.Groups[1].Captures[0].Value;

             return message;
        }


        // public async void UpdateBehaviour(int behaviourId, string _TypeOfTrigger, string _TriggerObjectId,
        //                                     string _TargetObjectId, string _projectId, string _NextBehaviour, string _Action)
        // {
        //     GraphApi.Query UpdateBehaviour = graphql_Var.graphql_api.GetQueryByName("UpdateBehaviour", GraphApi.Query.Type.Mutation);
        //     UpdateBehaviour.SetArgs(new{id = behaviourId, TypeOfTrigger = _TypeOfTrigger, TriggerObjectId = _TriggerObjectId, TargetObjectId = _TargetObjectId,
        //                                                 ProjectId = _projectId, NextBehaviour = _NextBehaviour, Action = _Action});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(UpdateBehaviour);
        // }


        public async void DeleteBehaviour(List<string> behaviourIds, string projectId)
        {
            for (int i = 0; i < behaviourIds.Count; i++)
            {
                GraphApi.Query DeleteBehaviour = graphql_Var.graphql_api.GetQueryByName("DeleteBehaviour", GraphApi.Query.Type.Mutation);
                DeleteBehaviour.SetArgs(new{Id = behaviourIds[i]});
                UnityWebRequest request = await graphql_Var.graphql_api.Post(DeleteBehaviour);
            }
        }

        public async void CreateUser(string username, string password/*, string _description, string _projName*/)
        {
            GraphApi.Query CreateUser = graphql_Var.graphql_api.GetQueryByName("CreateUser", GraphApi.Query.Type.Mutation);
            var data = new{Username = username, Password = password, /*input = new{Description = _description, ProjectName = _projName}*/};
            CreateUser.SetArgs(data);
            UnityWebRequest request = await graphql_Var.graphql_api.Post(CreateUser);


        //     string responseText = request.downloadHandler.text;
        //     Then I want to parse the response to get the created playerId so that I can use it for subscriptions. [28:32]
        //     //, DateModified = currentTime
        }

        // public async void UpdateUser(string _userName, string _passWord, string _description, string _projName)
        // {
        //     GraphApi.Query UpdateUser = graphql_Var.graphql_api.GetQueryByName("UpdateUser", GraphApi.Query.Type.Mutation);
        //     var data = new{Username = _userName, Password = _passWord};
        //     UpdateUser.SetArgs(data);
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(UpdateUser);
        // }

        // // DELETE_USER>>> HERE..

        // public async void CreateProject(string _ProjectName, string _Description, string _ownerUsername){
        //     GraphApi.Query CreateProject = graphql_Var.graphql_api.GetQueryByName("CreateProject", GraphApi.Query.Type.Mutation);
        //     CreateProject.SetArgs(new{ProjectName = _ProjectName, Description = _Description, ownerUsername = _ownerUsername});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(CreateProject);     
        // }

        // public async void UpdateProject(string _ProjectId, string _ProjectName, string _Description, string _ownerUsername){
        //     GraphApi.Query UpdateProject = graphql_Var.graphql_api.GetQueryByName("UpdateProject", GraphApi.Query.Type.Mutation);
        //     UpdateProject.SetArgs(new{Id = _ProjectId, ProjectName = _ProjectName, Description = _Description, ownerUsername = _ownerUsername});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(UpdateProject);
        // }

        // public async void DeleteProject(string _ProjectId){
        //     GraphApi.Query DeleteProject = graphql_Var.graphql_api.GetQueryByName("DeleteProject", GraphApi.Query.Type.Mutation);
        //     DeleteProject.SetArgs(new{Id = _ProjectId});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(DeleteProject);
        // }

        //

        public async void CreateObject(FlowTObject flowObject, string _projectId)
        {
            GraphApi.Query CreateObject = graphql_Var.graphql_api.GetQueryByName("CreateObject", GraphApi.Query.Type.Mutation);
            CreateObject.SetArgs(new{Id = flowObject.Id, Name = flowObject.Name, X = flowObject.X, Y = flowObject.Y, Z = flowObject.Z, Q_x = flowObject.Q_x,
                                    Q_y = flowObject.Q_y, Q_z = flowObject.Q_z, Q_w = flowObject.Q_w, S_x = flowObject.S_x, S_y = flowObject.S_y,
                                    S_z = flowObject.S_z, R = flowObject.R, G = flowObject.G, B = flowObject.B, A = flowObject.A, Prefab = flowObject.Prefab, projectId = _projectId});
            UnityWebRequest request = await graphql_Var.graphql_api.Post(CreateObject);  
        }

        // public async void UpdateObject(int objectId, string Name, int X, int Y, int Z, int Q_x, int Q_y, int Q_z, int Q_w,
        //                                 int S_x, int S_y, int S_z, int R, int G, int B, int A, string Prefab, string projectId)
        // {
        //     GraphApi.Query UpdateObject = graphql_Var.graphql_api.GetQueryByName("UpdateObject", GraphApi.Query.Type.Mutation);
        //     UpdateObject.SetArgs(new{id = objectId, Name = _Name, X = _X, Y = _Y, Z = _Z, Q_x = _Q_x, Q_y = _Q_y, Q_z = _Q_z, Q_w = _Q_w,
        //                             S_x = _S_x, S_y =_S_y, S_z =_S_z, R = _R, G =_G, B = _B, A = _A, Prefab = _Prefab, projectId = _projectID});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(UpdateObject);
        // }

        public async Task<string> DeleteObject(string objectId, string projectID){
            GraphApi.Query DeleteObject = graphql_Var.graphql_api.GetQueryByName("DeleteObject", GraphApi.Query.Type.Mutation);
            DeleteObject.SetArgs(new{Id = objectId, projectId = projectID});
            UnityWebRequest request = await graphql_Var.graphql_api.Post(DeleteObject);
            
            // This is a check to make sure graphql was successfully executed...
            string text = request.downloadHandler.text;
            Match match = Regex.Match(text, @"""Id"":""(\S+)""");
            string message = match.Groups[1].Captures[0].Value;

            return message;
        }

    /// VSGRAPH
        public async void CreateVSGraph(FlowVSGraph flowVSGraph, string ProjectId) // what about isUpdated?
        {
            GraphApi.Query CreateVSGraph = graphql_Var.graphql_api.GetQueryByName("CreateVSGraph", GraphApi.Query.Type.Mutation);
            CreateVSGraph.SetArgs(new{Id = flowVSGraph.Id, Name = flowVSGraph.Name, serializedNodes = new{flowVSGraph.serializedNodes/**/}, edges= flowVSGraph.edges/**/, groups = flowVSGraph.groups/**/,
                stackNodes = flowVSGraph.stackNodes/**/, pinnedElements = flowVSGraph.pinnedElements/**/, exposedParameters = flowVSGraph.exposedParameters/**/, stickyNotes = flowVSGraph.stickyNotes/**/,
                position = new{flowVSGraph.position.x, flowVSGraph.position.y, flowVSGraph.position.z}, scale = new{flowVSGraph.scale.x, flowVSGraph.scale.y, flowVSGraph.scale.z}, /*references = hm  ??? */ projectId = ProjectId});
            UnityWebRequest request = await graphql_Var.graphql_api.Post(CreateVSGraph);  
        }

        // public async void UpdateVSGraph(int objectId, string Name, int X, int Y, int Z, int Q_x, int Q_y, int Q_z, int Q_w,
        //                                 int S_x, int S_y, int S_z, int R, int G, int B, int A, string Prefab, string projectId)
        // {
        //     GraphApi.Query UpdateVSGraph = graphql_Var.graphql_api.GetQueryByName("UpdateVSGraph", GraphApi.Query.Type.Mutation);
        //     UpdateVSGraph.SetArgs(new{id = objectId, Name = _Name, X = _X, Y = _Y, Z = _Z, Q_x = _Q_x, Q_y = _Q_y, Q_z = _Q_z, Q_w = _Q_w,
        //                             S_x = _S_x, S_y =_S_y, S_z =_S_z, R = _R, G =_G, B = _B, A = _A, Prefab = _Prefab, projectId = _projectID});
        //     UnityWebRequest request = await graphql_Var.graphql_api.Post(UpdateVSGraph);
        // }

        public async void DeleteVSGraph(string objectId){
            GraphApi.Query DeleteVSGraph = graphql_Var.graphql_api.GetQueryByName("DeleteVSGraph", GraphApi.Query.Type.Mutation);
            DeleteVSGraph.SetArgs(new{Id = objectId});
            UnityWebRequest request = await graphql_Var.graphql_api.Post(DeleteVSGraph);
        }



    }

    //The Post function returns a "UnityWebRequest" object and data gotten from the "UnityWebRequest" object can be gotten by
    // string data = request.downloadHandler.text;
    // WE can use a JSON uniy parser on this later...

        
}

