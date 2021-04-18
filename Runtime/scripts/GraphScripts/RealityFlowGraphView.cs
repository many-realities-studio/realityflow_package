using System;
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
using Packages.realityflow_package.Runtime.scripts;
public class RealityFlowGraphView : MonoBehaviour {
	public BaseGraph graph;
	public FlowVSGraph vsGraph;

	private JsonElement savePoint; 
	public ProcessGraphProcessor processor;

	public EdgeListener connectorListener;

	public CommandPalette commandPalette;

	public GameObject Labeled;
	public GameObject contentPanel;

	public GameObject parameterContent;
	
	public GameObject parameterCreationCanvas;
	public GameObject SelectComparisonCanvas;
	public GameObject VSGraphDropdownCanvas;

	public GameObject nodePortView;
	public GameObject nodeView;
	public GameObject paramView;
	public GameObject edgeView;

	public Dictionary<string,NodeView> nodeViewDict = new Dictionary<string,NodeView>();
	public static Dictionary<string,RealityFlowGraphView> nodeViewtoRFGVDict = new Dictionary<string,RealityFlowGraphView>();
	public Dictionary<string,NodeView> selectedNVDict = new Dictionary<string,NodeView>();
	public List<BaseNode> selected = new List<BaseNode>();
	public Dictionary<string,ParameterView> paramDict = new Dictionary<string, ParameterView>();

	Vector2 newNodePosition = new Vector2();
	Vector2 nullChecker = new Vector2(-1,-1);
	public Vector2 canvasDimensions = new Vector2(2560, 1080); // FOR NOW, dont have these hardcoded in final demo

	public float updateTimer;
	public float maxUpdateTime;
	public bool reloadCoroutineStarted;

	private bool nodeRoutineRunning = false;
	private bool edgeRoutineRunning = false;
	private bool paramRoutineRunning = false;


	private void Start () {
		updateTimer = 0f;
		maxUpdateTime = 0.05f;
		reloadCoroutineStarted = false;
		SelectComparisonCanvas.SetActive(false);
		parameterCreationCanvas.SetActive(false);
		VSGraphDropdownCanvas.SetActive(false);
	}

	private void Update(){
		updateTimer += Time.deltaTime;
	}

	public void InitializeGraphStep1()
	{
		VSGraphDropdownCanvas.SetActive(true);
		VSGraphDropdownCanvas.GetComponent<VSGraphSelectionDropdown>().LoadGraphs();
	}
	public void InitializeGraph(FlowVSGraph VSGraph){
		vsGraph = VSGraph;
		graph = (BaseGraph)VSGraph;
		graph.name = (VSGraph.Name + " - " + VSGraph.Id);
		commandPalette = GameObject.Find("CommandPalette").GetComponent<CommandPalette>();
        graph.onGraphChanges += GraphChangesCallback;
		graph.onExposedParameterListChanged += ParamChangesListChangeCallBack;
		graph.onExposedParameterModified += ParamChangesModifiedCallBack;
		Operations.updateRFGV += ReloadRFGV;
		Operations.runVSGraph += ReceiveRunVSGraph;
		Operations.deleteVSGraph += ReceiveDeleteVSGraph;
		savePoint = JsonSerializer.Serialize(graph);

		HardLoadGraph(graph);
	}

	protected void SoftLoadGraph(BaseGraph graph){
		ClearWhiteBoard();
		newNodePosition = new Vector2(-1,-1);
		foreach(ExposedParameter p in graph.exposedParameters){
			StartCoroutine(AddExposedParameterCoroutine(p));
		}
		foreach (BaseNode node in graph.nodes ){
			StartCoroutine (AddNodeCoroutine(node));
		}
		Debug.LogError("Nodes Views: "+ nodeViewDict.Count);
		Debug.LogError("Nodes: "+ graph.nodes.Count);
		foreach (SerializableEdge edge in graph.edges){
			StartCoroutine( AddEdgeCoroutine(edge));
		}

		foreach(KeyValuePair<string,NodeView> nv in nodeViewDict){
			if (nv.Value.CanBeModified){
				nv.Value.CheckIn();
			}
		}
	}

	protected void HardLoadGraph(BaseGraph graph){
		ClearGraph();
		newNodePosition = new Vector2(-1,-1);
		foreach(ExposedParameter p in graph.exposedParameters){
			StartCoroutine(AddExposedParameterCoroutine(p));
		}
		foreach (BaseNode node in graph.nodes ){
			StartCoroutine (AddNodeCoroutine(node));
		}
		foreach (SerializableEdge edge in graph.edges){
			StartCoroutine( AddEdgeCoroutine(edge));
		}
	}
	
	void ReloadRFGV()
	{
		updateTimer = 0f;
		if (!reloadCoroutineStarted)
		{
			StartCoroutine (CallSoftReloadCoroutine());
			reloadCoroutineStarted = true;
		}
	}

	void ReceiveRunVSGraph(string receivedVSGraphId)
	{
		if (vsGraph.Id == receivedVSGraphId)
		{
			processor = new ProcessGraphProcessor (graph);
			processor.Run ();
		}
	}

	void ReceiveDeleteVSGraph(string receivedVSGraphId)
	{
		if (vsGraph.Id == receivedVSGraphId)
		{
			ClearGraph();
			vsGraph = null;
			graph = null;
		}
	}

	// when we receive a graph update, we don't immediately want to trigger a reload. We want to first check if we get any additional updates in a time
	// frame, and if we do not, then call the reload
	public IEnumerator CallSoftReloadCoroutine() {
		yield return new WaitUntil(() => updateTimer > maxUpdateTime && !nodeRoutineRunning && !edgeRoutineRunning && !paramRoutineRunning);
		SoftLoadGraph(graph);
		updateTimer = 0f;
		reloadCoroutineStarted = false;
	}
	
	// unsubscribe from all events when play mode is exited
	void OnApplicationQuit() 
	{
		Operations.updateRFGV -= ReloadRFGV;
		Operations.runVSGraph -= ReceiveRunVSGraph;
		graph.onExposedParameterListChanged -= ParamChangesListChangeCallBack;
		graph.onExposedParameterModified -= ParamChangesModifiedCallBack;
		graph.onGraphChanges -= GraphChangesCallback;
		Operations.deleteVSGraph -= ReceiveDeleteVSGraph;
	}

	// triggers when the parameter list is updated
	void ParamChangesListChangeCallBack()
	{
		vsGraph.IsUpdated = true;
	}

	// triggers when a parameter value is modified
	void ParamChangesModifiedCallBack(string s)
	{
		vsGraph.IsUpdated = true;
	}

	// these trigger when specific events happen on the basegraph
	// they then set the update flad to true so they can happen on the whiteboard
    void GraphChangesCallback(GraphChanges changes)
    {
        if(changes.addedNode != null)
        {
			vsGraph.IsUpdated = true;
        }
        if(changes.removedNode != null)
        {
			vsGraph.IsUpdated = true;
        }
		if (changes.nodeChanged != null)
		{
			vsGraph.IsUpdated = true;
		}
		if(changes.addedEdge != null)
		{
			vsGraph.IsUpdated = true;
		}
		if(changes.removedEdge != null)
		{
			vsGraph.IsUpdated = true;
		}
    }

	// TODO: implement user-based undo and set up undo to be able to undo more than the last thing
	public void UndoLastCommand(){
		// get the command itself
		Command cmd;
		cmd = commandPalette.GetCommandStack()[0];
		JsonUtility.FromJsonOverwrite(cmd.GetGraphState(), graph);
		graph.Deserialize();
		vsGraph.IsUpdated = true;
		HardLoadGraph(graph);
	}

	public void DeleteSelection(){
		// serialize the current version of the graph
		string tmp;
		tmp = JsonUtility.ToJson(graph);

		commandPalette.AddCommandToStack(new DeleteNodeCommand("Delete Selection of Nodes", tmp));

		foreach(KeyValuePair<string,NodeView> nv in selectedNVDict){
			nodeViewDict.Remove(nv.Key);
			graph.RemoveNode(nv.Value.node);
			nv.Value.Delete();
		}
		selectedNVDict.Clear();
	}

	// Overload of single node deletion
	public void DeleteSelection(NodeView nv){
		// serialize the current version of the graph
		string tmp;
		tmp = JsonUtility.ToJson(graph);

		commandPalette.AddCommandToStack(new DeleteNodeCommand("Delete Selection of Nodes", tmp));

		nodeViewDict.Remove(nv.node.GUID);
		graph.RemoveNode(nv.node);

	}

	public void SetNewNodeLocation(Vector2 pos2D){
		newNodePosition = pos2D;
	}
 
	public void AddNodeCommand(string nodeTag){
		// serialize the current version of the graph
		string tmp = JsonUtility.ToJson(graph);

		// send this to the command palette
		commandPalette.AddCommandToStack(new AddNodeCommand("Add Node", tmp));

		// perform the actual command action
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
			default:
				Debug.Log("This case of addnode did not use a tag");
				break; 
		}		
	}

	public void AddParameterNodeToGraph(string epnGUID){
		string tmp = JsonUtility.ToJson(graph);

		// send this to the command palette
		commandPalette.AddCommandToStack(new AddNodeCommand("Add Parameter to Graph", tmp));

		ParameterNode pn = BaseNode.CreateFromType<ParameterNode> (new Vector2 ());
		pn.parameterGUID = epnGUID;

		graph.AddNode(pn);
		pn.position = new Rect(new Vector2(),new Vector2(100,100));
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
		commandPalette.PrintStack();
	}

	public void AddToSelection(BaseNode n){
		selected.Add(n);
	}

	public void AddToSelectionNV(NodeView n){
            selectedNVDict.Add (n.node.GUID,n);
	}

	public void AddParameter(){
		parameterCreationCanvas.SetActive(true);
	}
	public void AddParameterStep2(string parameterType, string parameterName)
	{
		string tmp = JsonUtility.ToJson(graph);

		// send this to the command palette
		commandPalette.AddCommandToStack(new AddExposedParameterCommand("Add Exposed Parameter", tmp));

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
		graph.AddExposedParameter (parameterName, type, null);
		ExposedParameter epn = graph.GetExposedParameter (parameterName);
		StartCoroutine(AddExposedParameterCoroutine(epn));
	}

	public IEnumerator AddExposedParameterCoroutine (ExposedParameter epn){
		if(!paramRoutineRunning)
			paramRoutineRunning = true;
		ParameterView newParamView = Instantiate(paramView,new Vector3(),Quaternion.identity).GetComponent<ParameterView> ();
		newParamView.gameObject.transform.SetParent (parameterContent.transform, false);
		newParamView.title.text = epn.name;
		newParamView.type.text = epn.type;
        newParamView.guid.text = epn.guid.Substring (epn.guid.Length - 5);
		newParamView.rfgv = this;
		newParamView.pn = epn;
		paramDict.Add (epn.guid,newParamView);
		if(newParamView.pn.serializedValue.value == null && newParamView.pn.type != "UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
		{
			newParamView.ModifyParameterValue();
		}
		paramRoutineRunning = false;
		yield return new WaitForSeconds (.01f);
	}

	public void ModifyExposedParameterValue()
	{
		string tmp = JsonUtility.ToJson(graph);
        commandPalette.AddCommandToStack(new ModifyExposedParameterCommand("Modify Exposed Parameter", tmp));
	}

	public void RemoveParameter(ParameterView pv)
	{
		string tmp = JsonUtility.ToJson(graph);
        commandPalette.AddCommandToStack(new DeleteExposedParameterCommand("Delete Exposed Parameter", tmp));
		paramDict.Remove(pv.pn.guid);
		vsGraph.paramIdToObjId.Remove(pv.pn.guid);
		graph.RemoveExposedParameter(pv.pn);
	}

	// A function created by Dr. Murray to create a template graph
	public void CreateGraph () {
		string tmp = JsonUtility.ToJson(graph);

		// send this to the command palette
		commandPalette.AddCommandToStack(new CreateGraphCommand("Create Graph", tmp));

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
		processor = new ProcessGraphProcessor (graph);
    }
	
	public void ConnectEdges(NodePortView input, NodePortView output){
		string tmp = JsonUtility.ToJson(graph);
        commandPalette.AddCommandToStack(new AddEdgeCommand("Created Edge", tmp));
		SerializableEdge edge = graph.Connect(input.port, output.port, true);
		StartCoroutine(AddEdgeCoroutine(edge));
		/* Backend:
		 - Create an Edge
		 - Store this edge into the list

		 Frontend:
		 - Create an EdgeView with the edge
		  */
	}
	
	// deletes everything on the graph and whiteboard. Hard clear
	public void ClearGraph () {
		string tmp = JsonUtility.ToJson(graph);
        commandPalette.AddCommandToStack(new DeleteExposedParameterCommand("Clearing Graph", tmp));

		foreach(KeyValuePair<string,ParameterView> pv in paramDict){
			pv.Value.Delete();
		}
		paramDict.Clear();

		foreach(KeyValuePair<string,NodeView> nv in nodeViewDict){
			nv.Value.Delete();
		}
		nodeViewDict.Clear();
    }

	// clears everything on the whiteboard but does not delete anything from the graph
	public void ClearWhiteBoard()
	{
		foreach(KeyValuePair<string,ParameterView> pv in paramDict){
			pv.Value.DeleteFromWhiteBoard();
		}
		paramDict.Clear();

		foreach(KeyValuePair<string,NodeView> nv in nodeViewDict){
			nv.Value.DeleteFromWhiteBoard();
		}
		nodeViewDict.Clear();
	}

    public List <BaseNode> GetNodes()
    {
        Debug.Log("There are "+graph.nodes.Count+" inside runtimegraph");
        return graph.nodes;
    }

	// runs the graph
	public void DoProcessing () {
		Operations.RunVSGraph(vsGraph.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {/* Debug.Log(e.message);*/ });
		processor = new ProcessGraphProcessor (graph);
		processor.Run ();
	}

	public float padding = 0.1f;
	// This couroutine is in charge of asynchronously making the EdgeView Prefab
	public IEnumerator AddEdgeCoroutine (SerializableEdge edge){
		if(!edgeRoutineRunning)
		{
			yield return new WaitUntil(() => !nodeRoutineRunning && !paramRoutineRunning);
			edgeRoutineRunning = true;
		}
		if (nodeViewDict.Count != graph.nodes.Count){
			yield return new WaitUntil(() => nodeViewDict.Count == graph.nodes.Count);
		}
		else{
		 	yield return new WaitForSeconds (.01f);
		}
		EdgeView newEdge = Instantiate( edgeView, new Vector3(), Quaternion.identity).GetComponent<EdgeView>();
		newEdge.gameObject.transform.SetParent (contentPanel.transform, false);
		newEdge.rfgv = this;
		NodeView inputView = nodeViewDict[edge.inputNodeGUID];
		NodeView outputView = nodeViewDict[edge.outputNodeGUID];

		foreach (NodePortView npv in inputView.inputPortViews){
			if (npv.port == edge.inputPort){ newEdge.input = npv; break; }
		}
		foreach (NodePortView npv in outputView.outputPortViews){
			if (npv.port == edge.outputPort){ newEdge.output = npv; break; }
		}
		newEdge.input.edges.Add(newEdge);
		newEdge.output.edges.Add(newEdge);
		// Set the positions of the linerenderer
		newEdge.edge = edge;
		newEdge.Init();

		edgeRoutineRunning = false;
		yield return new WaitForSeconds (.01f);
	}

	public IEnumerator AddNodeCoroutine (BaseNode node) {
		if(!nodeRoutineRunning)
			nodeRoutineRunning = true;
        NodeView newView = Instantiate (nodeView, new Vector3(), Quaternion.identity).GetComponent<NodeView> ();
        newView.gameObject.transform.SetParent (contentPanel.transform, false);
		// Set the rectTransform position here after we've set the parent
        newView.title.text = node.name;
        newView.node = node;
		// check if the node is a parameter. If it is, newView.GUID.text = node.name;
        newView.GUID.text = node.GUID.Substring (node.GUID.Length - 5);
		newView.rfgv = this;
		newView.nodeGUID = node.GUID;
        foreach (NodePort input in node.inputPorts) {
            NodePortView npv = Instantiate (nodePortView).GetComponent<NodePortView> ();
            npv.gameObject.transform.SetParent (newView.inputPanel.transform, false);
            npv.gameObject.GetComponent<RectTransform> ().SetAsLastSibling ();
			npv.listener = connectorListener;
			npv.type = "input";
            yield return new WaitForSeconds (.01f);
            npv.Init (input);
			newView.inputPortViews.Add(npv);
        }
        foreach (NodePort output in node.outputPorts) {
            NodePortView npv = Instantiate (nodePortView).GetComponent<NodePortView> ();
            npv.gameObject.transform.SetParent (newView.outputPanel.transform,false);
            npv.gameObject.GetComponent<RectTransform> ().SetAsLastSibling ();
			npv.listener = connectorListener;
			npv.type = "output";
            yield return new WaitForSeconds (.01f);
            npv.Init (output);
			newView.outputPortViews.Add(npv);
        }
        nodeViewDict.Add (node.GUID,newView);

		if(nodeViewtoRFGVDict.ContainsKey(node.GUID))
		{
			nodeViewtoRFGVDict[node.GUID] = this;
		}
		else
		{
			nodeViewtoRFGVDict.Add(node.GUID,this);
		}

		Operations.CheckinNodeView(newView.nodeGUID, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => {});

		RectTransform rect = newView.gameObject.GetComponent<RectTransform> ();
        rect.SetAsLastSibling ();
		if (newNodePosition == nullChecker){ // if the newNodePosition is not set, then we can load the position from server
			rect.anchoredPosition = new Vector2(canvasDimensions.x*node.position.x, canvasDimensions.y*node.position.y);
		} else {
			rect.anchoredPosition = canvasDimensions*newNodePosition;
		}

		nodeRoutineRunning = false;
        yield return new WaitForSeconds (.01f);
    }
}