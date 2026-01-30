using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    [SerializeField] float mouseSensitivity = 3f;
    float pitch = 0f; 

    void LateUpdate()
    {
        if (target == null) return;

        float mouseY = Input.GetAxis("Mouse Y");
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 45f); 
        Quaternion rotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.LookAt(target.position + Vector3.up * 1.5f); 
    }
}
