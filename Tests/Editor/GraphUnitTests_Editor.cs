using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using RealityFlow.Plugin.Contrib;
using RealityFlow.Plugin.Scripts;
using Packages.realityflow_package.Runtime.scripts;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;
using GraphProcessor;
using NodeGraphProcessor.Examples;
// ARRANGE, ACT, ASSERT
namespace RealityFlow.Plugin.Tests
{
    public class GraphUnitTests
    {
        const int messageTimeout = 4000; // Timeout for a response from the server in milliseconds
        FlowProject testProject;
        FlowVSGraph testGraph;
        [OneTimeSetUp]
        public void Setup()
        {
            var testUser = new Mock<FlowUser>();
            testProject = new FlowProject("noRoom", "Description", 0, "TestProject");
            testGraph = new FlowVSGraph("id","testGraph",new List<JsonElement>(),new List<SerializableEdge>(),new List<Group>(),new List<BaseStackNode>(),new List<PinnedElement>(),"[]",new List<StickyNote>(),new Vector3(0,0,0),new Vector3(1,1,1),"{\"keys\":[],\"values\":[]}");
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            
        }
        
        // Graph tests
        [UnityTest]
        public IEnumerator GraphCreationTest()
        {
            bool WasSuccessful = false;
            FlowVSGraph test = new FlowVSGraph("Testy");
            for(int i = 0; i <= 1; i++)
            {
                WasSuccessful = (test.Name == "Testy"?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.Id != "id"?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.serializedNodes.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.edges.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.groups.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.stackNodes.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.pinnedElements.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.exposedParameters.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.stickyNotes.Count == 0?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.position == new Vector3()?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.scale == new Vector3(1.0f,1.0f,1.0f)?true:false);
                if(!WasSuccessful)
                    break;
                WasSuccessful = (test.paramIdToObjId.Count == 0?true:false);
                break;
            }
            Assert.IsTrue(WasSuccessful);
            yield return null;
        }
        [UnityTest]
        public IEnumerator DeleteAllGraphsTest()
        {
            FlowVSGraph test = new FlowVSGraph("Testy");
            FlowVSGraph test2 = new FlowVSGraph("Test");
            FlowVSGraph test3 = new FlowVSGraph("TTest");
            bool graphsCleared = false;
            FlowVSGraph.RemoveAllGraphsFromScene();
            if(FlowVSGraph.idToVSGraphMapping.Count == 0)
            {
                graphsCleared = true;
            }
            Assert.IsTrue(graphsCleared);
            yield return null;
        }
        // [UnityTest]
        // public IEnumerator ClearGraphTest()
        // {
        //     yield return null;
        // }

        // Node tests
        [UnityTest]
        public IEnumerator AddNodeTest()
        {
            bool nodeAdded = false;
            FlowVSGraph test = new FlowVSGraph("test");
            BaseNode node = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            test.AddNode(node);
            if(test.nodes.Count == 1)
            {
                nodeAdded = true;
            }
            Assert.IsTrue(nodeAdded);
            yield return null;
        }
        [UnityTest]
        public IEnumerator DeleteNodeTest()
        {
            bool nodeDeleted = false;
            FlowVSGraph test = new FlowVSGraph("test");
            BaseNode node = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            test.AddNode(node);
            test.RemoveNode(node);
            if(test.nodes.Count == 0)
            {
                nodeDeleted = true;
            }
            Assert.IsTrue(nodeDeleted);
            yield return null;
        }

        // Edge tests
        [UnityTest]
        public IEnumerator AddEdgeTest()
        {
            bool edgeAdded = false;
            FlowVSGraph test = new FlowVSGraph("test");
            BaseNode nodeA = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            BaseNode nodeB = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            test.AddNode(nodeA);
            test.AddNode(nodeB);
            SerializableEdge edge = test.Connect(nodeA.inputPorts[0], nodeB.outputPorts[0]);
            if(edge != null)
            {
                edgeAdded = true;
            }
            Assert.IsTrue(edgeAdded);
            yield return null;
        }
        [UnityTest]
        public IEnumerator DeleteEdgeTest()
        {
            bool edgeDeleted = false;
            FlowVSGraph test = new FlowVSGraph("test");
            BaseNode nodeA = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            BaseNode nodeB = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
            test.AddNode(nodeA);
            test.AddNode(nodeB);
            SerializableEdge edge = test.Connect(nodeA.inputPorts[0], nodeB.outputPorts[0]);
            test.Disconnect(edge);
            if ( test.edges.Count == 0 ){
                edgeDeleted = true;
            }
            Assert.IsTrue(edgeDeleted);
            yield return null;
        }

        // Parameter tests
        [UnityTest]
        public IEnumerator AddExposedParameterTest()
        {
            bool parameterAdded = false;
            FlowVSGraph test = new FlowVSGraph("test");
            test.AddExposedParameter("testParam",typeof(GameObject),null);
            if (test.exposedParameters.Count == 1){
                parameterAdded = true;
            }
            Assert.IsTrue(parameterAdded);
            yield return null;
        }
        [UnityTest]
        public IEnumerator DeleteParameterTest()
        {
            bool parameterDeleted = false;
            FlowVSGraph test = new FlowVSGraph("test");
            string epnGUID = test.AddExposedParameter("testParam",typeof(GameObject),null);
            test.RemoveExposedParameter(epnGUID);
            if (test.exposedParameters.Count == 0){
                parameterDeleted = true;
            }
            Assert.IsTrue(parameterDeleted);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ModifyParameterValueTest()
        {
            bool parameterModified = false;
            FlowVSGraph test = new FlowVSGraph("test");
            string epnGUID = test.AddExposedParameter("testParam",typeof(string),null);
            ExposedParameter p = test.GetExposedParameterFromGUID(epnGUID);
            object newValue = new SerializableObject("Hello world",typeof(object),null);
            p.serializedValue.value = newValue;
            if (p.serializedValue.value == newValue){
                parameterModified = true;
            }
            Assert.IsTrue(parameterModified);
            yield return null;
        }

    }
}