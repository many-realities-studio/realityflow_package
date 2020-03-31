using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RealityFlow.Plugin.Tests
{
    public class ProjectMessageTests
    {
        int messageTimeout = 3000;
        FlowProject testProject;
        FlowTObject testObject;
        FlowUser testUser;

        [OneTimeSetUp]
        public void Setup()
        {
            string url = "ws://echo.websocket.org";
            Operations.ConnectToServer(url);

            testObject = new FlowTObject("id", 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, "name", "");
            testProject = new FlowProject("flowId", "description", 0, "projectName"/*, new FlowTObject[]
            {
                testObject
            }*/);

            testUser = new FlowUser("user", "pass");
        }

        [Test]
        public void CreateProjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            CreateProject_Received expected = new CreateProject_Received(null, true);

            // Act (and assert)
            ConfirmationMessage_Received actual = null;
            Operations.CreateProject(testProject, testUser, (sender, e) =>
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

        [Test]
        public void DeleteProjectTest()
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

        [Test]
        public void OpenProjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            OpenProject_Received expected = new OpenProject_Received(testProject);

            // Act (and assert)
            OpenProject_Received actual = null;
            Operations.OpenProject(testProject.Id, testUser, (sender, e) =>
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

        [Test]
        public void GetAllUserProjectsTest()
        {
            //// Arrange
            //AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            //GetAllUserProjects_Received expected = new GetAllUserProjects_Received(new Tuple<string, string>[] 
            //{
            //    new Tuple<string, string>(testProject.Id, testProject.ProjectName)
            //});

            //// Act (and assert)
            //GetAllUserProjects_Received actual = null;
            //Operations.GetAllUserProjects(testUser, (sender, e) =>
            //{
            //    Debug.Log("Received message: " + e.message.ToString());
            //    actual = e.message;
            //    autoResetEvent.Set();
            //});

            //// Wait for 3 seconds for a response
            //Assert.IsTrue(autoResetEvent.WaitOne(messageTimeout));
            //Debug.Log("actual = " + actual?.ToString());
            //Assert.AreEqual(expected, actual);
        }
    }
}
