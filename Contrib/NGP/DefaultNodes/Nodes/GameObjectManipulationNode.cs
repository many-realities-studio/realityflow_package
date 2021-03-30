using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/GameObjectManipulation")]
public class GameObjectManipulationNode : BaseNode
{
    [Input(name = "Condition")]
    public ConditionalLink condition;

	[Input(name = "Color"), ShowAsDrawer]
	public Color color;

	[Input(name = "Game Object"), ShowAsDrawer]
	public GameObject gameObject;

	public override string name => "Game Object Manipulation";

	protected override void Process() {
            gameObject.GetComponent<Renderer>().material.color = color;
            Debug.Log("Color set!");
    }
}