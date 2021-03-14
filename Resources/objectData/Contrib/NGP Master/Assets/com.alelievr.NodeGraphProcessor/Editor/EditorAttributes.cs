using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace RealityFlow.Plugin.Contrib
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeCustomEditor : Attribute
	{
		public Type nodeType;

		public NodeCustomEditor(Type nodeType)
		{
			this.nodeType = nodeType;
		}
	}
}