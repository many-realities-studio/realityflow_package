using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using RealityFlow.Plugin.Scripts;

public class WhiteboardManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static Dictionary<string, FlowVSGraph> whiteboards = new Dictionary<string, FlowVSGraph>();

    void Start()
    {
        
    }

    // TODO: Receive message from the server
    // TODO: adding a new graph to the dictionary?

    public static void AddNewGraphToDict(FlowVSGraph graph){
        // GameObject WhiteBoard = Instantiate(Canvas, Canvas.transform.position, Canvas.transform.rotation);
        // add the graph to the list of managed whiteboards
        Debug.Log("adding graph " + graph.Id + "To whiteboardmanager");
        whiteboards.Add(graph.Id, graph);
        foreach (KeyValuePair<string, FlowVSGraph> board in whiteboards ){
            Debug.Log("graph info:" + JsonUtility.ToJson(board.Value));
        }
        // do some commands that make it show up in the scene 
    }

    public static void RemoveGraph(string idToRemove){
        whiteboards.Remove(idToRemove);
        // do some commands to delete everything from the scene properly
        
    }
    
}
