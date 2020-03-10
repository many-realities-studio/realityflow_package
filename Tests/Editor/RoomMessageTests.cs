using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;
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
    public class RoomMessageTests
    {
        private FlowProject testProject;
        int messageTimeout = 3000;
        FlowTObject testObject;
        FlowUser testUser;

        [OneTimeSetUp]
        public void Setup()
        {
            string url = "ws://echo.websocket.org";
            Operations.ConnectToServer(url);

            testObject = new FlowTObject(new Color(0, 0, 0), "FlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "name");
            testProject = new FlowProject("flowId", "description", 0, "projectName", new FlowTObject[]
            {
                testObject
            });

            testUser = new FlowUser("user", "pass");
        }

        [Test]
        public void JoinRoomTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            JoinRoom_Received expected = new JoinRoom_Received(testProject);

            // Act (and assert)
            JoinRoom_Received actual = null;
            Operations.JoinRoom(testProject.FlowId, testUser, (sender, e) =>
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
