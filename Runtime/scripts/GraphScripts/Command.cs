/* Goals

    - Want 3 types of undos
        1. last change on specific object (TODO)
        2. last change by specific user (TODO)
        3. Last change done ever (DONE)
        4. Undo any change in stack by selecting it (TODO)


Types of Commands:

    - Add Node
    - Move Nodes
    - Delete Nodes
    - Connect Edges / Connect Nodes

    - Run Graph
    - Clear Graph

    - Undo
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GraphProcessor;

public abstract class Command : MonoBehaviour
{   
    public string action;
    string graphState; // This just stores the state of the graph BEFORE the command was performed.

    public Command(string desc, string sp){
        this.action = desc;
        // this.graph = graph;
        this.graphState = sp;
    }

    public string GetGraphState(){
        return graphState;
    }

    public virtual string PrintCommand(){
        return String.Format("{0}", this.action);
    }
}

public class AddNodeCommand : Command {
    public AddNodeCommand(string desc, string sp) : base (desc, sp){}

}

public class AddEdgeCommand : Command {
    public AddEdgeCommand(string desc, string sp) : base (desc, sp){}

}

public class AddExposedParameterCommand : Command {
    public AddExposedParameterCommand(string desc, string sp) : base(desc,sp){}
}

public class DeleteExposedParameterCommand : Command {
    public DeleteExposedParameterCommand(string desc, string sp) : base (desc,sp){}
}

public class ModifyExposedParameterCommand : Command {
    public ModifyExposedParameterCommand(string desc, string sp) : base(desc,sp){}
}


public class DeleteNodeCommand : Command {
    public DeleteNodeCommand(string desc, string sp) : base (desc, sp){}
}

public class CreateGraphCommand : Command {
    public CreateGraphCommand(string desc, string sp) : base(desc, sp){}
}

public class ClearGraphCommand : Command {
    public ClearGraphCommand(string desc, string sp) : base(desc, sp){}
}

