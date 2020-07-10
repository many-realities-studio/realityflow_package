using Newtonsoft.Json;

namespace Packages.realityflow_package.Runtime.scripts.Messages.CheckoutMessages
{
    public class CheckoutObject_SendToServer : BaseMessage
    {
        [JsonProperty("ObjectId")]
        public string ObjectId;

        [JsonProperty("ProjectId")]
        public string ProjectId;

        public CheckoutObject_SendToServer(string objectId, string projectId)
        {
            ObjectId = objectId;
            ProjectId = projectId;
            this.MessageType = "CheckoutObject";
        }
    }

    public class CheckinObject_SendToServer : BaseMessage
    {
        [JsonProperty("ObjectId")]
        public string ObjectId;

        [JsonProperty("ProjectId")]
        public string ProjectId;

        public CheckinObject_SendToServer(string objectId, string projectId)
        {
            ObjectId = objectId;
            ProjectId = projectId;
            this.MessageType = "CheckinObject";
        }
    }
}