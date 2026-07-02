using UnityEngine;

public class SmoothFollowUI : MonoBehaviour
{
    public Transform target; 
    public float distance = 1.8f;
    public float heightOffset = -0.1f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        
        Vector3 desiredPos = target.position + target.forward * distance;
        desiredPos.y += heightOffset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        
        Quaternion desiredRot = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, followSpeed * Time.deltaTime);
    }
}
