using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.realityflow_package.Runtime.scripts.Messages
{
    /// <summary>
    /// Received message format for pass/fail requests
    /// </summary>
    public abstract class ConfirmationMessage_Received : ReceivedMessage
    {
        [JsonProperty("WasSuccessful")]
        public bool WasSuccessful { get; set; }

        protected ConfirmationMessage_Received(bool wasSuccessful) : base(typeof(ConfirmationMessage_Received))
        {
            WasSuccessful = wasSuccessful;
        }
    }
}
