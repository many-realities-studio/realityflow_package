using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.WebRTC.Unity;

public class DSSSignalerUI : MonoBehaviour
{
    public NodeDssSignaler NodeDssSignaler;

    /// <summary>
    /// The text input field in which we accept the target device name
    /// </summary>
    [Tooltip("The text input field in which we accept the target device name")]
    public InputField RemotePeerId;

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(NodeDssSignaler.RemotePeerId))
        {
            RemotePeerId.text = NodeDssSignaler.RemotePeerId;
        }
    }

    public void StartConnection()
    {
        if (!string.IsNullOrEmpty(RemotePeerId.text))
        {
            NodeDssSignaler.RemotePeerId = RemotePeerId.text;
            NodeDssSignaler.PeerConnection.StartConnection();
        }
    }
}
