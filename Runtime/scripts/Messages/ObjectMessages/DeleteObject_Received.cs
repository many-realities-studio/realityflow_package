using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages
{
    public class DeleteObject_Received : ConfirmationMessage_Received
    {
        [JsonProperty("ObjectId")]
        public string DeletedObjectId { get; set; } // The deleted object

        public DeleteObject_Received(string deletedObjectId, bool wasSuccessful) : base(wasSuccessful)
        {
            DeletedObjectId = deletedObjectId;
            this.MessageType = "DeleteObject";
        }
    }
}
