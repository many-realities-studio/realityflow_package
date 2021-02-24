using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
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
    public class ProjectMessageTests
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
        public IEnumerator CreateProjectTest()
        {
            bool projectCreated = false;
            // Arrange
            FlowProject createTestProject = new FlowProject("GUID", "Description", 0, "createTestProject");
            // Act (and assert)
            ConfirmationMessage_Received actual = null;

            // Broke down in operations.
            Operations.CreateProject(createTestProject, ConfigurationSingleton.SingleInstance.CurrentUser, (sender, e) =>
            {
                if (e.message.WasSuccessful != true)
                {
                    Assert.AreEqual(e.message.WasSuccessful, true);
                }
                Debug.Log("Received message: " + e.message.ToString());
                actual = e.message;
                Debug.Log(actual);
                projectCreated = e.message.WasSuccessful;
            });

            while(actual == null) {
               Debug.Log(actual);
                yield return null;
            }

            Assert.AreEqual(projectCreated, true);
        }

        /*
        [UnityTest]
        public IEnumerator DeleteProjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            DeleteProject_Received expected = new DeleteProject_Received(true);

            // Act (and assert)
            ConfirmationMessage_Received actual = null;
            Operations.DeleteProject(testProject, testUser, (sender, e) =>
            {
                Debug.Log("Received message: " + e.message.ToString());
                actual = e.message;
                autoResetEvent.Set();
            });

            // Wait for 3 seconds for a response
            Assert.IsTrue(autoResetEvent.WaitOne(messageTimeout));
            Debug.Log("actual = " + actual?.ToString());
            Assert.AreEqual(expected, actual);
        }
        */

        // This test is the same as the JoinRoomTest as Join Room uses the OpenProject function call
        // to do it's work.
        [UnityTest]
        public IEnumerator OpenProjectTest()
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

        // It's not possible right now to write a test that returns the expected list of projects
        // as the server responds to the query with the list of projects that a user has
        // associated to them.
        /*
        [UnityTest]
        public IEnumerator GetAllUserProjectsTest()
        {
            //// Arrange
            GetAllUserProjects_Received expected = new GetAllUserProjects_Received(IList<FlowProject>);

            // Act (and assert)
            GetAllUserProjects_Received actual = null;
            Operations.GetAllUserProjects(testUser, (sender, e) =>
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
