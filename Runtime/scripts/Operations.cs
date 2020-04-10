﻿//using Packages.realityflow_package.Runtime.scripts.Managers;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using RealityFlow.Plugin.Scripts;
//using RealityFlow.Plugin.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to provide a wrapper for the UnityPlugin,
    /// allowing for easy use with the networking tools
    /// </summary>
    public static class Operations
    {
        public static FlowWebsocket _FlowWebsocket { get; private set; }

        static Operations()
        {
            // Set up default behavior response to events

            // Set up Object updates
            CreateObject_Received.ReceivedEvent += _CreateObject;
            DeleteObject_Received.ReceivedEvent += _DeleteObject;
            UpdateObject_Received.ReceivedEvent += _UpdateObject;
            FinalizedUpdateObject_Received.ReceivedEvent += _FinalizedupdateObject;

            // Set up Project updates
            CreateProject_Received.ReceivedEvent += _CreateProject;
            DeleteProject_Received.ReceivedEvent += _DeleteProject;
            GetAllUserProjects_Received.ReceivedEvent += _GetAllUserProjects;
            OpenProject_Received.ReceivedEvent += _OpenProject;
            LeaveProject_Received.ReceivedEvent += _LeaveProject;

            // Set up Room updates
            JoinRoom_Received.ReceivedEvent += _JoinRoom;

            // Set up User updates
            LoginUser_Received.ReceivedEvent += _LoginUser;
            LogoutUser_Received.ReceivedEvent += _LogoutUser;
            RegisterUser_Received.ReceivedEvent += _RegisterUser;
        }

        #region UserOperations

        public static void Login(FlowUser flowUser, string url, LoginUser_Received.LoginReceived_EventHandler callbackFunction)
        {
            bool connectionSuccessful = ConnectToServer(url, flowUser);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                Login_SendToServer loginMessage = new Login_SendToServer(flowUser);
                _FlowWebsocket.SendMessage(loginMessage);

                ConfigurationSingleton.CurrentUser = flowUser;

                LoginUser_Received.ReceivedEvent += callbackFunction; 
            }
        }
        
        public static void Logout(FlowUser flowUser)
        {
            Logout_SendToServer logoutMessage = new Logout_SendToServer(flowUser);
            _FlowWebsocket.SendMessage(logoutMessage);

            _FlowWebsocket.Disconnect();
        }

        public static void Register(string username, string password, string url,RegisterUser_Received.RegisterUserReceived_EventHandler callbackFunction)
        {
            bool connectionSuccssfull = ConnectToServer(url);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                RegisterUser_SendToServer register = new RegisterUser_SendToServer(new FlowUser(username, password));
                _FlowWebsocket.SendMessage(register);

                RegisterUser_Received.ReceivedEvent += callbackFunction; 
            }
        }

        #endregion // UserOperations

        #region ObjectOperations
        public static void CreateObject(FlowTObject flowObject, /*FlowUser flowUser,*/ string projectId, CreateObject_Received.CreateObjectReceived_EventHandler callbackFunction)
        {
            CreateObject_SendToServer createObject =
                new CreateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            _FlowWebsocket.SendMessage(createObject);

            CreateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void UpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, UpdateObject_Received.UpdateObjectReceived_EventHandler callbackFunction)
        {
            UpdateObject_SendToServer updateObject = new UpdateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            _FlowWebsocket.SendMessage(updateObject);

            UpdateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void FinalizedUpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, FinalizedUpdateObject_Received.FinalizedUpdateObjectRecieved_EventHandler callbackFunction)
        {
            FinalizedUpdateObject_SendToServer finalUpdateObject = new FinalizedUpdateObject_SendToServer(flowObject, projectId);
            _FlowWebsocket.SendMessage(finalUpdateObject);

            FinalizedUpdateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void DeleteObject(string idOfObjectToDelete, string projectId, DeleteObject_Received.DeleteObjectReceived_EventHandler callbackFunction)
        {
            DeleteObject_SendToServer deleteObject = new DeleteObject_SendToServer(projectId, idOfObjectToDelete);
            _FlowWebsocket.SendMessage(deleteObject);

            DeleteObject_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // ObjectOperations

        #region ProjectOperations
        public static void CreateProject(FlowProject flowProject, FlowUser flowUser, CreateProject_Received.CreateProjectReceived_EventHandler callbackFunction)
        {
            CreateProject_SendToServer createProject = new CreateProject_SendToServer(flowProject, flowUser);
            _FlowWebsocket.SendMessage(createProject);

            CreateProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void DeleteProject(FlowProject flowProject, FlowUser flowUser, DeleteProject_Received.DeleteProjectReceived_EventHandler callbackFunction)
        {
            DeleteProject_SendToServer deleteProject = new DeleteProject_SendToServer(flowProject, flowUser);
            _FlowWebsocket.SendMessage(deleteProject);

            DeleteProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void OpenProject(string projectId, FlowUser flowUser, OpenProject_Received.OpenProjectReceived_EventHandler callbackFunction)
        {
            //if(GameObject.FindObjectsOfType<GameObject>().Length != 0)
            //{
            //    Debug.LogError("Cannot load project if there are game objects already in scene. Delete all game objects and try again");
            //}
            //else
            //{
            OpenProject_SendToServer openProject = new OpenProject_SendToServer(projectId, flowUser);
            _FlowWebsocket.SendMessage(openProject);
            
            OpenProject_Received.ReceivedEvent += callbackFunction;
            //}
        }

        public static void LeaveProject(string projectId, FlowUser flowUser, LeaveProject_Received.LeaveProjectReceived_EventHandler callbackFunction)
        {
            LeaveProject_SendToServer leaveProject = new LeaveProject_SendToServer(projectId, flowUser);
            _FlowWebsocket.SendMessage(leaveProject);

            LeaveProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void GetAllUserProjects(FlowUser flowUser, GetAllUserProjects_Received.GetAllUserProjects_EventHandler callbackFunction)
        {
            GetAllUserProjects_SendToServer getAllUserProjects = new GetAllUserProjects_SendToServer(flowUser);
            _FlowWebsocket.SendMessage(getAllUserProjects);

            GetAllUserProjects_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // ProjectOperations

        #region RoomMessages

        public static void JoinRoom(string projectId, FlowUser flowUser, JoinRoom_Received.JoinRoomReceived_EventHandler callbackFunction)
        {
            JoinRoom_SendToServer joinRoom = new JoinRoom_SendToServer(projectId, flowUser);
            _FlowWebsocket.SendMessage(joinRoom);

            JoinRoom_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // Room Messages

        #region Checkout system messages

        public static void CheckoutObject(string objectID, string projectID, CheckoutObject_Received.CheckoutObjectReceived_EventHandler callbackFunction)
        {
            CheckoutObject_SendToServer checkoutObject = new CheckoutObject_SendToServer(objectID, projectID);
            _FlowWebsocket.SendMessage(checkoutObject);

            CheckoutObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void CheckinObject(string objectID, string projectID, CheckinObject_Received.CheckinObjectReceived_EventHandler callbackFunction)
        {
            CheckinObject_SendToServer checkinObject = new CheckinObject_SendToServer(objectID, projectID);
            _FlowWebsocket.SendMessage(checkinObject);

            CheckinObject_Received.ReceivedEvent += callbackFunction;
        }

        #endregion Checkout system messages

        /// <summary>
        /// Establish a connection to the server
        /// </summary>
        /// <param name="url"></param>
        public static bool ConnectToServer(string url, FlowUser flowUser = null)
        {
            if(flowUser == null)
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

        private static void _CreateObject(object sender, CreateObjectMessageEventArgs eventArgs)
        {

        }

        private static void _DeleteObject(object sender, DeleteObjectMessageEventArgs eventArgs)
        {
            // Delete object in unity
            //NewObjectManager.DestroyObject(eventArgs.message.DeletedObject.Id);

            if(eventArgs.message.WasSuccessful == true)
            {
                GameObject gameObject = FlowTObject.idToGameObjectMapping[eventArgs.message.DeletedObjectId].AttachedGameObject;

                FlowTObject.idToGameObjectMapping.Remove(eventArgs.message.DeletedObjectId);

                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            Debug.Log("Delete Object: " + eventArgs.message.WasSuccessful);

                

        }

        private static void _UpdateObject(object sender, UpdateObjectMessageEventArgs eventArgs)
        {
            //eventArgs.message.flowObject.UpdateObjectGlobally(eventArgs.message.flowObject);
        }

        private static void _FinalizedupdateObject(object sender, FinalizedUpdateObjectMessageEventArgs eventArgs)
        {

        }

        #endregion Object messages received 

        #region Project messages received

        private static void _CreateProject(object sender, CreateProjectMessageArgs eventArgs)
        {
            ConfigurationSingleton.CurrentProject = eventArgs.message.flowProject;

            Operations.OpenProject(ConfigurationSingleton.CurrentProject.Id, ConfigurationSingleton.CurrentUser, (_, e) => { Debug.Log("opened project after create: " + e.message.WasSuccessful); });
        }

        private static void _DeleteProject(object sender, ConfirmationMessageEventArgs eventArgs)
        {
                
        }

        private static void _GetAllUserProjects(object sender, GetAllUserProjectsMessageEventArgs eventArgs)
        {
                
        }

        private static void _OpenProject(object sender, OpenProjectMessageEventArgs eventArgs)
        {
            ConfigurationSingleton.CurrentProject = eventArgs.message.flowProject;
        }

        private static void _LeaveProject(object sender, LeaveProjectMessageEventArgs eventArgs)
        {
            if(eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Successfully left project");
            }
            else
            {
                Debug.LogWarning("Unable to leave project.");
            }
            
        }

        #endregion Project messages received

        #region Room messages received
        private static void _JoinRoom(object sender, JoinRoomMessageEventArgs eventArgs)
        {
                
        }
        #endregion Room messages received

        #region User messages received
        private static void _LoginUser(object sender, LoginUserMessageEventArgs eventArgs)
        {
                
        }

        private static void _LogoutUser(object sender, ConfirmationMessageEventArgs eventArgs)
        {
            ConfigurationSingleton.CurrentProject = null;
            ConfigurationSingleton.CurrentUser = null;
        }

        private static void _RegisterUser(object sender, ConfirmationMessageEventArgs eventArgs)
        {
            _FlowWebsocket.Disconnect();
        }
    #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
