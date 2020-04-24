using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages
{
    /// <summary>
    /// Leave project received message format 
    /// </summary>
    public class LeaveProject_Received : ConfirmationMessage_Received
    {
        public LeaveProject_Received(bool wasSuccessful) : base(wasSuccessful)
        {
            this.MessageType = "LeaveProject";
        }
    }
}
