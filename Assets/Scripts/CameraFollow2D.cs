using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smooth = 8f;
    public Vector2 offset;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
    }
}
