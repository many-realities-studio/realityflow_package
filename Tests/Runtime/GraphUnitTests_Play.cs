using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using GraphProcessor;
using RealityFlow.Plugin.Scripts;
using RealityFlow.Plugin.Contrib;

public class GraphUnitTests_Play : MonoBehaviour
{
        const int messageTimeout = 4000; // Timeout for a response from the server in milliseconds
        FlowProject testProject;
        FlowVSGraph testGraph;
        public void Setup()
        {
            testProject = new FlowProject("noRoom", "Description", 0, "TestProject");
            testGraph = new FlowVSGraph("id","testGraph",new List<JsonElement>(),new List<SerializableEdge>(),new List<Group>(),new List<BaseStackNode>(),new List<PinnedElement>(),"[]",new List<StickyNote>(),new Vector3(0,0,0),new Vector3(1,1,1),"{\"keys\":[],\"values\":[]}");
        }
        [UnityTest]
        public IEnumerator GraphDeletionTest()
        {
            FlowVSGraph test = new FlowVSGraph("Testy");
            int graphCount = FlowVSGraph.idToVSGraphMapping.Count - 1;
            bool graphDeleted = false;
            FlowVSGraph.DestroyVSGraph(test.Id);
            if(FlowVSGraph.idToVSGraphMapping.Count == graphCount)
            {
                graphDeleted = true;
            }
            Assert.IsTrue(graphDeleted);
            yield return null;
        }
}
