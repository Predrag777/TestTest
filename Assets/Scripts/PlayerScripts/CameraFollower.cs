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

    [SerializeField] Vector3 lockOnOffset = new Vector3(0, 2, -3);

    float yaw = 0f;
    float pitch = 15f;

    CombatController combatController;

    void Start()
    {
        if(target != null)
            combatController = target.GetComponent<CombatController>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool hasEnemy = combatController != null && combatController.enemy != null;

        Vector3 desiredPosition;

        if(hasEnemy)
        {
            // Lock-on: kamera iza igraca, gledajuci ka neprijatelju
            Transform enemyTransform = combatController.enemy.transform;
            Vector3 dirToEnemy = (enemyTransform.position - target.position).normalized;
            dirToEnemy.y = 0f;
            dirToEnemy.Normalize();

            // Kamera ide iza igraca (suprotno od neprijatelja)
            Quaternion lockRot = Quaternion.LookRotation(dirToEnemy);
            desiredPosition = target.position + lockRot * lockOnOffset;
        }
        else
        {
            // Normalna rotacija kamere preko misa
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -30f, 60f);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            desiredPosition = target.position + rotation * offset;
        }

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
            float hitDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            desiredPosition = target.position + direction * hitDistance;
        }

        // =====================================================

        // Smooth pomeranje ka finalnoj poziciji
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if(hasEnemy)
        {
            // Gledaj izmedju igraca i neprijatelja
            Vector3 midPoint = (target.position + combatController.enemy.transform.position) / 2f;
            transform.LookAt(midPoint + Vector3.up * 1f);
        }
        else
        {
            // Kamera uvek gleda u igraca
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }
}
