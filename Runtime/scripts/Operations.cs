﻿//using Packages.realityflow_package.Runtime.scripts.Managers;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages;
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
        public static FlowWebsocket _FlowWebsocket { get; private set; }

        static Operations()
        {
            // Set up default behavior response to events
            
            // Set up Object updates
            ReceivedMessage.AddEventHandler(typeof(CreateObject_Received), false, _CreateObject);
            ReceivedMessage.AddEventHandler(typeof(DeleteObject_Received), false, _DeleteObject);
            ReceivedMessage.AddEventHandler(typeof(UpdateObject_Received), false, _UpdateObject);

            // Set up Project updates
            ReceivedMessage.AddEventHandler(typeof(CreateProject_Received), false, _CreateProject);
            ReceivedMessage.AddEventHandler(typeof(DeleteProject_Received), false, _DeleteProject);
            ReceivedMessage.AddEventHandler(typeof(GetAllUserProjects_Received), false, _GetAllUserProjects);
            ReceivedMessage.AddEventHandler(typeof(OpenProject_Received), false, _OpenProject);
            ReceivedMessage.AddEventHandler(typeof(LeaveProject_Received), false, _LeaveProject);

            // Set up Room updates
            ReceivedMessage.AddEventHandler(typeof(JoinRoom_Received), false, _JoinRoom);
            ReceivedMessage.AddEventHandler(typeof(UserLeftRoom_Received), false, _UserLeftRoom);

            // Set up User updates
            ReceivedMessage.AddEventHandler(typeof(LoginUser_Received), false, _LoginUser);
            ReceivedMessage.AddEventHandler(typeof(LogoutUser_Received), false, _LogoutUser);
            ReceivedMessage.AddEventHandler(typeof(RegisterUser_Received), false, _RegisterUser);

            // Set up Behaviour updates
            ReceivedMessage.AddEventHandler(typeof(CreateBehaviour_Received), false, _CreateBehaviour);
            ReceivedMessage.AddEventHandler(typeof(DeleteBehaviour_Received), false, _DeleteBehaviour);
            ReceivedMessage.AddEventHandler(typeof(UpdateBehaviour_Received), false, _UpdateBehaviour);
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

        public static void Register(string username, string password, string url , ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            bool connectionSuccssfull = ConnectToServer(url);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                RegisterUser_SendToServer register = new RegisterUser_SendToServer(new FlowUser(username, password));
                FlowWebsocket.SendMessage(register);

                ReceivedMessage.AddEventHandler(typeof(RegisterUser_Received), true, callbackFunction);
            }
        }

        #endregion // UserOperations

        #region ObjectOperations
        public static void CreateObject(FlowTObject flowObject, /*FlowUser flowUser,*/ string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CreateObject_SendToServer createObject =
                new CreateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            FlowWebsocket.SendMessage(createObject);

            ReceivedMessage.AddEventHandler(typeof(CreateObject_Received), true, callbackFunction);
        }

        public static void UpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            UpdateObject_SendToServer updateObject = new UpdateObject_SendToServer(flowObject, /*flowUser,*/ projectId);
            FlowWebsocket.SendMessage(updateObject);

            ReceivedMessage.AddEventHandler(typeof(UpdateObject_Received), true, callbackFunction);
        }

        public static void DeleteObject(string idOfObjectToDelete, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            DeleteObject_SendToServer deleteObject = new DeleteObject_SendToServer(projectId, idOfObjectToDelete);
            FlowWebsocket.SendMessage(deleteObject);

            ReceivedMessage.AddEventHandler(typeof(DeleteObject_Received), true, callbackFunction);
            ;
        }

        #endregion // ObjectOperations
        #region BehaviourOperations

        public static void CreateBehaviour(FlowBehaviour behaviour, string projectId, List<string> behavioursToLinkTo, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            CreateBehaviour_SendToServer createBehaviour = new CreateBehaviour_SendToServer(behaviour, projectId, behavioursToLinkTo);
            FlowWebsocket.SendMessage(createBehaviour);

            ReceivedMessage.AddEventHandler(typeof(CreateBehaviour_Received), true, callbackFunction);
        }


        public static void DeleteBehaviour(List<string> behaviourIds, string projectId, DeleteBehaviour_Received.DeleteBehaviourReceived_EventHandler callbackFunction)
        {
            DeleteBehaviour_SendToServer deleteBehaviour = new DeleteBehaviour_SendToServer(behaviourIds, projectId);
            _FlowWebsocket.SendMessage(deleteBehaviour);

            ReceivedMessage.AddEventHandler(typeof(DeleteBehaviour_Received), true, callbackFunction);
        }

        public static void UpdateBehaviour(FlowBehaviour behaviour, string projectId, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            UpdateBehaviour_SendToServer updateBehaviour = new UpdateBehaviour_SendToServer(behaviour, projectId);
            FlowWebsocket.SendMessage(updateBehaviour);

            ReceivedMessage.AddEventHandler(typeof(UpdateBehaviour_Received), true, callbackFunction);
        }

        #endregion



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

        #endregion // ProjectOperations

        #region RoomMessages

        public static void JoinRoom(string projectId, FlowUser flowUser, ReceivedMessage.ReceivedMessageEventHandler callbackFunction)
        {
            JoinRoom_SendToServer joinRoom = new JoinRoom_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(joinRoom);

            ReceivedMessage.AddEventHandler(typeof(JoinRoom_Received), true, callbackFunction);
        }

        #endregion // Room Messages

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

        private static void _CreateObject(object sender, BaseReceivedEventArgs eventArgs)
        {

        }

        private static void _DeleteObject(object sender, BaseReceivedEventArgs eventArgs)
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

        private static void _UpdateObject(object sender, BaseReceivedEventArgs eventArgs)
        {
            //eventArgs.message.flowObject.UpdateObjectGlobally(eventArgs.message.flowObject);
        }

        private static void _FinalizedupdateObject(object sender, BaseReceivedEventArgs eventArgs)
        {
        }

        #endregion Object messages received 
          
          
          
          
          
        #region Behaviour messages received
        private static void _CreateBehaviour(object sender, BaseReceivedEventArgs eventArgs)
        {
            if(eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Success creating behaviour " + eventArgs.message.flowBehaviour.TypeOfTrigger);

                BehaviourEventManager.CreateNewBehaviour(eventArgs.message.flowBehaviour);

                foreach(string behaviourToLinkId in eventArgs.message.behavioursToLinkTo)
                {
                    BehaviourEventManager.LinkBehaviours(eventArgs.message.flowBehaviour.Id, behaviourToLinkId);
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
                foreach(string id in eventArgs.message.BehaviourIds)
                {
                    FlowBehaviour fb = BehaviourEventManager.BehaviourList[id];

                    BehaviourEventManager.DeleteFlowBehaviour(fb.TriggerObjectId, fb.TriggerObjectId, fb);
                }
                Debug.Log("Successfully delete all behaviours in the chain");
                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);

            }
        }


        private static void _UpdateBehaviour(object sender, BaseReceivedEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Success updating behaviour " + eventArgs.message.flowBehaviour.TypeOfTrigger);

                BehaviourEventManager.UpdateBehaviour(eventArgs.message.flowBehaviour);
            }
        }

        #endregion




        #region Project messages received

        private static void _CreateProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;
            //ConfigurationSingleton asset = ScriptableObject.CreateInstance<ConfigurationSingleton>();
            //asset.CurrentProject = ConfigurationSingleton.SingleInstance.CurrentProject;
            //asset.CurrentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
            //AssetDatabase.CreateAsset(asset, "Assets/RealityFlowConfiguration.asset");
            //AssetDatabase.SaveAssets();

            //EditorUtility.FocusProjectWindow();

            //Selection.activeObject = asset;

            Operations.OpenProject(ConfigurationSingleton.SingleInstance.CurrentProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) => { Debug.Log("opened project after create: " + e.message.WasSuccessful); });
        }

        private static void _DeleteProject(object sender, BaseReceivedEventArgs eventArgs)
        {
                
        }

        private static void _GetAllUserProjects(object sender, BaseReceivedEventArgs eventArgs)
        {
                
        }

        private static void _OpenProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;

            ConfigurationSingleton asset = ScriptableObject.CreateInstance<ConfigurationSingleton>();
            asset.CurrentProject = ConfigurationSingleton.SingleInstance.CurrentProject;
            asset.CurrentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
            AssetDatabase.CreateAsset(asset, "Assets/RealityFlowConfiguration.asset");
            AssetDatabase.SaveAssets();

            //EditorUtility.FocusProjectWindow();

            //Selection.activeObject = asset;
           // GameObject bemObject = GameObject.FindGameObjectWithTag("BehaviourEventManager");
            if(eventArgs.message.WasSuccessful == true)
            {
                ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;

                // Clear the behaviour list and BEM
                BehaviourEventManager.Clear();
                BehaviourEventManager.Initialize();


                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);

                if(eventArgs.message.flowProject.behaviourList == null)
                {
                    Debug.Log("It's null huh");
                }
                else
                {
                    Debug.Log("ya yeet");
                }

                foreach(FlowBehaviour fb in eventArgs.message.flowProject.behaviourList)
                {
                    BehaviourEventManager.CreateNewBehaviour(fb);
                }

                Debug.Log("Number of behaviours in bem = " + BehaviourEventManager.BehaviourList.Count);

                //foreach (FlowBehaviour fb in BehaviourEventManager.BehaviourList.Values)
                //{
                //    Debug.Log(fb.flowAction.ActionType );
                //    if(fb.flowAction.ActionType != "NoAction")
                //    {
                //        Debug.Log("teleport coordinates is " + fb.flowAction.teleportCoordinates.coordinates.x);
                //    }
                //}
            }
            
        }

        private static void _LeaveProject(object sender, BaseReceivedEventArgs eventArgs)
        {
            if(eventArgs.message.WasSuccessful == true)
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

        #endregion Project messages received

        #region Room messages received
        private static void _JoinRoom(object sender, BaseReceivedEventArgs eventArgs)
        {
                
        }

        private static void _UserLeftRoom(object sender, BaseReceivedEventArgs eventArgs)
        {
            Debug.Log("Room Alert: " + eventArgs.message.leftRoomMessage);
        }
        #endregion Room messages received

        #region User messages received
        private static void _LoginUser(object sender, BaseReceivedEventArgs eventArgs)
        {
                
        }

        private static void _LogoutUser(object sender, BaseReceivedEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = null;
            ConfigurationSingleton.SingleInstance.CurrentUser = null;
        }

        private static void _RegisterUser(object sender, BaseReceivedEventArgs eventArgs)
        {
            _FlowWebsocket.Disconnect();
        }
    #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
