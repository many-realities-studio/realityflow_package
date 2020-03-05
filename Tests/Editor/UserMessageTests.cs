using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using RealityFlow.Plugin.Scripts;
using UnityEngine;

namespace RealityFlow.Plugin.Tests
{
    public class UserMessageTests
    {
        const int messageTimeout = 3000; // Timeout for a response from the server in milliseconds
        [OneTimeSetUp]
        public void Setup()
        {
            string url = "ws://echo.websocket.org";
            Operations.ConnectToServer(url);
        }


        /// <summary>
        /// This test verifies that the server reponds with the expected answer
        /// </summary>
        [Test]
        public void ClientRegisterTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            RegisterUser_Received expected = new RegisterUser_Received(null, true);
            expected.WasSuccessful = true;

            // Act (and assert)
            ConfirmationMessage_Received actual = null;
            Operations.Register("user", "pass", (sender, e) => 
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
        public void ClientLoginTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            LoginUser_Received expected = new LoginUser_Received(null, true);
            expected.MessageType = "Login";
            expected.WasSuccessful = true;

            // Act (and assert)
            ConfirmationMessage_Received actual = null;
            Operations.Login("user", "pass", (sender, e) =>
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
        public void ClientLogoutTest()
        {
            // Arrange
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            LogoutUser_Received expected = new LogoutUser_Received(null, true);
            expected.WasSuccessful = true;

            // Act (and assert)
            ConfirmationMessage_Received actual = null;
            Operations.Login("user", "pass", (sender, e) =>
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