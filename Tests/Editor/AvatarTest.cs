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
public class AvatarTest
{
    const int messageTimeout = 4000; // Timeout for a response from the server in milliseconds
    FlowProject testProject;
    FlowAvatar testAvatar;
    [OneTimeSetUp]
    public void Setup()
    {
        var testUser = new Mock<FlowUser>();
        testProject = new FlowProject("noRoom", "Description", 0, "TestProject");
        Transform head = GameObject.Find("Main Camera").transform;
        testAvatar = new FlowAvatar(head);

        // GameObject emptyAvatar = new GameObject();
        // Transform newTransform = emptyAvatar.transform;

        // testAvatar= new FlowAvatar(newTransform);

        // FlowAvatar createAvatar = new FlowAvatar(head);
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        
    }

    // // A Test behaves as an ordinary method
    // [Test]
    // public void AvatarTestSimplePasses()
    // {
    //     // Use the Assert class to test conditions
    // }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.

    /*
    Transform head = GameObject.Find("Main Camera").transform;
                        FlowAvatar createAvatar = new FlowAvatar(head);
                        Operations.CreateAvatar(createAvatar, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, f) => { Debug.Log(f.message); });
    
    */
    [UnityTest]
    public IEnumerator CreateAvatar()
    {
        bool WasSuccessful = false;
        Transform head = GameObject.Find("Main Camera").transform;
        FlowAvatar test = new FlowAvatar(head);
        for(int i = 0; i <= 1; i++)
        {
            WasSuccessful = (test.Id != "id"?true:false);
            if(!WasSuccessful)
                break;
            // WasSuccessful = (test.x == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.y == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.z == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.Q_x == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.Q_y == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.Q_z == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            // WasSuccessful = (test.Q_w == 0?true:false);
            // if(!WasSuccessful)
            //     break;
            WasSuccessful = (test.Position == new Vector3()?true:false);
            if(!WasSuccessful)
                break;
            WasSuccessful = (test.Rotation == Quaternion.identity?true:false);
            if(!WasSuccessful)
                break;
        }
        Assert.IsTrue(WasSuccessful);
        yield return null;
    }

    [UnityTest]
    public IEnumerator DeleteAllAvatarsTest()
    {
      Transform head = GameObject.Find("Main Camera").transform;
      FlowAvatar test = new FlowAvatar(head);
      FlowAvatar test2 = new FlowAvatar(head);
      FlowAvatar test3 = new FlowAvatar(head);
      bool avatarsCleared = false;
      // TODO: This doesn't exist
      // FlowVSGraph.RemoveAllAvatarsFromScene();
      // if(FlowVSGraph.idToAvatarMapping.Count == 0)
      // {
      //     avatarsCleared = true;
      // }
      // Assert.IsTrue(avatarsCleared);
      yield return null;

    }
}
