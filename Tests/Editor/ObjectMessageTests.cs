using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
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
    public class ObjectMessageTests
    {
        FlowProject testProject;
        FlowTObject testObject;
        FlowUser testUser;
        string url;
        IList<FlowProject> _ProjectList = null;

        [OneTimeSetUp]
        public void Setup()
        {
            IList<FlowProject> _ProjectList = null;
            string url = "ws://localhost:8999";
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
                            Debug.Log("WAITING3");
                        }
                        _ProjectList = _f.message.Projects;
                        Debug.Log("Project list = " + _ProjectList);
                    });
                }
                else
                {
                    ConfigurationSingleton.SingleInstance.CurrentUser = null;
                }
            });

            ConfigurationSingleton.SingleInstance.CurrentProject = new FlowProject("GUID", "description", 0, "testingproject2");

            Operations.CreateProject(ConfigurationSingleton.SingleInstance.CurrentProject, ConfigurationSingleton.SingleInstance.CurrentUser, (_, g) =>
            {
                if (g.message.WasSuccessful == true)
                {
                    Debug.Log(g.message);
                    Debug.Log("FINISHED CREATING PROJECT IN TEST");
                }
                else
                {
                    ConfigurationSingleton.SingleInstance.CurrentProject = null;
                }
            });

            Debug.Log("Project ID after CreateProject: " + ConfigurationSingleton.SingleInstance.CurrentProject.Id);
            

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

        /// <summary>
        /// This test verifies that the server responds with the expected answer
        /// </summary>
        [UnityTest]
        public IEnumerator CreateObjectTest()
        {
            // Arrange
            CreateObject_Received expected = new CreateObject_Received(testObject);
            // Act (and assert)
            CreateObject_Received actual = null;
            
            Debug.Log("Project ID in create object test: " + ConfigurationSingleton.SingleInstance.CurrentProject.Id);
            while(String.Equals(ConfigurationSingleton.SingleInstance.CurrentProject.Id, "GUID"))
            {
                Debug.Log("Waiting on Project Creation");
                yield return null;
            }

            Operations.CreateObject(testObject,/*projectId*/ConfigurationSingleton.SingleInstance.CurrentProject.Id, (sender, e) =>
            {
                Debug.Log("Received message: " + e.message.ToString());
                actual = e.message;
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

        /*
        [UnityTest]
        public IEnumerator UpdateObjectTest()
        {
            CreateObjectTest();
            // Arrange
            UpdateObject_Received expected = new UpdateObject_Received(testObject);

            // Act (and assert)
            UpdateObject_Received actual = null;

            Operations.UpdateObject(ConfigurationSingleton.SingleInstance.CurrentProject._ObjectList.First(), ConfigurationSingleton.SingleInstance.CurrentUser, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (sender, e) =>

            {
                Debug.Log("Received message: " + e.message.ToString());
                actual = e.message;
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
        */

        /*
        [UnityTest]
        public IEnumerator DeleteObjectTest()
        {
            // Arrange
            DeleteObject_Received expected = new DeleteObject_Received();

            // Act (and assert)
            DeleteObject_Received actual = null;
            Operations.DeleteObject(testObject.Id, testProject.Id, (sender, e) =>
            {
                Debug.Log("Received message: " + e.message.ToString());
                actual = e.message;
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
        */
    }
}