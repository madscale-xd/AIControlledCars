using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WaypointZone : MonoBehaviour
{
    public Vector3 GetRandomPointInZone()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 localCenter = box.center;
        Vector3 worldCenter = transform.TransformPoint(localCenter);

        Vector3 halfSize = Vector3.Scale(box.size, transform.lossyScale) * 0.5f;
        float randX = Random.Range(-halfSize.x, halfSize.x);
        float randZ = Random.Range(-halfSize.z, halfSize.z);

        return new Vector3(worldCenter.x + randX, transform.position.y, worldCenter.z + randZ);
    }
}