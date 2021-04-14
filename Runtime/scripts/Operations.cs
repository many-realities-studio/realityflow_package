//using Packages.realityflow_package.Runtime.scripts.Managers;
using Behaviours;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.VSGraphMessages;
using RealityFlow.Plugin.Scripts;
using GraphProcessor;
using System.Threading.Tasks;
using RealityFlow.Plugin.Contrib; // TODO: Fix reference

// Unity/GraphQL libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using GraphQlClient.Core; 
using Contrib.APIeditor;
using Newtonsoft.Json;
using System.Globalization;

namespace Packages.realityflow_package.Runtime.scripts
{
    public delegate void NotifyGraphUpdate();

    public delegate void RunGraphHandler(string vsGraphId);
    
    /// <summary>
    /// The purpose of this class is to provide a wrapper for the UnityPlugin,
    /// allowing for easy use with the networking tools
    /// </summary>
    public static class Operations
    {
        public static FlowWebsocket _FlowWebsocket { get; private set; }

        public static event NotifyGraphUpdate updateRFGV;

        public static event RunGraphHandler runVSGraph;

        static Operations()
        {
            // Set up default behavior response to events

            // Set up Object updates
            ReceivedMessage.AddEventHandler(typeof(CreateObject_Received), false, _CreateObject);
            ReceivedMessage.AddEventHandler(typeof(DeleteObject_Received), false, _DeleteObject);

            // Set up Avatar updates
            ReceivedMessage.AddEventHandler(typeof(CreateAvatar_Received), false, _CreateAvatar);
            ReceivedMessage.AddEventHandler(typeof(DeleteAvatar_Received), false, _DeleteAvatar);

            // Set up Project updates
            ReceivedMessage.AddEventHandler(typeof(CreateProject_Received), false, _CreateProject);
            ReceivedMessage.AddEventHandler(typeof(OpenProject_Received), false, _OpenProject);
            ReceivedMessage.AddEventHandler(typeof(LeaveProject_Received), false, _LeaveProject);
            ReceivedMessage.AddEventHandler(typeof(DeleteProject_Received), false, _DeleteProject);

            // Set up Room updates
            ReceivedMessage.AddEventHandler(typeof(UserLeftRoom_Received), false, _UserLeftRoom);

            // Set up User updates
            ReceivedMessage.AddEventHandler(typeof(LogoutUser_Received), false, _LogoutUser);
            ReceivedMessage.AddEventHandler(typeof(RegisterUser_Received), false, _RegisterUser);
            ReceivedMessage.AddEventHandler(typeof(DeleteUser_Received), false, _DeleteUser);

            // Set up Behaviour updates
            ReceivedMessage.AddEventHandler(typeof(CreateBehaviour_Received), false, _CreateBehaviour);
            ReceivedMessage.AddEventHandler(typeof(DeleteBehaviour_Received), false, _DeleteBehaviour);
            ReceivedMessage.AddEventHandler(typeof(UpdateBehaviour_Received), false, _UpdateBehaviour);

            // Set up Graph updates
            ReceivedMessage.AddEventHandler(typeof(CreateVSGraph_Received), false, _CreateVSGraph);
            ReceivedMessage.AddEventHandler(typeof(DeleteVSGraph_Received), false, _DeleteVSGraph);
            ReceivedMessage.AddEventHandler(typeof(UpdateVSGraph_Received), false, _UpdateVSGraph);
            ReceivedMessage.AddEventHandler(typeof(FinalizedUpdateVSGraph_Received), false, _FinalizedUpdateVSGraph);
            ReceivedMessage.AddEventHandler(typeof(RunVSGraph_Received), false, _RunVSGraph);
            // ReceivedMessage.AddEventHandler(typeof(UpdateNodeView_Received), false, _UpdateNodeView);
        }

        #region UserOperations

        public static void Login(FlowUser flowUser, string url, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            bool connectionSuccessful = ConnectToServer(url, flowUser);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                Login_SendToServer loginMessage = new Login_SendToServer(flowUser);
                FlowWebsocket.SendMessage(loginMessage);

                ConfigurationSingleton.SingleInstance.CurrentUser = flowUser;

                ReceivedMessage.AddEventHandler(typeof(LoginUser_Received), true, callbackFunction);
            }
        }

        public static void Logout(FlowUser flowUser)
        {
            Logout_SendToServer logoutMessage = new Logout_SendToServer(flowUser);
            FlowWebsocket.SendMessage(logoutMessage);

            _FlowWebsocket.Disconnect();
        }

        // public static void LogoutGuest(FlowUser flowUser)
        // {
        //     LogoutGuest_SendToServer logoutGuestMessage = new LogoutGuest_SendToServer(flowUser);
        //     FlowWebsocket.SendMessage(logoutGuestMessage);

        //     _FlowWebsocket.Disconnect();
        // }

        public static void Register(string username, string password, string url, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
             bool connectionSuccssfull = ConnectToServer(url);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                RegisterUser_SendToServer register = new RegisterUser_SendToServer(new FlowUser(username, password));
                FlowWebsocket.SendMessage(register);

                ReceivedMessage.AddEventHandler(typeof(RegisterUser_Received), true, callbackFunction);
            }

            // graphqlClient_Editor createUser = new graphqlClient_Editor();
            // createUser.CreateUser(username, password); 
        }

        public static void DeleteUser(FlowUser flowUser)
        {
            DeleteUser_SendToServer deleteUserMessage = new DeleteUser_SendToServer(flowUser);
            FlowWebsocket.SendMessage(deleteUserMessage);

            //_FlowWebsocket.Disconnect();
        }

        #endregion UserOperations

        #region ObjectOperations

        public static void CreateObject(FlowTObject flowObject, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // graphqlClient_Editor createObject = new graphqlClient_Editor();
            // createObject.CreateObject(flowObject, projectId);

            CreateObject_SendToServer createObject = new CreateObject_SendToServer(flowObject, projectId);
            FlowWebsocket.SendMessage(createObject);

            ReceivedMessage.AddEventHandler(typeof(CreateObject_Received), true, callbackFunction);
        }

        public static void UpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // graphqlClient_Editor updateObject = new graphqlClient_Editor();
            // updateObject.UpdateObject(flowObject, projectId);

            UpdateObject_SendToServer _updateObject = new UpdateObject_SendToServer(flowObject, projectId);
            FlowWebsocket.SendMessage(_updateObject);

            ReceivedMessage.AddEventHandler(typeof(UpdateObject_Received), true, callbackFunction);
        }

        public static async void DeleteObject(string idOfObjectToDelete, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // graphqlClient_Editor deleteObject = new graphqlClient_Editor();
            // string deletedObjectId = await deleteObject.DeleteObject(idOfObjectToDelete, projectId);

            // if(deletedObjectId != null){ // meaning that the requets went through.
            //     __DeleteObject(deletedObjectId);
            // }
            // else
            //     Debug.Log("Error, didn't delete object.");

            DeleteObject_SendToServer _deleteObject = new DeleteObject_SendToServer(projectId, idOfObjectToDelete);
            FlowWebsocket.SendMessage(_deleteObject);

            ReceivedMessage.AddEventHandler(typeof(DeleteObject_Received), true, callbackFunction);
        }

        #endregion ObjectOperations

    #region AvatarOperations

        public static void CreateAvatar(FlowAvatar flowAvatar, string projectId , ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CreateAvatar_SendToServer createAvatar =
                new CreateAvatar_SendToServer(flowAvatar, projectId);
            FlowWebsocket.SendMessage(createAvatar);

            ReceivedMessage.AddEventHandler(typeof(CreateAvatar_Received), true, callbackFunction);
        }

        public static void UpdateAvatar(FlowAvatar flowAvatar, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            UpdateAvatar_SendToServer updateObject = new UpdateAvatar_SendToServer(flowAvatar, projectId);
            FlowWebsocket.SendMessage(updateObject);

            ReceivedMessage.AddEventHandler(typeof(UpdateAvatar_Received), true, callbackFunction);
        }

        public static void DeleteAvatar(string idOfObjectToDelete, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            DeleteAvatar_SendToServer deleteObject = new DeleteAvatar_SendToServer(projectId, idOfObjectToDelete);
            FlowWebsocket.SendMessage(deleteObject);

            ReceivedMessage.AddEventHandler(typeof(DeleteAvatar_Received), true, callbackFunction);
        }

        #endregion AvatarOperations

        #region VSGraphOperations

        // Temporary debugging function
        // public static void CreateVSGraph(string msg)
        // {
        //     FlowWebsocket.SendStringMessage(msg);

        //     //ReceivedMessage.AddEventHandler(typeof(CreateObject_Received), true, callbackFunction);
        // }

        public static void CreateVSGraph(FlowVSGraph flowVSGraph, /*FlowUser flowUser,*/ string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // CreateVSGraph_SendToServer createVSGraph =
            //     new CreateVSGraph_SendToServer(flowVSGraph, /*flowUser,*/ projectId);
            string message = ("{\"FlowVSGraph\":" + JsonUtility.ToJson(flowVSGraph) + ",\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"CreateVSGraph\"}");
            // json.FlowVSGraph = flowVSGraph;
            // json.MessageType = "CreateVSGraph";
            // json.ProjectId = projectId;
            Debug.Log(message);

            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(CreateVSGraph_Received), true, callbackFunction);
        }

        public static void UpdateVSGraph(FlowVSGraph flowVSGraph, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //UpdateVSGraph_SendToServer updateVSGraph = new UpdateVSGraph_SendToServer(flowVSGraph, /*flowUser,*/ projectId); // TODO: format string msg
            Debug.Log(JsonUtility.ToJson(flowVSGraph.edges));
            string message = ("{\"FlowVSGraph\":" + JsonUtility.ToJson(flowVSGraph) + ",\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"UpdateVSGraph\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(UpdateVSGraph_Received), true, callbackFunction);
        }

        public static void FinalizedUpdateVSGraph(FlowVSGraph flowVSGraph, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //UpdateVSGraph_SendToServer updateVSGraph = new UpdateVSGraph_SendToServer(flowVSGraph, /*flowUser,*/ projectId); // TODO: format string msg
            Debug.Log(JsonUtility.ToJson(flowVSGraph.edges));
            string message = ("{\"FlowVSGraph\":" + JsonUtility.ToJson(flowVSGraph) + ",\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"FinalizedUpdateVSGraph\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(FinalizedUpdateVSGraph_Received), true, callbackFunction);
        }

        public static void DeleteVSGraph(string idOfVSGraphToDelete, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //DeleteVSGraph_SendToServer deleteVSGraph = new DeleteVSGraph_SendToServer(projectId, idOfVSGraphToDelete); // TODO: format string msg
            string message = ("{\"VSGraphId\":\"" + idOfVSGraphToDelete + "\",\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"DeleteVSGraph\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(DeleteVSGraph_Received), true, callbackFunction);
        }

        public static void UpdateNodeView(NodeView nodeView, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //UpdateVSGraph_SendToServer updateVSGraph = new UpdateVSGraph_SendToServer(flowVSGraph, /*flowUser,*/ projectId); // TODO: format string msg
            string boolValue = (nodeView.CanBeModified ? "true" : "false");
            string message = ("{\"FlowNodeView\": {\"CanBeModified\":" + boolValue + ",\"LocalPos\":" + JsonUtility.ToJson(nodeView.localPos) + ",\"NodeGUID\":\"" + nodeView.nodeGUID + "\"},\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"UpdateNodeView\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(UpdateVSGraph_Received), true, callbackFunction);
        }

        public static void RunVSGraph(string idOfVSGraphToRun, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            string message = ("{\"VSGraphId\":\"" + idOfVSGraphToRun + "\",\"ProjectId\":\"" + projectId + "\",\"MessageType\":\"RunVSGraph\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(RunVSGraph_Received), true, callbackFunction);
        }

        #endregion VSGraphOperations

        #region BehaviourOperations

        public static async void CreateBehaviour(FlowBehaviour behaviour, string projectId, List<string> behavioursToLinkTo, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // GraphQL
             graphqlClient_Editor createBehaviour = new graphqlClient_Editor();
             //createBehaviour.CreateBehaviour(behaviour, projectId, behavioursToLinkTo);
             string IdToBeLinked = await createBehaviour.CreateBehaviour(behaviour, projectId, behavioursToLinkTo);
    
            if(IdToBeLinked != null){
                behaviour.BehaviourName = behaviour.flowAction.ActionType;
                //behavioursToLinkTo.Add(IdToBeLinked);
                __CreateBehaviour(behaviour, projectId, behavioursToLinkTo, IdToBeLinked);
                
            }
            
            // Probz change this name...
            // CreateBehaviour_SendToServer _createBehaviour = new CreateBehaviour_SendToServer(behaviour, projectId, behavioursToLinkTo);

            // FlowWebsocket.SendMessage(_createBehaviour);
            //ReceivedMessage.AddEventHandler(typeof(CreateBehaviour_Received), true, callbackFunction);

        }

        public static void DeleteBehaviour(List<string> behaviourIds, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //GraphQL
            // graphqlClient_Editor deleteBehaviour = new graphqlClient_Editor();
            // deleteBehaviour.DeleteBehaviour(behaviourIds, projectId);
            //__DeleteBehaviour(behaviourIds);
            DeleteBehaviour_SendToServer deleteBehaviour = new DeleteBehaviour_SendToServer(behaviourIds, projectId);
            FlowWebsocket.SendMessage(deleteBehaviour);

            ReceivedMessage.AddEventHandler(typeof(DeleteBehaviour_Received), true, callbackFunction);
        }

        public static void UpdateBehaviour(FlowBehaviour behaviour, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            UpdateBehaviour_SendToServer updateBehaviour = new UpdateBehaviour_SendToServer(behaviour, projectId);
            FlowWebsocket.SendMessage(updateBehaviour);

            ReceivedMessage.AddEventHandler(typeof(UpdateBehaviour_Received), true, callbackFunction);
        }

        #endregion BehaviourOperations

        #region ProjectOperations

        public static void CreateProject(FlowProject flowProject, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CreateProject_SendToServer createProject = new CreateProject_SendToServer(flowProject, flowUser);
            FlowWebsocket.SendMessage(createProject);

            ReceivedMessage.AddEventHandler(typeof(CreateProject_Received), true, callbackFunction);
        }

        public static void DeleteProject(FlowProject flowProject, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            DeleteProject_SendToServer deleteProject = new DeleteProject_SendToServer(flowProject, flowUser);
            FlowWebsocket.SendMessage(deleteProject);

            ReceivedMessage.AddEventHandler(typeof(DeleteProject_Received), true, callbackFunction);
        }

        public static void OpenProject(string projectId, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            //if(GameObject.FindObjectsOfType<GameObject>().Length != 0)
            //{
            //    Debug.LogError("Cannot load project if there are game objects already in scene. Delete all game objects and try again");
            //}
            //else
            //{
            // GameObject.Find()
            // Create Avatar HERE
            OpenProject_SendToServer openProject = new OpenProject_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(openProject);

            ReceivedMessage.AddEventHandler(typeof(OpenProject_Received), true, callbackFunction);
            //}
        }

        public static void LeaveProject(string projectId, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            LeaveProject_SendToServer leaveProject = new LeaveProject_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(leaveProject);

            ReceivedMessage.AddEventHandler(typeof(LeaveProject_Received), true, callbackFunction);
        }

        public static void GetAllUserProjects(FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            GetAllUserProjects_SendToServer getAllUserProjects = new GetAllUserProjects_SendToServer(flowUser);
            FlowWebsocket.SendMessage(getAllUserProjects);

            ReceivedMessage.AddEventHandler(typeof(GetAllUserProjects_Received), true, callbackFunction);
        }

        #endregion ProjectOperations

        #region RoomMessages

        public static void JoinRoom(string projectId, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            JoinRoom_SendToServer joinRoom = new JoinRoom_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(joinRoom);

            ReceivedMessage.AddEventHandler(typeof(JoinRoom_Received), true, callbackFunction);
        }

        #endregion RoomMessages

        #region Checkout system messages

        public static void CheckoutObject(string objectID, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CheckoutObject_SendToServer checkoutObject = new CheckoutObject_SendToServer(objectID, projectID);
            FlowWebsocket.SendMessage(checkoutObject);

            ReceivedMessage.AddEventHandler(typeof(CheckoutObject_Received), true, callbackFunction);
        }

        public static void CheckinObject(string objectID, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CheckinObject_SendToServer checkinObject = new CheckinObject_SendToServer(objectID, projectID);
            FlowWebsocket.SendMessage(checkinObject);

            ReceivedMessage.AddEventHandler(typeof(CheckinObject_Received), true, callbackFunction);
        }

        public static void CheckoutVSGraph(string vsGraphID, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CheckoutVSGraph_SendToServer checkoutVSGraph = new CheckoutVSGraph_SendToServer(vsGraphID, projectID);
            FlowWebsocket.SendMessage(checkoutVSGraph);

            ReceivedMessage.AddEventHandler(typeof(CheckoutVSGraph_Received), true, callbackFunction);
        }

        public static void CheckinVSGraph(string vsGraphID, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CheckinVSGraph_SendToServer checkinVSGraph = new CheckinVSGraph_SendToServer(vsGraphID, projectID);
            FlowWebsocket.SendMessage(checkinVSGraph);

            ReceivedMessage.AddEventHandler(typeof(CheckinVSGraph_Received), true, callbackFunction);
        }

        public static void CheckoutNodeView(NodeView nodeView, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            // CheckoutNodeView_SendToServer checkoutNodeView = new CheckoutNodeView_SendToServer(nodeGUID, projectID);
            // FlowWebsocket.SendMessage(checkoutNodeView);
            string boolValue = (nodeView.CanBeModified ? "true" : "false");
            string message = ("{\"FlowNodeView\": {\"CanBeModified\":" + boolValue + ",\"LocalPos\":" + JsonUtility.ToJson(nodeView.localPos) + ",\"NodeGUID\":\"" + nodeView.nodeGUID + "\"},\"ProjectId\":\"" + projectID + "\",\"MessageType\":\"CheckoutNodeView\"}");
            FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(CheckoutNodeView_Received), true, callbackFunction);
        }

        public static void CheckinNodeView(string nodeGUID, string projectID, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CheckinNodeView_SendToServer checkinNodeView = new CheckinNodeView_SendToServer(nodeGUID, projectID);
            FlowWebsocket.SendMessage(checkinNodeView);
            // string message = ("{\"NodeView\": {\"CanBeModified\":" + nodeView.CanBeModified + ",\"LocalPos\":" + JsonUtility.ToJson(nodeView.localPos) + ",\"NodeGUID\":\"" + nodeView.nodeGUID + "\"},\"ProjectId\":\"" + projectID + "\",\"MessageType\":\"CheckinNodeView\"}");
            // FlowWebsocket.SendGraphMessage(message);

            ReceivedMessage.AddEventHandler(typeof(CheckinNodeView_Received), true, callbackFunction);
        }

        #endregion Checkout system messages

        /// <summary>
        /// Establish a connection to the server
        /// </summary>
        /// <param name="url"></param>
        public static bool ConnectToServer(string url, FlowUser flowUser = null)
        {
            if (flowUser == null)
            {
                flowUser = new FlowUser("God", "Jesus");
            }
            _FlowWebsocket = new FlowWebsocket(url, flowUser);

            return _FlowWebsocket.IsConnected;
        }

        //public delegate void functionCalledOnUpdate();
        //public static void OnUpdate(functionCalledOnUpdate functionCalledOnUpdate)
        //{
        //    // Handle what the GUI needs to update on every frame
        //    functionCalledOnUpdate();
        //}

        // TODO: Fill out default behavior

        #region Default actions taken after receiving messages (These happen no matter what the user does)

        #region Object messages received

        private static void _CreateObject(object sender, BaseReceivedEventArgs eventArgs)
        {
        }

        private static void _DeleteObject(object sender, BaseReceivedEventArgs eventArgs)
        {
            // Delete object in unity
            //NewObjectManager.DestroyObject(eventArgs.message.DeletedObject.Id);

            if (eventArgs.message.WasSuccessful == true)
            {
                GameObject gameObject = FlowTObject.idToGameObjectMapping[eventArgs.message.DeletedObjectId].AttachedGameObject;

                FlowTObject.idToGameObjectMapping.Remove(eventArgs.message.DeletedObjectId);

                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            Debug.Log("Delete Object: " + eventArgs.message.WasSuccessful);
        }

        // GRAPHQL deleteObject
        private static void __DeleteObject(string DeletedObjectId)
        {
            GameObject gameObject = FlowTObject.idToGameObjectMapping[DeletedObjectId].AttachedGameObject;

            FlowTObject.idToGameObjectMapping.Remove(DeletedObjectId);

            UnityEngine.Object.DestroyImmediate(gameObject);

            Debug.Log("Delete Object was successful!");
        }

        #endregion Object messages received

        #region Avatar messages received

        private static void _CreateAvatar(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
/*                 let returnContent = {
      "MessageType": "CreateAvatar",
      "FlowAvatar": returnData[0],
      "WasSuccessful": (returnData[0] == null) ? false: true,
      "AvatarList": returnData[2]
    } */        
                // load the prefab of avatar

                // foreach (FlowAvatar user in eventArgs.message.AvatarList){
                //     // make sure we exclude the client's FlowAvatar
                //     if ( user.Id != eventArgs.message.FlowAvatar.Id ){
                        
                //     }
                // }
            }
        }

        private static void _DeleteAvatar(object sender, BaseReceivedEventArgs eventArgs)
        {
            // Delete object in unity
            //NewObjectManager.DestroyObject(eventArgs.message.DeletedObject.Id);

            if (eventArgs.message.WasSuccessful == true)
            {

                // TODO: Remove Mono
                GameObject gameObject = FlowAvatar.idToAvatarMapping[eventArgs.message.DeletedAvatarId].AttachedGameObject;
        
                // FlowAvatar.idToGameObjectMapping.Remove(eventArgs.message.DeletedAvatarId);

                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            Debug.Log("Delete Avatar: " + eventArgs.message.WasSuccessful);
        }

        #endregion Object messages received

        #region VSGraph messages received

        private static void _CreateVSGraph(object sender, BaseReceivedEventArgs eventArgs)
        {
        }

        private static void _DeleteVSGraph(object sender, BaseReceivedEventArgs eventArgs)
        {
            // TODO: Stuff to delete the graph in Unity goes here.
            if (eventArgs.message.WasSuccessful == true)
            {
                GameObject gameObject = FlowVSGraph.idToVSGraphMapping[eventArgs.message.DeletedVSGraphId].AttachedGameObject;

                FlowVSGraph.idToVSGraphMapping.Remove(eventArgs.message.DeletedVSGraphId);

                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            Debug.Log("Delete VSGraph: " + eventArgs.message.WasSuccessful);
        }

        private static void _UpdateVSGraph(object sender, BaseReceivedEventArgs eventArgs)
        {
            updateRFGV?.Invoke();
        }

        private static void _FinalizedUpdateVSGraph(object sender, BaseReceivedEventArgs eventArgs)
        {
            updateRFGV?.Invoke();
        }

        private static void _RunVSGraph(object sender, BaseReceivedEventArgs eventArgs)
        {
            runVSGraph?.Invoke(eventArgs.message.VSGraphId);
        }

        #endregion VSGraph messages received

        #region Behaviour messages received

        private static void _CreateBehaviour(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Success creating behaviour " + eventArgs.message.flowBehaviour.TypeOfTrigger);

                BehaviourEventManager.CreateNewBehaviour(eventArgs.message.flowBehaviour);

                foreach (string behaviourToLinkId in eventArgs.message.behavioursToLinkTo)
                {
                    BehaviourEventManager.LinkBehaviours(eventArgs.message.flowBehaviour.Id, behaviourToLinkId);
                }
            }

            Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);
        }

        //GraphQL method
        private static void __CreateBehaviour(FlowBehaviour behaviour, string projectId, List<string> behavioursToLinkTo, string currentBehaviourId)
        {      // Parent
            if (currentBehaviourId != null)
            {
                Debug.Log("Success creating behaviour with GraphQL " + behaviour.TypeOfTrigger);

                BehaviourEventManager.CreateNewBehaviour(behaviour);

                foreach (string behaviourToLinkId in behavioursToLinkTo)
                {
                    BehaviourEventManager.LinkBehaviours(behaviour.Id, behaviourToLinkId);
                }
            }

            Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);
        }

        private static void _DeleteBehaviour(object sender, BaseReceivedEventArgs eventArgs)
        {
            // this is where things happen after a DeleteBehaviour message is deserialized
            if (eventArgs.message.WasSuccessful)
            {
                // for each behaviour id in behaviourIds, delete from behaviour list and from each object's interactablevents
                foreach (string id in eventArgs.message.BehaviourIds)
                {
                    FlowBehaviour fb = BehaviourEventManager.BehaviourList[id];

                    BehaviourEventManager.DeleteFlowBehaviour(fb.TriggerObjectId, fb.TriggerObjectId, fb);
                }
                Debug.Log("Successfully delete all behaviours in the chain");
                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);
            }
        }

        //gql
        private static void __DeleteBehaviour(List<string> BehaviourIds)
        {
            // this is where things happen after a DeleteBehaviour message is deserialized
            // if (eventArgs.message.WasSuccessful)
            // {
                // for each behaviour id in behaviourIds, delete from behaviour list and from each object's interactablevents
                foreach (string id in BehaviourIds)
                {
                    FlowBehaviour fb = BehaviourEventManager.BehaviourList[id];

                    BehaviourEventManager.DeleteFlowBehaviour(fb.TriggerObjectId, fb.TriggerObjectId, fb);
                }
                Debug.Log("Successfully delete all behaviours in the chain");
                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);
            //
        }

        private static void _UpdateBehaviour(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Success updating behaviour " + eventArgs.message.flowBehaviour.TypeOfTrigger);

                BehaviourEventManager.UpdateBehaviour(eventArgs.message.flowBehaviour);
            }
        }

        #endregion Behaviour messages received

        #region Project messages received

        private static void _CreateProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;

            Operations.OpenProject(ConfigurationSingleton.SingleInstance.CurrentProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) => { Debug.Log("opened project after create: " + e.message.WasSuccessful); });
        }

        private static void _OpenProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            Debug.Log("Now in load project");
            ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;
            
            ConfigurationSingleton asset = ScriptableObject.CreateInstance<ConfigurationSingleton>();
            asset.CurrentProject = ConfigurationSingleton.SingleInstance.CurrentProject;
            asset.CurrentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
            AssetDatabase.CreateAsset(asset, "Assets/RealityFlowConfiguration.asset");
            AssetDatabase.SaveAssets();

            if (eventArgs.message.WasSuccessful == true)
            {
                ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;

                // Clear the behaviour list and BEM
                BehaviourEventManager.Clear();
                BehaviourEventManager.Initialize();

                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);

                if (eventArgs.message.flowProject.behaviourList == null)
                {
                    Debug.Log("It's null huh");
                }
                else
                {
                    Debug.Log("ya yeet");
                }

                foreach (FlowBehaviour fb in eventArgs.message.flowProject.behaviourList)
                {
                    BehaviourEventManager.CreateNewBehaviour(fb);
                }

                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);
            }
        }

        private static void _LeaveProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Successfully left project");

                // Clear the behaviour list and BEM
                BehaviourEventManager.Clear();
                BehaviourEventManager.Initialize();
            }
            else
            {
                Debug.LogWarning("Unable to leave project.");
            }
        }

        private static void _DeleteProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Successfully deleted project");

                // Clear the behaviour list and BEM
                BehaviourEventManager.Clear();
                BehaviourEventManager.Initialize();
            }
            else
            {
                Debug.LogWarning("Unable to delete project.");
            }
        }

        #endregion Project messages received

        #region Room messages received

        private static void _UserLeftRoom(object sender, BaseReceivedEventArgs eventArgs)
        {
            Debug.Log("Room Alert: " + eventArgs.message.leftRoomMessage);

        }

        #endregion Room messages received

        #region User messages received

        private static void _LogoutUser(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = null;
            ConfigurationSingleton.SingleInstance.CurrentUser = null;
        }

        private static void _DeleteUser(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = null;
            ConfigurationSingleton.SingleInstance.CurrentUser = null;
            _FlowWebsocket.Disconnect();
        }

        private static void _RegisterUser(object sender, BaseReceivedEventArgs eventArgs)
        {
            _FlowWebsocket.Disconnect();
        }

        #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
