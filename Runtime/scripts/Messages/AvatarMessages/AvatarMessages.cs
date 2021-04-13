using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;

namespace Packages.realityflow_package.Runtime.scripts.Messages.AvatarMessages
{
    /// <summary>
    /// Response: <see cref="CreateAvatar_Received"/>
    /// </summary>
    public class CreateAvatar_SendToServer : BaseMessage
    {
        [JsonProperty("FlowAvatar")]
        public FlowAvatar flowAvatar { get; set; }

        //[JsonProperty("")]
        //public FlowUser flowUser { get; set; }

        [JsonProperty("ProjectId")]
        public string projectId { get; set; }

        public CreateAvatar_SendToServer(FlowAvatar flowAvatar, /*FlowUser flowUser,*/ string projectId)
        {
            this.flowAvatar = flowAvatar;
            this.projectId = projectId;

            this.MessageType = "CreateAvatar";
        }
    }

    /// <summary>
    /// Response: <see cref="UpdateAvatar_Received"/>
    /// </summary>
    public class UpdateAvatar_SendToServer : BaseMessage
    {
        [JsonProperty("flowAvatar")]
        public FlowAvatar flowAvatar { get; set; }

        [JsonProperty("ProjectId")]
        public string projectId { get; set; }

        public UpdateAvatar_SendToServer(FlowAvatar flowAvatar, /*FlowUser flowUser,*/ string projectId)
        {
            this.flowAvatar = flowAvatar;
            this.projectId = projectId;

            this.MessageType = "UpdateAvatar";
        }
    }

    /// <summary>
    /// Response: <see cref="DeleteAvatar_Received"/>
    /// </summary>
    public class DeleteAvatar_SendToServer : BaseMessage
    {
        [JsonProperty("ObjectId")]
        public string ObjectId { get; set; } // Id of the deleted object

        [JsonProperty("ProjectId")]
        public string ProjectId { get; set; }

        public DeleteAvatar_SendToServer(string projectId, string objectId)
        {
            ProjectId = projectId;
            this.ObjectId = objectId;
            this.MessageType = "DeleteAvatar";
        }
    }

    /// <summary>
    /// Response: <see cref="FinalizedUpdateAvatar_Received"/>
    /// </summary>
    public class FinalizedUpdateAvatar_SendToServer : BaseMessage
    {
        [JsonProperty("flowAvatar")]
        public FlowAvatar flowAvatar { get; set; }

        [JsonProperty("projectId")]
        public string projectId { get; set; }

        public FinalizedUpdateAvatar_SendToServer(FlowAvatar flowAvatar, string projectId)
        {
            this.flowAvatar = flowAvatar;
            this.projectId = projectId;

            this.MessageType = "FinalizedUpdateAvatar";
        }
    }
}