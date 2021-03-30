﻿using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Linq;
using RealityFlow.Plugin.Contrib;
using RealityFlow.Plugin.Scripts;
using NodeGraphProcessor.Examples;

// public struct Edge {
// 	NodePort input;
// 	NodePort output;
// };

public class RealityFlowGraphView : MonoBehaviour {
	public BaseGraph graph;
	public FlowVSGraph vsGraph;

	private JsonElement savePoint; 
	// public bool inputGraph;
	public ProcessGraphProcessor processor;

	public EdgeListener connectorListener;

	public static CommandPalette commandPalette;

	public GameObject Labeled;
	public GameObject contentPanel;

	public GameObject parameterContent;
	public GameObject parameterCreationCanvas;
	public GameObject SelectComparisonCanvas;

	public GameObject nodePortView;
	public GameObject nodeView;
	public GameObject paramView;
	public GameObject edgeView;

	public List<NodeView> nodeViewList = new List<NodeView> ();
	public Dictionary<string,NodeView> nodeViewDict = new Dictionary<string,NodeView>();
	public List<NodeView> selectedNV = new List<NodeView>();
	public List<BaseNode> selected = new List<BaseNode>();
	public List<EdgeView> edgeViews = new List<EdgeView>();

	public Dictionary<string,ExposedParameter> paramDict = new Dictionary<string, ExposedParameter>();
	public List<ExposedParameter> paramList = new List<ExposedParameter>();

	Vector2 newNodePosition = new Vector2();
	Vector2 nullChecker = new Vector2(-1,-1);
	public Vector2 canvasDimensions = new Vector2(2560, 1080); // FOR NOW, dont have these hardcoded in final demo

	// protected virtual EdgeListener CreateEdgeConnectorListener()
	// 	 => new EdgeListener(this);

    //public RealityFlowGraphView instance;


	private void Start () {
		SelectComparisonCanvas.SetActive(false);
		parameterCreationCanvas.SetActive(false);
		//InitializeGraph();
	}

	public void InitializeGraph(FlowVSGraph VSGraph){

		// connectorListener = new EdgeListener(this);
		// Debug.Log(connectorListener);
		//instance = this;
		// if (inputGraph) {
		// 	// graph1.AddExposedParameter ("LabelContainer", typeof (GameObject), Labeled);
		// 	// processor = new ProcessGraphProcessor (graph1);
		// 	// graph1.SetParameterValue ("LabelContainer", Labeled);
		// }
		// TODO: have it create a new empty graph and use that as the graph
		// graph = new BaseGraph();
		// Debug.Log(JsonUtility.ToJson(graph));
		// graph.name = "TEST GRAPH "+graph.GetInstanceID();
		vsGraph = VSGraph;
		graph = (BaseGraph)VSGraph;
		graph.name = (VSGraph.Name + " - " + VSGraph.Id);
		commandPalette = GameObject.Find("CommandPalette").GetComponent<CommandPalette>();
		// commandPalette = new CommandPalette();
        graph.onGraphChanges += GraphChangesCallback;
		savePoint = JsonSerializer.Serialize(graph);
		
		// selected =
		// NodeView.instance.LoadGraph(graph);
		LoadGraph(graph);
	}

	protected void LoadGraph(BaseGraph graph){
		newNodePosition = new Vector2(-1,-1);
		foreach (BaseNode node in graph.nodes ){
			StartCoroutine (AddNodeCoroutine(node));
		}
		foreach (SerializableEdge edge in graph.edges){
			StartCoroutine( AddEdgeCoroutine(edge));
		}
	}

    void GraphChangesCallback(GraphChanges changes)
    {
		// TODO have this event redraw the graph UI
		// Debug.Log(JsonSerializer.Serialize(graph));
        if(changes.addedNode != null)
        {
			vsGraph.IsUpdated = true;
			Debug.Log("Added node");            // Debug.Log("Added a node "+changes.addedNode);
            //Undo.RegisterCompleteObjectUndo(graph,"Added Node RF");
			// commandPalette.AddCommandToStack(new AddNodeCommand("Add Node", changes,graph));
            Debug.Log("Serialized changes:" +JsonUtility.ToJson(graph));
			Debug.Log("Serialized VSGraph changes:" + JsonUtility.ToJson(vsGraph));
			Debug.Log(vsGraph.IsUpdated);
        }
        if(changes.removedNode != null)
        {
			vsGraph.IsUpdated = true;
			Debug.Log("Removed node"); 
            // Debug.Log("Removed node "+JsonSerializer.Serialize(changes.removedNode));
			// commandPalette.AddCommandToStack(new DeleteNodeCommand("Remove Node", changes.removedNode));
        }
		if (changes.nodeChanged != null)
		{
			vsGraph.IsUpdated = true;
			Debug.Log("Node changed"); 
			// Debug.Log(JsonSerializer.Serialize(changes.nodeChanged));
		}
		if(changes.addedEdge != null)
		{
			vsGraph.IsUpdated = true;
			Debug.Log("Added edge"); 
		}
		if(changes.removedEdge != null)
		{
			vsGraph.IsUpdated = true;
			Debug.Log("Removed edge"); 
		}
		// Debug.Log(JsonSerializer.Serialize(changes));
        // Debug.Log("Serialized changes:" + JsonSerializer.Serialize(changes));

		// TODO: Add command to palette here instead
    }

	public void UndoLastCommand(){
		// get the command itself
		Command cmd;
		// cmd = commandPalette.GetCommandStack()[0];
		// cmd = commandPalette.GetCommandStack()[commandPalette.GetCommandStack().Count-1];
		cmd = commandPalette.GetCommandStack()[0];
		// Debug.Log(cmd.PrintCommand());
		JsonUtility.FromJsonOverwrite(cmd.GetGraphState(), graph);
		graph.Deserialize();
	}

	public void DeleteSelection(){
		// serialize the current version of the graph
		string tmp;
		tmp = JsonUtility.ToJson(graph);
		// tmp = JsonSerializer.Serialize(graph);
		// tmp.jsonDatas = JsonUtility.ToJson(graph);

		// BaseGraph preCmd = graph;
		// JsonUtility.FromJsonOverwrite(tmp, preCmd);

		// send this to the command palette
		commandPalette.AddCommandToStack(new DeleteNodeCommand("Delete Selection of Nodes", tmp));

		// 3. Perform the command's action
		Debug.Log(selectedNV.Count);
		// foreach( BaseNode n in selected){
		// 	graph.RemoveNode(n);
		// }
		// selected.Clear();
		// TODO: Change deletion process so we use the NodeView.guid to delete specific dictionary indicies instead of using a list (Requires we change our NodeView List into a dictionary).
		foreach( NodeView n in selectedNV){
			// TODO: Deletion for Dictionary
			nodeViewList.Remove(n);
			n.Delete();
		}
		foreach(KeyValuePair<string,NodeView> nv in nodeViewDict){
			nodeViewDict.Remove(nv.Key);
			nv.Value.Delete();
		}
		selectedNV.Clear();
		// nodeViewList.RemoveAll(nv => nv==null);
		
		// BaseGraph postCmd = graph;
		// JsonUtility.FromJsonOverwrite(tmp, postCmd);
		// iterate over list
			// remove all nodes from the graph.
			// remove all their edges
			// remove any selected exposed params
	}

	public void SetNewNodeLocation(Vector2 pos2D){
		newNodePosition = pos2D;
		Debug.Log("New Node position in RFGV: "+newNodePosition);
	}
 
	public void AddNodeCommand(string nodeTag){
		// serialize the current version of the graph
		// savePoint = JsonSerializer.Serialize(graph);
		string tmp = JsonUtility.ToJson(graph);

		// send this to the command palette
		commandPalette.AddCommandToStack(new AddNodeCommand("Add Node", tmp));

		// perform the actual command action
        //TextNode tn = BaseNode.CreateFromType<TextNode> (new Vector2 ());
		BaseNode node;
		switch(nodeTag)
		{
			case "TextNode":
				TextNode tn = BaseNode.CreateFromType<TextNode> (new Vector2 ());
				graph.AddNode (tn);
				tn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				tn.output = "Hello World";
				StartCoroutine (AddNodeCoroutine(tn));
				break;
			case "PrintNode":
				PrintNode pn = BaseNode.CreateFromType<PrintNode> (new Vector2 ());
				graph.AddNode (pn);
				pn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				StartCoroutine (AddNodeCoroutine(pn));
				break;
			case "FloatNode":
				FloatNode fn = BaseNode.CreateFromType<FloatNode> (new Vector2 ());
				graph.AddNode (fn);
				fn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				fn.output = 0.5f;
				StartCoroutine (AddNodeCoroutine(fn));
				break;
			case "IntNode":
				IntNode intn = BaseNode.CreateFromType<IntNode> (new Vector2 ());
				graph.AddNode (intn);
				intn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				intn.output = 1;
				StartCoroutine (AddNodeCoroutine(intn));
				break;
			case "BoolNode":
				SelectComparisonCanvas.SetActive(true);
				//while(SelectComparisonCanvas.GetComponent<SelectComparison>().ready == false)
				//{ CAUSES INFINITE LOOP, REWORK
					// wait until choice made
				//}
				// BoolNode bn = BaseNode.CreateFromType<BoolNode> (new Vector2 ());
				// bn.compareFunction = comparisonFunction;
				// bn.inA = 0f;
				// bn.inB = 0f;
				// graph.AddNode(bn);
				// StartCoroutine (AddNodeCoroutine(bn));
				break;
			case "ConditionalNode":
				IfNode cn = BaseNode.CreateFromType<IfNode> (new Vector2 ());
				graph.AddNode(cn);
				cn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				StartCoroutine (AddNodeCoroutine(cn));
				break;
			case "StartNode":
				StartNode sn = BaseNode.CreateFromType<StartNode>(new Vector2());
				graph.AddNode(sn);
				sn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				StartCoroutine(AddNodeCoroutine(sn));
				break;
			case "GameObjectManipulationNode":
				GameObjectManipulationNode gn = BaseNode.CreateFromType<GameObjectManipulationNode>(new Vector2());
				graph.AddNode(gn);
				gn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
				StartCoroutine(AddNodeCoroutine(gn));
				break;
			// case "ParameterNode":
			// 	ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());
			// 	graph.AddNode(pn);
			// 	StartCoroutine(AddNodeCoroutine(pn));
			default:
				Debug.Log("This case of addnode did not use a tag");
				break; 
		}		
	}

	public void AddParameterNodeToGraph(string epnGUID){
		ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());
		// pn.parameter = epn;
		pn.parameterGUID = epnGUID;

		graph.AddNode(pn);
		pn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
		StartCoroutine(AddNodeCoroutine(pn));
	}

	public void BoolNodeStep2(string comparisonFunction)
	{
		BoolNode bn = BaseNode.CreateFromType<BoolNode> (new Vector2 ());
		bn.compareFunction = comparisonFunction;
		bn.inA = 0f;
		bn.inB = 0f;
		graph.AddNode(bn);
		bn.position = new Rect(new Vector2(newNodePosition.x,newNodePosition.y),new Vector2(100,100));
		StartCoroutine (AddNodeCoroutine(bn));
	}

	public void PrintCommandStack(){
		// Debug.Log("Print stack disabled until we get UI elements in vr");
		commandPalette.PrintStack();
	}

	public void AddToSelection(BaseNode n){
		selected.Add(n);
	}

	public void AddToSelectionNV(NodeView n){
		selectedNV.Add(n);
	}

	public void AddParameter(){
		parameterCreationCanvas.SetActive(true);
		// //while(parameterCreationCanvas.GetComponent<ParameterCreation>().ready == false)
		// //{ CAUSES INFINITE LOOP, REWORK
		// 	// wait until choice made
		// //}
		// string tmp = JsonUtility.ToJson(graph);
		// // get name of parameter from user input via mtrk keyboard (probably)
		// string name = "autofill";
		// Type type;
		// switch(parameterType)
		// {
		// 	default:
		// 	case "GameObject": type = typeof(GameObject); break;
		// 	case "String": type = typeof(string); break;
		// 	case "Float": type = typeof(float); break;
		// 	case "Int": type = typeof(int); break;
		// 	case "Bool": type = typeof(bool); break;
		// }
		// //Debug.Log(type.AssemblyQualifiedName);
		// // make sure it's not a duplicate name
		// // add parameter to a list that is drag and droppable
		// ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());
		// graph.AddExposedParameter (name, type, Labeled);
		// ExposedParameter epn = graph.GetExposedParameter (name);
		// pn.parameterGUID = epn.guid;
		// //paramDict.Add(epn.guid,epn);
		// paramList.Add(epn);
		// // instantiate paramView
		// ParameterView newParamView = Instantiate(paramView,new Vector3(),Quaternion.identity).GetComponent<ParameterView> ();
		// newParamView.gameObject.transform.SetParent (parameterContent.transform, false);
		// newParamView.title.text = epn.name;
		// newParamView.type.text = epn.type;
        // newParamView.guid.text = epn.guid.Substring (epn.guid.Length - 5);
		// newParamView.rfgv = this;
		// newParamView.pn = epn;
		// // add paramView to content panel
	}
	public void AddParameterStep2(string parameterType, string parameterName)
	{
		//while(parameterCreationCanvas.GetComponent<ParameterCreation>().ready == false)
		//{ CAUSES INFINITE LOOP, REWORK
			// wait until choice made
		//}
		string tmp = JsonUtility.ToJson(graph);
		// get name of parameter from user input via mtrk keyboard (probably)
		Type type;
		switch(parameterType)
		{
			default:
			case "GameObject": type = typeof(GameObject); break;
			case "String": type = typeof(string); break;
			case "Float": type = typeof(float); break;
			case "Int": type = typeof(int); break;
			case "Bool": type = typeof(bool); break;
			case "Color":type = typeof(Color); break;
		}
		//Debug.Log(type.AssemblyQualifiedName);
		// make sure it's not a duplicate name
		// add parameter to a list that is drag and droppable
		// ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());
		graph.AddExposedParameter (parameterName, type, Labeled);
		ExposedParameter epn = graph.GetExposedParameter (parameterName);
		// pn.parameterGUID = epn.guid;
		//paramDict.Add(epn.guid,epn);
		paramList.Add(epn);
		// instantiate paramView
		ParameterView newParamView = Instantiate(paramView,new Vector3(),Quaternion.identity).GetComponent<ParameterView> ();
		newParamView.gameObject.transform.SetParent (parameterContent.transform, false);
		newParamView.title.text = epn.name;
		newParamView.type.text = epn.type;
        newParamView.guid.text = epn.guid.Substring (epn.guid.Length - 5);
		newParamView.rfgv = this;
		newParamView.pn = epn;
		// newParamView.NodeInstance = pn;
		// add paramView to content panel
	}

	public void RemoveParameter(ExposedParameter pn)
	{
		graph.RemoveExposedParameter(pn);
		paramList.Remove(pn);
	}

	public void CreateGraph () {
		// graph.SetDirty();
		graph.AddExposedParameter ("LabelContainer", typeof (GameObject), Labeled);
		TextNode tn = BaseNode.CreateFromType<TextNode> (new Vector2 ());
		graph.AddNode (tn);
		tn.output = "Hello World";
		StartCoroutine (AddNodeCoroutine(tn));
		SetLabelNode sln = BaseNode.CreateFromType<SetLabelNode> (new Vector2 ());
		graph.AddNode (sln);
		graph.Connect (sln.GetPort ("newLabel", null), tn.GetPort ("output", null));
		StartCoroutine (AddNodeCoroutine(sln));
		ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());

		pn.parameterGUID = graph.GetExposedParameter ("LabelContainer").guid;
		graph.AddNode (pn);
		graph.Connect (sln.GetPort ("input", ""), pn.GetPort ("output", "output"));
		StartCoroutine (AddNodeCoroutine(pn));
		graph.UpdateComputeOrder ();
		// graph.ed
		// graph.SetParameterValue ("LabelContainer", Labeled);
		processor = new ProcessGraphProcessor (graph);
    }
	
	// public void ConnectEdges(NodePort input, NodePort output){
	public void ConnectEdges(NodePortView input, NodePortView output){ // Replacing arguments w/ NodePortViews
		// graph.Connect(input, output, true);
		// SerializableEdge newEdge = graph.Connect(input.port, output.port, true);
		SerializableEdge edge = graph.Connect(input.port, output.port, true);
		StartCoroutine(AddEdgeCoroutine(edge));
		/* Backend:
		 - Create an Edge
		 - Store this edge into the list

		 Frontend:
		 - Create an EdgeView with the edge
		  */
	}
	
	public void ClearGraph () {

		while (graph.nodes.Count > 0) {
			graph.RemoveNode (graph.nodes[0]);
		}
		while (graph.exposedParameters.Count > 0) {
			graph.RemoveExposedParameter (graph.exposedParameters[0]);
		}
			//graph.SetDirty();
        // EditorWindow.GetWindow<CustomToolbarGraphWindow> ().InitializeGraph (graph as BaseGraph)

		// TODO: graphs don't have GUIDs....
		// commandPalette.AddCommandToStack(new Command("Clear Graph", "NO GUID"));
    }

    public List <BaseNode> GetNodes()
    {
        Debug.Log("There are "+graph.nodes.Count+" inside runtimegraph");
        return graph.nodes;
    }
	public void DoProcessing () {
		processor.Run ();
	}

	public float padding = 0.1f;
	// This couroutine is in charge of asynchronously making the EdgeView Prefab
	// public IEnumerator AddEdgeCoroutine (NodePortView input, NodePortView output){
	public IEnumerator AddEdgeCoroutine (SerializableEdge edge){
		
		EdgeView newEdge = Instantiate( edgeView, new Vector3(), Quaternion.identity).GetComponent<EdgeView>();
		newEdge.rfgv = this;
		yield return new WaitUntil(() => nodeViewDict.Count == graph.nodes.Count);
		NodeView inputView = nodeViewDict[edge.inputNodeGUID];
		NodeView outputView = nodeViewDict[edge.outputNodeGUID];

		foreach (NodePortView npv in inputView.inputPortViews){
			if (npv.port == edge.inputPort){ newEdge.input = npv; break; }
		}
		foreach (NodePortView npv in outputView.outputPortViews){
			if (npv.port == edge.outputPort){ newEdge.output = npv; break; }
		}
		newEdge.gameObject.transform.SetParent (contentPanel.transform, false);
		// Set the positions of the linerenderer
		newEdge.edge = edge;
		LineRenderer lr = newEdge.GetComponent<LineRenderer>();
		// calculate the extra points for a better looking circuit board line
		Vector3 [] edgePoints = new [] {
			newEdge.output.GetComponent<RectTransform>().transform.position,
			newEdge.input.GetComponent<RectTransform>().transform.position
			};
		lr.SetPositions(edgePoints);
		newEdge.input.edges.Add(newEdge);
		newEdge.output.edges.Add(newEdge);
		yield return new WaitForSeconds (.01f);
	}

	public IEnumerator AddNodeCoroutine (BaseNode node) {
        NodeView newView = Instantiate (nodeView, new Vector3(), Quaternion.identity).GetComponent<NodeView> ();
        newView.gameObject.transform.SetParent (contentPanel.transform, false);
		// Set the rectTransform position here after we've set the parent
        newView.title.text = node.name;
        newView.node = node;
        newView.GUID.text = node.GUID.Substring (node.GUID.Length - 5);
		newView.rfgv = this;
        // contentPanel.GetComponent<ContentSizeFitter>().enabled = false;
        foreach (NodePort input in node.inputPorts) {
            // newView.GetComponent<ContentSizeFitter>().enabled = false/;
            NodePortView npv = Instantiate (nodePortView).GetComponent<NodePortView> ();
            npv.gameObject.transform.SetParent (newView.inputPanel.transform, false);
            npv.gameObject.GetComponent<RectTransform> ().SetAsLastSibling ();
			npv.listener = connectorListener;
			npv.type = "input";
            yield return new WaitForSeconds (.01f);
            npv.Init (input);
			newView.inputPortViews.Add(npv);
            // LayoutRebuilder.MarkLayoutForRebuild ((RectTransform) newView.transform);
            // newView.GetComponent<ContentSizeFitter>().enabled = true;
        }
        foreach (NodePort output in node.outputPorts) {
			Debug.Log(output.portData);
            // newView.GetComponent<ContentSizeFitter>().enabled = false;
            NodePortView npv = Instantiate (nodePortView).GetComponent<NodePortView> ();
            npv.gameObject.transform.SetParent (newView.outputPanel.transform,false);
            npv.gameObject.GetComponent<RectTransform> ().SetAsLastSibling ();
			npv.listener = connectorListener;
			npv.type = "output";
            yield return new WaitForSeconds (.01f);
            npv.Init (output);
            // LayoutRebuilder.MarkLayoutForRebuild ((RectTransform) newView.transform);
            // newView.GetComponent<ContentSizeFitter>().enabled = true;
			newView.outputPortViews.Add(npv);
        }
		nodeViewDict.Add (node.GUID,newView);
        nodeViewList.Add (newView);
        // LayoutRebuilder.MarkLayoutForRebuild ((RectTransform) newView.transform);
		// RectTransform rect = newView.gameObject.transform.GetChild(0).GetComponent<RectTransform> ();
		RectTransform rect = newView.gameObject.GetComponent<RectTransform> ();
        rect.SetAsLastSibling ();
		if (newNodePosition == nullChecker){ // if the newNodePosition is not set, then we can load the position from server
			rect.anchoredPosition = new Vector2(canvasDimensions.x*node.position.x, canvasDimensions.y*node.position.y);
			// rect.anchoredPosition = new Vector2(node.position.x, node.position.y);
		} else {
			rect.anchoredPosition = canvasDimensions*newNodePosition;
		}
		// rect.anchoredPosition = new Vector2(canvasDimensions*newNodePosition);
        // contentPanel.GetComponent<VerticalLayoutGroup>().enabled = false;
        yield return new WaitForSeconds (.01f);
        // contentPanel.GetComponent<VerticalLayoutGroup>().enabled = true;
        // contentPanel.GetComponent<ContentSizeFitter>().enabled = true;
        // LayoutRebuilder.MarkLayoutForRebuild ((RectTransform) contentPanel.transform);
    }

}
