﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealityFlow.Plugin.Contrib;
using System.Linq;

[System.Serializable, NodeMenuItem("String")]
public class StringNode : BaseNode
{
	[Output(name = "Out"), SerializeField]
	public string				output;

	public override string		name => "String";
}
