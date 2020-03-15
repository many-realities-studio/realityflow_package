using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using RealityFlow.Plugin.Scripts;
using UnityEngine;

namespace RealityFlow.Plugin.Tests
{
    public class ObjectMessageTests
    {
        const int messageTimeout = 3000; // Timeout for a response from the server in milliseconds
        FlowTObject testObject;
        FlowUser testUser;
        FlowProject testProject;

        [OneTimeSetUp]
        public void Setup()
        {
            string url = "ws://echo.websocket.org";
            Operations.ConnectToServer(url);

            testObject = new FlowTObject(new Color(0, 0, 0), "FlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "name");
            testUser = new FlowUser("user", "pass");
            testProject = new FlowProject("FlowProjectId", "Description", 0, "TestProject");
        }


        /// <summary>
        /// This test verifies that the server reponds with the expected answer
        /// </summary>
        [Test]
        public void CreateObjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            CreateObject_Received expected = new CreateObject_Received(testObject);

            // Act (and assert)
            CreateObject_Received actual = null;
            Operations.CreateObject(testObject, /*testUser,*/ "projectId", (sender, e) =>
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
        public void UpdateObjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            UpdateObject_Received expected = new UpdateObject_Received(testObject);

            // Act (and assert)
            UpdateObject_Received actual = null;
            Operations.UpdateObject(testObject, testUser, "projectId", (sender, e) =>
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
        public void DeleteObjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            DeleteObject_Received expected = new DeleteObject_Received(testObject);

            // Act (and assert)
            DeleteObject_Received actual = null;
            Operations.DeleteObject(testObject, testProject.FlowId, (sender, e) =>
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
        public void FinalizedUpdateObjectTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            FinalizedUpdateObject_Received expected = new FinalizedUpdateObject_Received(testObject);

            // Act (and assert)
            FinalizedUpdateObject_Received actual = null;
            Operations.FinalizedUpdateObject(testObject, testUser, "projectId", (sender, e) =>
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
    }
}