using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 2, -4);
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float smoothSpeed = 10f;

    float yaw = 0f;
    float pitch = 15f;

    void LateUpdate()
    {
        if (target == null) return;

        // Rotacija kamere preko miša
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        // Izračunaj rotaciju kamere
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Pozicija kamere oko igrača
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smooth pomeranje
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Kamera uvek gleda u igrača
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
