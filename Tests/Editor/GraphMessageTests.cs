using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using RealityFlow.Plugin.Scripts;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;
// ARRANGE, ACT, ASSERT
namespace RealityFlow.Plugin.Tests
{
    public class GraphMessageTests
    {
        const int messageTimeout = 4000; // Timeout for a response from the server in milliseconds

        [OneTimeSetUp]
        public void Setup()
        {
                    var mockDependency = new Mock<FlowTObject>();
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            
        }
        [Test]
        public void Test()
        {

        }
    }
}