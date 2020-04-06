using UnityEngine;

public class TeleportCoordinates : MonoBehaviour
{
    [SerializeField]
    public Vector3 coordinates = Vector3.zero;
    [SerializeField]
    public Quaternion rotation = Quaternion.identity;
    [SerializeField]
    public Vector3 scale = Vector3.one;
    [SerializeField]
    public bool IsSnapZone = false;

    public TeleportCoordinates(Vector3 coordinates, Quaternion rotation, Vector3 scale, bool isSnapZone)
    {
        this.coordinates = coordinates;
        this.rotation = rotation;
        this.scale = scale;
        this.IsSnapZone = isSnapZone;
    }

    public TeleportCoordinates(GameObject gameObjectToTeleportTo, bool isSnapZone)
    {
        coordinates = gameObjectToTeleportTo.transform.position;
        rotation = gameObjectToTeleportTo.transform.rotation;
        scale = gameObjectToTeleportTo.transform.localScale;

        IsSnapZone = isSnapZone;
    }

    /// <summary>
    /// Global coordinates for Teleport, local for SnapZone
    /// </summary>
    /// <returns></returns>
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

    public void SetSnapZone(bool tf)
    {
        IsSnapZone = tf;
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
