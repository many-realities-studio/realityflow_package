using Newtonsoft.Json;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    public class DeleteAvatar_Received : ConfirmationMessage_Received
    {
        [JsonProperty("ObjectId")]
        public string DeletedObjectId { get; set; } // The deleted avatar

        public DeleteAvatar_Received(string deletedObjectId, bool wasSuccessful) : base(wasSuccessful)
        {
            DeletedObjectId = deletedObjectId;
            this.MessageType = "DeleteAvatar";
        }
    }
}