using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityFlow.Plugin.Scripts
{
	public static class Config
	{
		public static List<FlowTObject> objs = null;

		public static FlowUser flowUser = null;
		public static string projectId = null;
		public static string objectId = null;
		public static List<FlowProject> projectList = null;
		public static Dictionary<string, string> objectIds = null;

		public static void ResetValues()
		{

			flowUser = null;
			projectId = null;
			objectId = null;
			projectList = null;

			if (objs != null)
				objs.Clear();
		}
	}
}
