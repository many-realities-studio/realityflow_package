using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace RealityFlow.Plugin.Scripts
{
    public class FlowUser
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        public FlowUser(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}