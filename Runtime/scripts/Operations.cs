//using Packages.realityflow_package.Runtime.scripts.Managers;
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
            UserLeftRoom_Received.ReceivedEvent += _UserLeftRoom;

            // Set up User updates
            LoginUser_Received.ReceivedEvent += _LoginUser;
            LogoutUser_Received.ReceivedEvent += _LogoutUser;
            RegisterUser_Received.ReceivedEvent += _RegisterUser;

            // Set up Behaviour updates
            CreateBehaviour_Received.ReceivedEvent += _CreateBehaviour;
            DeleteBehaviour_Received.ReceivedEvent += _DeleteBehaviour;
            UpdateBehaviour_Received.ReceivedEvent += _UpdateBehaviour;

        }

        #region UserOperations

        public static void Login(FlowUser flowUser, string url, LoginUser_Received.LoginReceived_EventHandler callbackFunction)
        {
            bool connectionSuccessful = ConnectToServer(url, flowUser);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                Login_SendToServer loginMessage = new Login_SendToServer(flowUser);
                FlowWebsocket.SendMessage(loginMessage);

                ConfigurationSingleton.SingleInstance.CurrentUser = flowUser;

                LoginUser_Received.ReceivedEvent += callbackFunction; 
            }
        }
        
        public static void Logout(FlowUser flowUser)
        {
            Logout_SendToServer logoutMessage = new Logout_SendToServer(flowUser);
            FlowWebsocket.SendMessage(logoutMessage);

            _FlowWebsocket.Disconnect();
        }

        public static void Register(string username, string password, string url,RegisterUser_Received.RegisterUserReceived_EventHandler callbackFunction)
        {
            bool connectionSuccssfull = ConnectToServer(url);

            if (FlowWebsocket.websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                RegisterUser_SendToServer register = new RegisterUser_SendToServer(new FlowUser(username, password));
                FlowWebsocket.SendMessage(register);

                RegisterUser_Received.ReceivedEvent += callbackFunction; 
            }
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

        public static void FinalizedUpdateObject(FlowTObject flowObject, FlowUser flowUser, string projectId, FinalizedUpdateObject_Received.FinalizedUpdateObjectRecieved_EventHandler callbackFunction)
        {
            FinalizedUpdateObject_SendToServer finalUpdateObject = new FinalizedUpdateObject_SendToServer(flowObject, projectId);
            FlowWebsocket.SendMessage(finalUpdateObject);

            FinalizedUpdateObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void DeleteObject(string idOfObjectToDelete, string projectId, DeleteObject_Received.DeleteObjectReceived_EventHandler callbackFunction)
        {
            DeleteObject_SendToServer deleteObject = new DeleteObject_SendToServer(projectId, idOfObjectToDelete);
            FlowWebsocket.SendMessage(deleteObject);

            DeleteObject_Received.ReceivedEvent += callbackFunction;
        }

        #endregion // ObjectOperations
        #region BehaviourOperations

        public static void CreateBehaviour(FlowBehaviour behaviour, string projectId, List<string> behavioursToLinkTo, CreateBehaviour_Received.CreateBehaviourReceived_EventHandler callbackFunction)
        {
            CreateBehaviour_SendToServer createBehaviour = new CreateBehaviour_SendToServer(behaviour, projectId, behavioursToLinkTo);
            _FlowWebsocket.SendMessage(createBehaviour);

            CreateBehaviour_Received.ReceivedEvent += callbackFunction;
        }

        

        public static void DeleteBehaviour(FlowBehaviour behaviour, string behaviourId, string projectId, DeleteBehaviour_Received.DeleteBehaviourReceived_EventHandler callbackFunction)
        {
            DeleteBehaviour_SendToServer deleteBehaviour = new DeleteBehaviour_SendToServer(behaviour, behaviourId, projectId);
            _FlowWebsocket.SendMessage(deleteBehaviour);

            DeleteBehaviour_Received.ReceivedEvent += callbackFunction;
        }

        public static void UpdateBehaviour(FlowBehaviour behaviour, string projectId, UpdateBehaviour_Received.UpdateBehaviourReceived_EventHandler callbackFunction)
        {
            UpdateBehaviour_SendToServer updateBehaviour = new UpdateBehaviour_SendToServer(behaviour, projectId);
            _FlowWebsocket.SendMessage(updateBehaviour);

            UpdateBehaviour_Received.ReceivedEvent += callbackFunction;
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
            //if(GameObject.FindObjectsOfType<GameObject>().Length != 0)
            //{
            //    Debug.LogError("Cannot load project if there are game objects already in scene. Delete all game objects and try again");
            //}
            //else
            //{
            OpenProject_SendToServer openProject = new OpenProject_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(openProject);
            
            OpenProject_Received.ReceivedEvent += callbackFunction;
            //}
        }

        public static void LeaveProject(string projectId, FlowUser flowUser, LeaveProject_Received.LeaveProjectReceived_EventHandler callbackFunction)
        {
            LeaveProject_SendToServer leaveProject = new LeaveProject_SendToServer(projectId, flowUser);
            FlowWebsocket.SendMessage(leaveProject);

            LeaveProject_Received.ReceivedEvent += callbackFunction;
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

        #region Checkout system messages

        public static void CheckoutObject(string objectID, string projectID, CheckoutObject_Received.CheckoutObjectReceived_EventHandler callbackFunction)
        {
            CheckoutObject_SendToServer checkoutObject = new CheckoutObject_SendToServer(objectID, projectID);
            FlowWebsocket.SendMessage(checkoutObject);

            CheckoutObject_Received.ReceivedEvent += callbackFunction;
        }

        public static void CheckinObject(string objectID, string projectID, CheckinObject_Received.CheckinObjectReceived_EventHandler callbackFunction)
        {
            CheckinObject_SendToServer checkinObject = new CheckinObject_SendToServer(objectID, projectID);
            FlowWebsocket.SendMessage(checkinObject);

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
          
          
          
          
          
        #region Behaviour messages received
        private static void _CreateBehaviour(object sender, CreateBehaviourEventArgs eventArgs)
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


        private static void _DeleteBehaviour(object sender, DeleteBehaviourEventArgs eventArgs)
        {
            // this is where things happen after a DeleteBehaviour message is deserialized
        }


        private static void _UpdateBehaviour(object sender, UpdateBehaviourEventArgs eventArgs)
        {
            if (eventArgs.message.WasSuccessful == true)
            {
                Debug.Log("Success updating behaviour " + eventArgs.message.flowBehaviour.TypeOfTrigger);

                BehaviourEventManager.UpdateBehaviour(eventArgs.message.flowBehaviour);
            }
        }

        #endregion




        #region Project messages received

        private static void _CreateProject(object sender, CreateProjectMessageArgs eventArgs)
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

        private static void _DeleteProject(object sender, ConfirmationMessageEventArgs eventArgs)
        {
                
        }

        private static void _GetAllUserProjects(object sender, GetAllUserProjectsMessageEventArgs eventArgs)
        {
                
        }

        private static void _OpenProject(object sender, OpenProjectMessageEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = eventArgs.message.flowProject;

            ConfigurationSingleton asset = ScriptableObject.CreateInstance<ConfigurationSingleton>();
            asset.CurrentProject = ConfigurationSingleton.SingleInstance.CurrentProject;
            asset.CurrentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
            AssetDatabase.CreateAsset(asset, "Assets/RealityFlowConfiguration.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
           // GameObject bemObject = GameObject.FindGameObjectWithTag("BehaviourEventManager");
            if(eventArgs.message.WasSuccessful == true)
            {
                ConfigurationSingleton.CurrentProject = eventArgs.message.flowProject;

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

                foreach (FlowBehaviour fb in BehaviourEventManager.BehaviourList.Values)
                {
                    Debug.Log(fb.flowAction.ActionType);
                }
            }
            
        }

        private static void _LeaveProject(object sender, LeaveProjectMessageEventArgs eventArgs)
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
        private static void _JoinRoom(object sender, JoinRoomMessageEventArgs eventArgs)
        {
                
        }

        private static void _UserLeftRoom(object sender, UserLeftRoomMessageEventArgs eventArgs)
        {
            Debug.Log("Room Alert: " + eventArgs.message.leftRoomMessage);
        }
        #endregion Room messages received

        #region User messages received
        private static void _LoginUser(object sender, LoginUserMessageEventArgs eventArgs)
        {
                
        }

        private static void _LogoutUser(object sender, ConfirmationMessageEventArgs eventArgs)
        {
            ConfigurationSingleton.SingleInstance.CurrentProject = null;
            ConfigurationSingleton.SingleInstance.CurrentUser = null;
        }

        private static void _RegisterUser(object sender, ConfirmationMessageEventArgs eventArgs)
        {
            _FlowWebsocket.Disconnect();
        }
    #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
