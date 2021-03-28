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

	[Input(name = "Vector 4"), ShowAsDrawer]
	public Vector4 vector4;

	[Input(name = "Vector 3"), ShowAsDrawer]
	public Vector3 vector3;

	[Input(name = "Vector 2"), ShowAsDrawer]
	public Vector2 vector2;

	[Input(name = "Float"), ShowAsDrawer]
	public float floatInput;

	[Input(name = "Int"), ShowAsDrawer]
	public int intInput;

	[Input(name = "Empty")]
	public int intInput2;

	[Input(name = "String"), ShowAsDrawer]
	public string stringInput;

	[Input(name = "Color"), ShowAsDrawer]
	public Color color;

	[Input(name = "Game Object"), ShowAsDrawer]
	public GameObject gameObject;

	[Input(name = "Animation Curve"), ShowAsDrawer]
	public AnimationCurve animationCurve;

	[Input(name = "Rigidbody"), ShowAsDrawer]
	public Rigidbody rigidbody;

	[Input("Layer Mask"), ShowAsDrawer]
	public LayerMask layerMask;

	public override string name => "Game Object Manipulation";

	protected override void Process() {
            gameObject.GetComponent<Renderer>().material.color = color;
            Debug.Log("Color set!");
    }
}