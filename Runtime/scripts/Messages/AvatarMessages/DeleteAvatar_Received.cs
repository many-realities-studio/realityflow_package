using Newtonsoft.Json;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    public class DeleteAvatar_Received : ConfirmationMessage_Received
    {
        [JsonProperty("avatarId")]
        public string DeletedAvatarId { get; set; } // The deleted avatar

        public DeleteAvatar_Received(string deletedAvatarId, bool wasSuccessful) : base(wasSuccessful)
        {
            DeletedAvatarId = deletedAvatarId;
            this.MessageType = "DeleteAvatar";
        }
    }
}