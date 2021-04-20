// using System.Collections;
// using System.Collections.Generic;
// using System.Threading;
// using NUnit.Framework;
// using Packages.realityflow_package.Runtime.scripts;
// using Packages.realityflow_package.Runtime.scripts.Messages;
// using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
// using RealityFlow.Plugin.Contrib;
// using RealityFlow.Plugin.Scripts;
// using Packages.realityflow_package.Runtime.scripts;
// using UnityEngine;
// using UnityEngine.TestTools;
// using Moq;
// using GraphProcessor;
// using NodeGraphProcessor.Examples;
// public class AvatarTest
// {
//     const int messageTimeout = 4000; // Timeout for a response from the server in milliseconds
//     FlowProject testProject;
//     [OneTimeSetUp]
//     public void Setup()
//     {
//         var testUser = new Mock<FlowUser>();
//         testProject = new FlowProject("noRoom", "Description", 0, "TestProject");

//         GameObject emptyAvatar = new GameObject();
//         Transform newTransform = emptyAvatar.transform;

//         testAvatar= new FlowAvatar(newTransform);
//     }
    
//     [OneTimeTearDown]
//     public void TearDown()
//     {
        
//     }

//     // A Test behaves as an ordinary method
//     [Test]
//     public void AvatarTestSimplePasses()
//     {
//         // Use the Assert class to test conditions
//     }

//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator CreateAvatar()
//     {
//         bool WasSuccessful = false;
//         FlowAvatar test = new FlowAvatar();
//         for(int i = 0; i <= 1; i++)
//         {
//             WasSuccessful = (test.Name == "Testy"?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.Id != "id"?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.serializedNodes.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.edges.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.groups.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.stackNodes.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.pinnedElements.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.exposedParameters.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.stickyNotes.Count == 0?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.position == new Vector3()?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.scale == new Vector3(1.0f,1.0f,1.0f)?true:false);
//             if(!WasSuccessful)
//                 break;
//             WasSuccessful = (test.paramIdToObjId.Count == 0?true:false);
//             break;
//         }
//         Assert.IsTrue(WasSuccessful);
//         yield return null;
//     }
// }
