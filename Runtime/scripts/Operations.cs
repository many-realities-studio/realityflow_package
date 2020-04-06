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
        public static FlowWebsocket FlowWebsocket { get; private set; }
        public static object FlowNetworkManagerEditor { get; private set; }

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

            // Set up Behaviour updates
            CreateBehaviour_Received.ReceivedEvent += _CreateBehaviour;
            DeleteBehaviour_Received.ReceivedEvent += _DeleteBehaviour;
            UpdateBehaviour_Received.ReceivedEvent += _UpdateBehaviour;

        }

        #region UserOperations

        public static void Login(FlowUser flowUser, LoginUser_Received.LoginReceived_EventHandler callbackFunction)
        {
            Login_SendToServer loginMessage = new Login_SendToServer(flowUser);
            FlowWebsocket.SendMessage(loginMessage);

            ConfigurationSingleton.CurrentUser = flowUser;

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

        public static void CreateBehaviour(FlowBehaviour behaviour, string projectId, CreateBehaviour_Received.CreateBehaviourReceived_EventHandler callbackFunction)
        {
            CreateBehaviour_SendToServer createBehaviour = new CreateBehaviour_SendToServer(behaviour, projectId);
            FlowWebsocket.SendMessage(createBehaviour);

            CreateBehaviour_Received.ReceivedEvent += callbackFunction;
        }

        

        public static void DeleteBehaviour(FlowBehaviour behaviour, string behaviourId, string projectId, DeleteBehaviour_Received.DeleteBehaviourReceived_EventHandler callbackFunction)
        {
            DeleteBehaviour_SendToServer deleteBehaviour = new DeleteBehaviour_SendToServer(behaviour, behaviourId, projectId);
            FlowWebsocket.SendMessage(deleteBehaviour);

            DeleteBehaviour_Received.ReceivedEvent += callbackFunction;
        }

        public static void UpdateBehaviour(FlowBehaviour behaviour, string projectId, UpdateBehaviour_Received.UpdateBehaviourReceived_EventHandler callbackFunction)
        {
            UpdateBehaviour_SendToServer updateBehaviour = new UpdateBehaviour_SendToServer(behaviour, projectId);
            FlowWebsocket.SendMessage(updateBehaviour);

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
        public static void ConnectToServer(string url)
        {
            FlowWebsocket = new FlowWebsocket(url);
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
            // this is where things happen after a createBehaviour message is deserialized
            FlowBehaviour fb = eventArgs.message.flowBehaviour;
            string behaviourName = fb.TypeOfTrigger;
            
            if (fb.flowAction.Equals("empty"))
            {
                // The TypeOfTrigger would be "Immediate", so we use ActionType instead 
                behaviourName = fb.flowAction.ActionType;
            }

            ObjectIsInteractable oIsIFirst = FindAndMakeInteractable(fb.TriggerObjectId);
            ObjectIsInteractable oIsISecond = FindAndMakeInteractable(fb.TargetObjectId);
            
            if(oIsIFirst == null || oIsISecond == null)
            {
                Debug.Log("There is a missing gameobject. Failed to make Interaction.");
                return;
            }

            BehaviourEvent newBehaviour = BehaviourEventManager.CreateNewBehaviourEvent(behaviourName, fb.Id, oIsIFirst.GetGuid(), oIsISecond.GetGuid(), null);

            if(newBehaviour == null)
            {
                Debug.Log("Failed to create new behaviour");
                return;
            }

            BehaviourEventManager.BehaviourList.Add(newBehaviour.Id, newBehaviour);
        }


        /// <summary>
        /// Finds the gameobject associated with objectId, and adds an ObjectIsInteractable component to it.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static ObjectIsInteractable FindAndMakeInteractable(string objectId)
        {
            if(FlowTObject.idToGameObjectMapping.TryGetValue(objectId, out FlowTObject foundObject))
            {
                Debug.Log("Found object " + foundObject.Id + " ... making interactable");
                ObjectIsInteractable oIsI = BehaviourEventManager.MakeObjectInteractable(foundObject.AttachedGameObject, objectId);
                return oIsI;
            }

            else
            {
                Debug.Log("Cannot make object interactable. Object not found in project.");
                return null;
            }
        }



        //public static BehaviourEvent SetBehaviour(FlowBehaviour fb, BehaviourEvent chainedBehaviour)
        //{
        //    // this is where things happen after a createBehaviour message is deserialized

        //    FlowTObject.idToGameObjectMapping.TryGetValue(fb.TriggerObjectId, out FlowTObject firstobject);
        //    FlowTObject.idToGameObjectMapping.TryGetValue(fb.TargetObjectId, out FlowTObject secondobject);

        //    Debug.Log("FIRSTOBJECT");
        //    Debug.Log(fb.TriggerObjectId);

        //    GameObject firstGameObject = firstobject.AttachedGameObject;
        //    GameObject secondGameObject = secondobject.AttachedGameObject;

        //    //ObjectIsInteractable oIsIFirst = firstObj.GetComponent<ObjectIsInteractable>();
        //    //ObjectIsInteractable oIsISecond = secondObj.GetComponent<ObjectIsInteractable>();

        //    //if (oIsIFirst == null)
        //    //{

        //    ObjectIsInteractable oIsIFirst = BehaviourEventManager.MakeObjectInteractable(firstGameObject, fb.TriggerObjectId);
        //    ObjectIsInteractable oIsISecond = BehaviourEventManager.MakeObjectInteractable(secondGameObject, fb.TargetObjectId);
            
        //    //Debug.Log("GUID IS");
        //    //Debug.Log(oIsIFirst.GetGuid().ToString());


        //    BehaviourEvent newBehaviour = BehaviourEventManager.CreateNewBehaviourEvent(fb.TypeOfTrigger, oIsIFirst.GetGuid(), oIsISecond.GetGuid(), chainedBehaviour);

        //    if(newBehaviour == null)
        //    {
        //        Debug.Log("The behaviour is NULL");
        //    }
        //    else
        //    {
        //        Debug.Log("Behaviour Created!");
        //        //Debug.Log(newBehaviour.GetName());
        //        Debug.Log(newBehaviour.GetSecondObject());
        //    }
        //    return newBehaviour;
        //}

        private static void _DeleteBehaviour(object sender, DeleteBehaviourEventArgs eventArgs)
        {
            // this is where things happen after a DeleteBehaviour message is deserialized
        }

        private static void _UpdateBehaviour(object sender, UpdateBehaviourEventArgs eventArgs)
        {
            // this is where things happen after an UpdateBehaviour message is deserialized
            // do the same as createBehaviour?

            // this is where things happen after a createBehaviour message is deserialized
            FlowBehaviour fb = eventArgs.message.flowBehaviour;

            // For those receiving updateBehaviour messages, if they did not remove it from 
            // the list already, then it still exists on the objects
            if(BehaviourEventManager.BehaviourList.ContainsKey(fb.Id))
            {
                if (BehaviourEventManager.BehaviourList.TryGetValue(fb.Id, out BehaviourEvent outdatedBehaviourEvent))
                {
                    BehaviourEventManager.BehaviourList.Remove(outdatedBehaviourEvent.Id);
                    BehaviourEventManager.DeleteBehaviourEvent(outdatedBehaviourEvent.GetFirstObject(), outdatedBehaviourEvent.GetSecondObject(), outdatedBehaviourEvent);
                    UnityEngine.Object.Destroy(outdatedBehaviourEvent);
                }
            }


            string behaviourName = fb.TypeOfTrigger;

            if (fb.flowAction.Equals("empty"))
            {
                // The TypeOfTrigger would be "Immediate", so we use ActionType instead 
                behaviourName = fb.flowAction.ActionType;
            }

            ObjectIsInteractable oIsIFirst = FindAndMakeInteractable(fb.TriggerObjectId);
            ObjectIsInteractable oIsISecond = FindAndMakeInteractable(fb.TargetObjectId);

            if (oIsIFirst == null || oIsISecond == null)
            {
                Debug.Log("There is a missing gameobject. Failed to make Interaction.");
                return;
            }

            BehaviourEvent newBehaviour = BehaviourEventManager.CreateNewBehaviourEvent(behaviourName, fb.Id, oIsIFirst.GetGuid(), oIsISecond.GetGuid(), null);

            if (newBehaviour == null)
            {
                Debug.Log("Failed to create new behaviour");
                return;
            }

            BehaviourEventManager.BehaviourList.Add(newBehaviour.Id, newBehaviour);
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
            ConfigurationSingleton.CurrentProject = eventArgs.message.flowProject;

           // GameObject bemObject = GameObject.FindGameObjectWithTag("BehaviourEventManager");

            /*if (bemObject == null)
            {
                bemObject = new GameObject();
                bemObject.AddComponent<BehaviourEventManager>();
                bemObject.name = "BehaviourEventManager";
                bemObject.tag = "BehaviourEventManager";
            }

            BehaviourEventManager bem = bemObject.GetComponent<BehaviourEventManager>();
            bem.Initialize();*/

            BehaviourEventManager.Clear();
            BehaviourEventManager.Initialize();

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
                
        }
    #endregion User messages received

        #endregion Default actions taken after receiving messages (These happen no matter what the user does)
    }
}
