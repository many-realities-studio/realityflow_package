using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages
{
    /// <summary>
    /// Message format of a response from the server upon a request of a list of all projects that a user has
    /// </summary>
    public class GetAllUserProjects_Received : ReceivedMessage
    {
        /// <summary>
        /// in the format of (ProjectId, Name)
        /// </summary>
        [JsonProperty("Projects")]
        public List<FlowProject> Projects { get; set; } 

        public GetAllUserProjects_Received(List<FlowProject> projectList) : base(typeof(GetAllUserProjects_Received))
        {
            this.MessageType = "FetchProjects";
            this.Projects = projectList;
        }
    }
}
