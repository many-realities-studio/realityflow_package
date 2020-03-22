﻿using Packages.realityflow_package.Runtime.scripts.Managers;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages;
using RealityFlow.Plugin.Scripts;
using Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UIElements;

//using RealityFlow.Plugin.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to provide a wrapper for the UnityPlugin,
    /// allowing for easy use with the networking tools
    /// </summary>
    public static class Operations
    {
        private static FlowWebsocket _flowWebsocket;
        public static FlowWebsocket FlowWebsocket { get => _flowWebsocket; }

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

            // Set up Room updates
            JoinRoom_Received.ReceivedEvent += _JoinRoom;

            // Set up User updates
            LoginUser_Received.ReceivedEvent += _LoginUser;
            LogoutUser_Received.ReceivedEvent += _LogoutUser;
            RegisterUser_Received.ReceivedEvent += _RegisterUser;

            CreateBehaviour_Received.ReceivedEvent += _CreateBehaviour;
            // Set up Behaviour updates

        }

        #region UserOperations

        public static void Login(FlowUser flowUser, LoginUser_Received.LoginReceived_EventHandler callbackFunction)
        {
            Login_SendToServer loginMessage = new Login_SendToServer(flowUser);
            FlowWebsocket.SendMessage(loginMessage);

            LoginUser_Received.ReceivedEvent += callbackFunction;
        }
        
        public static void Logout(FlowUser flowUser, LogoutUser_Received.LogoutReceived_EventHandler callbackFunction)
        {
            Logout_SendToServer logoutMessage = new Logout_SendToServer(flowUser);
            FlowWebsocket.SendMessage(logoutMessage);

            LogoutUser_Received.ReceivedEvent += callbackFunction;
        }

        public static void Register(string username, string password, RegisterUser_Received.RegisterUserReceived_EventHandler callbackFunction)
        {
            RegisterUser_SendToServer register = new RegisterUser_SendToServer(new FlowUser(username, password));
            FlowWebsocket.SendMessage(register);

            RegisterUser_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // UserOperations

        #region ObjectOperations
        public static void CreateObject(FlowTObject flowObject, /*FlowUser flowUser,*/ string projectId, CreateObject_Received.CreateObjectReceived_EventHandler callbackFunction)
        {
            CreateObject_SendToServer createObject =
                new CreateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            FlowWebsocket.SendMessage(createObject);

            CreateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void UpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, UpdateObject_Received.UpdateObjectReceived_EventHandler callbackFunction)
        {
            UpdateObject_SendToServer updateObject = new UpdateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            FlowWebsocket.SendMessage(updateObject);

            UpdateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void FinalizedUpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, FinalizedUpdateObject_Received.OpenProjectReceived_EventHandler callbackFunction)
        {
            FinalizedUpdateObject_SendToServer finalUpdateObject = new FinalizedUpdateObject_SendToServer(flowObject, flowUser, projectId);
            FlowWebsocket.SendMessage(finalUpdateObject);

            FinalizedUpdateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void DeleteObject(FlowTObject objectToDelete, string projectId, DeleteObject_Received.DeleteObjectReceived_EventHandler callbackFunction)
        {
            DeleteObject_SendToServer deleteObject = new DeleteObject_SendToServer(objectToDelete, projectId);
            FlowWebsocket.SendMessage(deleteObject);

            DeleteObject_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // ObjectOperations
        #region BehaviourOperations

        public static void CreateBehaviour(FlowBehaviour behaviour, FlowUser flowUser, string projectId, string objectId, CreateBehaviour_Received.CreateBehaviourReceived_EventHandler callbackFunction)
        {
            CreateBehaviour_SendToServer createBehaviour = new CreateBehaviour_SendToServer(behaviour, flowUser, projectId, objectId);
            FlowWebsocket.SendMessage(createBehaviour);

            CreateBehaviour_Received.ReceivedEvent += callbackFunction;
        }

        #endregion



        #region ProjectOperations
        public static void CreateProject(FlowProject flowProject, FlowUser flowUser, CreateProject_Received.CreateProjectReceived_EventHandler callbackFunction)
        {
            CreateProject_SendToServer createProject = new CreateProject_SendToServer(flowProject, flowUser);
            FlowWebsocket.SendMessage(createProject);

            CreateProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void DeleteProject(FlowProject flowProject, FlowUser flowUser, DeleteProject_Received.DeleteProjectReceived_EventHandler callbackFunction)
        {
            DeleteProject_SendToServer deleteProject = new DeleteProject_SendToServer(flowProject, flowUser);
            FlowWebsocket.SendMessage(deleteProject);

            DeleteProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void OpenProject(string projectId, FlowUser flowUser, OpenProject_Received.OpenProjectReceived_EventHandler callbackFunction)
        {
            OpenProject_SendToServer openProject = new OpenProject_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(openProject);

            OpenProject_Received.ReceivedEvent += callbackFunction;
        }

        public static void GetAllUserProjects(FlowUser flowUser, GetAllUserProjects_Received.GetAllUserProjects_EventHandler callbackFunction)
        {
            GetAllUserProjects_SendToServer getAllUserProjects = new GetAllUserProjects_SendToServer(flowUser);
            FlowWebsocket.SendMessage(getAllUserProjects);

            GetAllUserProjects_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // ProjectOperations

        #region RoomMessages

        public static void JoinRoom(string projectId, FlowUser flowUser, JoinRoom_Received.JoinRoomReceived_EventHandler callbackFunction)
        {
            JoinRoom_SendToServer joinRoom = new JoinRoom_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(joinRoom);

            JoinRoom_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // Room Messages

        /// <summary>
        /// Establish a connection to the server
        /// </summary>
        /// <param name="url"></param>
        public static void ConnectToServer(string url)
        {
            _flowWebsocket = new FlowWebsocket(url);
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
                NewObjectManager.DestroyObject(eventArgs.message.DeletedObject.FlowId);
            }

            private static void _UpdateObject(object sender, UpdateObjectMessageEventArgs eventArgs)
            {

            }

            private static void _FinalizedupdateObject(object sender, FinalizedUpdateObjectMessageEventArgs eventArgs)
            {

            }

        #endregion Object messages received 

        #region Behaviour messages received
        private static void _CreateBehaviour(object sender, CreateBehaviourEventArgs eventArgs)
        {
            // this is where things happen after a createBehaviour  message is received

             BehaviourEventManager bem = UnityEngine.Object.FindObjectOfType<EventSystem>().GetComponent<BehaviourEventManager>();
            //

            // attach the bem to the event system if not already on



            GameObject go1 = GameObject.FindGameObjectWithTag("Cubey");
            Debug.Log(go1);
            GameObject go2 = GameObject.FindGameObjectWithTag("Sphere");
            Debug.Log(go2);

            TeleportCoordinates tc = go2.GetComponent<TeleportCoordinates>();
            if (tc == null)
                tc = go2.AddComponent<TeleportCoordinates>();

            tc.SetCoordinates(go2.transform.position);

           
            Guid g1 = bem.MakeObjectInteractable(go1);
            Guid g2 = bem.MakeObjectInteractable(go2);


            BehaviourEvent b = bem.CreateNewBehaviourEvent("Teleport", g1, g2, null);
            //b.CallBehaviourEvent();

            GameObject go = GameObject.FindGameObjectWithTag("other");
            Debug.Log(go);

            Guid goi = bem.MakeObjectInteractable(go);
            BehaviourEvent t = bem.CreateNewBehaviourEvent("Click", goi, goi, b);

            BehaviourEventManager.blist.Add(t);


            Debug.Log("hola");
            /*
            Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep |
             SelectionMode.ExcludePrefab | SelectionMode.Editable);

            string placeOnSurfaceDialogDecisionKey = "Example.PlaceOnSurfaceDecision";
            if (
                EditorUtility.DisplayDialog("Start behavior?",
                    "Teleport to Cube", "ok", "no"))
            {
                Debug.Log(b);
                Debug.Log("THE SECOND OBJECT");
                Debug.Log(b.secondObject);
                t.EventTrigger();
            }
            */

        }


        #endregion

        #region Project messages received

        private static void _CreateProject(object sender, ConfirmationMessageEventArgs eventArgs)
            {
                
            }

            private static void _DeleteProject(object sender, ConfirmationMessageEventArgs eventArgs)
            {
                
            }

            private static void _GetAllUserProjects(object sender, GetAllUserProjectsMessageEventArgs eventArgs)
            {
                
            }

            private static void _OpenProject(object sender, OpenProjectMessageEventArgs eventArgs)
            {
                
            }

            #endregion Project messages received

            #region Room messages received
            private static void _JoinRoom(object sender, JoinRoomMessageEventArgs eventArgs)
            {
                
            }
            #endregion Room messages received

            #region User messages received
            private static void _LoginUser(object sender, ConfirmationMessageEventArgs eventArgs)
            {
                
            }

            private static void _LogoutUser(object sender, ConfirmationMessageEventArgs eventArgs)
            {
                
            }

            private static void _RegisterUser(object sender, ConfirmationMessageEventArgs eventArgs)
            {
                
            }
        #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
