using UnityEngine;

public class TeleportCoordinates : MonoBehaviour
{
    [SerializeField]
    private Vector3 coordinates = Vector3.zero;
    [SerializeField]
    private Quaternion rotation = Quaternion.identity;
    [SerializeField]
    private Vector3 scale = Vector3.one;
    [SerializeField]
    private bool isSZone = false;

    public Vector3 GetCoordinates()
    {
        return coordinates;
    }

    public Vector3 GetScale()
    {
        return scale;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }

    public bool IsSnapZone()
    {
        return isSZone;
    }

    public void SetCoordinates(Vector3 newCoordinates)
    {
        coordinates = newCoordinates;
    }
    public void SetScale(Vector3 newScale)
    {
        scale = newScale;
    }
    public void SetRotation(Quaternion newRot)
    {
        rotation = newRot;
    }
}
