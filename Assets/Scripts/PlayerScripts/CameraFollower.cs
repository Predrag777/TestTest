using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 2, -4);
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float smoothSpeed = 10f;

    // 🔽 NOVO – podesavanja za anti-clipping
    [SerializeField] float collisionRadius = 0.3f;   // debljina kamere (SphereCast radius)
    [SerializeField] float minDistance = 0.5f;       // minimalna udaljenost od igraca
    [SerializeField] LayerMask collisionMask;        // koji layer-i blokiraju kameru

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

        // Idealna (zeljena) pozicija kamere
        Vector3 desiredPosition = target.position + rotation * offset;

        // =====================================================
        // 🔽 ANTI-CLIPPING LOGIKA
        // =====================================================

        // Pravac od igraca ka kameri
        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;

        // SphereCast je bolji od Raycast jer simulira "debljinu" kamere
        if (Physics.SphereCast(
            target.position,          // pocetna tacka
            collisionRadius,          // radius kugle
            direction,                // pravac
            out hit,                  // rezultat
            distance,                 // maksimalna udaljenost
            collisionMask             // layer-i koji blokiraju
        ))
        {
            Debug.DrawRay(target.position, direction * distance, Color.red);
            // Ako smo pogodili prepreku,
            // postavimo kameru malo ispred mesta sudara
            float hitDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            desiredPosition = target.position + direction * hitDistance;
        }

        // =====================================================

        // Smooth pomeranje ka finalnoj poziciji
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Kamera uvek gleda u igraca
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
