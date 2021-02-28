using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace RealityFlow.Plugin.Tests
{
    public class RoomMessageTests
    {
        FlowProject testProject;
        FlowTObject testObject;
        FlowUser testUser;
        string url;
        IList<FlowProject> _ProjectList = null;
        
        [OneTimeSetUp]
        public void Setup()
        {
            string url = "ws://localhost:8999";
            // Change these based on user.
            string usernm = "Morgan";
            string passwrd = "Lovered98";

            ConfigurationSingleton.SingleInstance.CurrentUser = new FlowUser(usernm, passwrd);
            Operations.Login(ConfigurationSingleton.SingleInstance.CurrentUser, url, (_, f) =>
            {
                Debug.Log("login callback: " + f.message.WasSuccessful.ToString());
                if (f.message.WasSuccessful == true)
                {
                    Operations.GetAllUserProjects(ConfigurationSingleton.SingleInstance.CurrentUser, (__, _f) =>
                    {
                        while(_f.message == null)
                        {
                            Debug.Log("WAITING");
                        }
                        _ProjectList = _f.message.Projects;
                        Debug.Log("Project list = " + _ProjectList);
                        testProject = _ProjectList[0];
                        testProject._ObjectList = new List<FlowTObject>();
                        testProject.behaviourList = new List<FlowBehaviour>();
                        //Debug.Log(JsonUtility.ToJson(testProject));
                    });
                }
                else
                {
                    ConfigurationSingleton.SingleInstance.CurrentUser = null;
                }
            });

            testObject = new FlowTObject("id", 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, "name", "Pig");
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Operations.LeaveProject(ConfigurationSingleton.SingleInstance.CurrentProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
            {
                if (e.message.WasSuccessful == true)
                {
                    ConfigurationSingleton.SingleInstance.CurrentProject = null;
                    FlowTObject.RemoveAllObjectsFromScene();
                }
                else
                {
                    Debug.LogWarning("Error leaving project.");
                }
            });

            Debug.Log("Deleting project with ID " + ConfigurationSingleton.SingleInstance.CurrentProject.Id);
            Operations.DeleteProject(ConfigurationSingleton.SingleInstance.CurrentProject, ConfigurationSingleton.SingleInstance.CurrentUser, (_, f) =>
            {
                Debug.Log("Received message: " + f.message.ToString());
            });

            ConfigurationSingleton.SingleInstance.CurrentProject = null;
            ConfigurationSingleton.SingleInstance.CurrentUser = null;
        }


        [UnityTest]
        public IEnumerator JoinRoomTest()
        {
            while(testProject == null) {
                Debug.Log("Test Project = " +testProject);
                yield return null;
            }

            OpenProject_Received expected = new OpenProject_Received(testProject, true);
            OpenProject_Received actual = null;

            // Act (and assert)
            Operations.OpenProject(testProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (sender, e) =>
            {
                actual = e.message;
                Debug.Log("Actual = " + actual?.ToString());
            });

            while(actual == null) {
                Debug.Log(actual);
                yield return null;
            }
            // Wait for 3 seconds for a response
            string jsonActual = MessageSerializer.SerializeMessage(actual);
            string jsonExpected = MessageSerializer.SerializeMessage(expected);
            Debug.Log("actual " + jsonActual);
            Debug.Log("expected " + jsonExpected);
            Assert.AreEqual(jsonExpected, jsonActual);
        }

    }
}
