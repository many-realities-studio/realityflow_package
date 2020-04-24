using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages
{
    /// <summary>
    /// The purpose of this class is to provide the information which all messages contain.
    /// (Both those sent to and received from the server)
    /// </summary>
    public class BaseMessage
    {
        //public delegate void ParseMessage(string message); // Definition of a parse message method

        [JsonProperty("MessageType")]
        public string MessageType { get; set; } // Type of message being sent

        //[JsonProperty("Message")]
        //public string Message { get; set; }
    }
}
