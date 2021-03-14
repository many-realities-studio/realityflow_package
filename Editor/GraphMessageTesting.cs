//using RealityFlow.Plugin.Scripts.Events;
using Packages.realityflow_package.Runtime.scripts;
//using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using RealityFlow.Plugin.Editor;
using RealityFlow.Plugin.Scripts;
//using RealityFlow.Plugin.Scripts;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

namespace RealityFlow.Plugin.Editor
{
    /// <summary>
    /// A new editor window to send testing graph messages to server
    /// </summary>
    public class GraphMessageTesting : EditorWindow
    {
        private static GraphMessageTesting window;

        public string CreateGraphMessage = ("{\n  \"FlowVSGraph\": {\n      \"serializedNodes\": [\n        {\n          \"type\": \"TextNode, RealityFlowPackage, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\",\n          \"jsonDatas\": \"{\\\"GUID\\\":\\\"d2915644-694d-4fbc-a7ea-95fe759c46f0\\\",\\\"computeOrder\\\":0,\\\"position\\\":{\\\"serializedVersion\\\":\\\"2\\\",\\\"x\\\":0.0,\\\"y\\\":0.0,\\\"width\\\":100.0,\\\"height\\\":100.0},\\\"expanded\\\":false,\\\"debug\\\":false,\\\"nodeLock\\\":false,\\\"output\\\":\\\"Hello World\\\"}\"\n        },\n        {\n          \"type\": \"SetLabelNode, RealityFlowPackage, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\",\n          \"jsonDatas\": \"{\\\"GUID\\\":\\\"f17b27eb-0453-48ed-a6b1-c44d7ea7caa2\\\",\\\"computeOrder\\\":2,\\\"position\\\":{\\\"serializedVersion\\\":\\\"2\\\",\\\"x\\\":0.0,\\\"y\\\":0.0,\\\"width\\\":100.0,\\\"height\\\":100.0},\\\"expanded\\\":false,\\\"debug\\\":false,\\\"nodeLock\\\":false,\\\"newLabel\\\":\\\"\\\",\\\"input\\\":{\\\"instanceID\\\":0}}\"\n        },\n        {\n          \"type\": \"GraphProcessor.ParameterNode, com.alelievr.NodeGraphProcessor.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\",\n          \"jsonDatas\": \"{\\\"GUID\\\":\\\"d1c7282f-91fc-4315-84b5-3f55bf5321d6\\\",\\\"computeOrder\\\":1,\\\"position\\\":{\\\"serializedVersion\\\":\\\"2\\\",\\\"x\\\":0.0,\\\"y\\\":0.0,\\\"width\\\":100.0,\\\"height\\\":100.0},\\\"expanded\\\":false,\\\"debug\\\":false,\\\"nodeLock\\\":false,\\\"parameterGUID\\\":\\\"2ff67e12-627b-447e-9439-b567c8bb1691\\\",\\\"accessor\\\":0}\"\n        }\n      ],\n      \"edges\": [\n        {\n          \"GUID\": \"54777eda-c5c1-43e0-bff8-0d7347c43bfb\",\n          \"owner\": {\n            \"instanceID\": 9716\n          },\n          \"inputNodeGUID\": \"f17b27eb-0453-48ed-a6b1-c44d7ea7caa2\",\n          \"outputNodeGUID\": \"d2915644-694d-4fbc-a7ea-95fe759c46f0\",\n          \"inputFieldName\": \"newLabel\",\n          \"outputFieldName\": \"output\",\n          \"inputPortIdentifier\": \"\",\n          \"outputPortIdentifier\": \"\"\n        },\n        {\n          \"GUID\": \"dacbac95-410f-476e-9e27-fcb956b6443a\",\n          \"owner\": {\n            \"instanceID\": 9716\n          },\n          \"inputNodeGUID\": \"f17b27eb-0453-48ed-a6b1-c44d7ea7caa2\",\n          \"outputNodeGUID\": \"d1c7282f-91fc-4315-84b5-3f55bf5321d6\",\n          \"inputFieldName\": \"input\",\n          \"outputFieldName\": \"output\",\n          \"inputPortIdentifier\": \"\",\n          \"outputPortIdentifier\": \"output\"\n        }\n      ],\n      \"groups\": [],\n      \"stackNodes\": [],\n      \"pinnedElements\": [\n        {\n          \"position\": {\n            \"serializedVersion\": \"2\",\n            \"x\": 0,\n            \"y\": 87,\n            \"width\": 226.99998474121094,\n            \"height\": 100\n          },\n          \"opened\": true,\n          \"editorType\": {\n            \"serializedType\": \"GraphProcessor.ExposedParameterView, com.alelievr.NodeGraphProcessor.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\"\n          }\n        },\n        {\n          \"position\": {\n            \"serializedVersion\": \"2\",\n            \"x\": 17,\n            \"y\": 221,\n            \"width\": 150,\n            \"height\": 200\n          },\n          \"opened\": false,\n          \"editorType\": {\n            \"serializedType\": \"ConditionalProcessorView, Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\"\n          }\n        },\n        {\n          \"position\": {\n            \"serializedVersion\": \"2\",\n            \"x\": 212,\n            \"y\": 286,\n            \"width\": 150,\n            \"height\": 200\n          },\n          \"opened\": false,\n          \"editorType\": {\n            \"serializedType\": \"GraphProcessor.ProcessorView, com.alelievr.NodeGraphProcessor.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\"\n          }\n        }\n      ],\n      \"exposedParameters\": [\n        {\n          \"guid\": \"2ff67e12-627b-447e-9439-b567c8bb1691\",\n          \"name\": \"LabelContainer\",\n          \"type\": \"UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\",\n          \"serializedValue\": {\n            \"serializedType\": \"UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\",\n            \"serializedName\": \"\",\n            \"serializedValue\": \"{\\\"value\\\":{\\\"instanceID\\\":31380}}\"\n          },\n          \"input\": true,\n          \"settings\": {\n            \"isHidden\": false\n          }\n        }\n      ],\n      \"stickyNotes\": [],\n      \"position\": {\n        \"x\": 555,\n        \"y\": 300,\n        \"z\": 0\n      },\n      \"scale\": {\n        \"x\": 0.6575162410736084,\n        \"y\": 0.6575162410736084,\n        \"z\": 1\n      },\n      \"references\": {\n        \"version\": 1,\n        \"00000000\": {\n          \"type\": {\n            \"class\": \"Terminus\",\n            \"ns\": \"UnityEngine.DMAT\",\n            \"asm\": \"FAKE_ASM\"\n          },\n          \"data\": {}\n        }\n      },\n      \"Id\": \"testID\",\n      \"Name\": \"testGraphName\"\n  },\n  \"MessageType\": \"CreateVSGraph\",\n  \"ProjectId\": \"" + ConfigurationSingleton.SingleInstance.CurrentProject.Id +"\"\n}");
        

        //dynamic json = JsonConvert.DeserializeObject(CreateGraphMessage);

        public static void OpenWindow()
        {
            window = (GraphMessageTesting)GetWindow(typeof(GraphMessageTesting));
            window.minSize = new Vector2(200, 200);
            window.Show();
        }

        private void OnGUI()
        {
            DrawSettings();
        }

        private void DrawSettings()
        {
            // TODO: Add extra else if case to check if the name already exists in the project
                if (GUILayout.Button("Send testing CreateVSGraph message", GUILayout.Height(30)))
                {
                    // Operations.CreateVSGraph(CreateGraphMessage);

                    //window.Close();
                }
        }
    }
}