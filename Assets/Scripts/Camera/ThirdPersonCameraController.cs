using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("目标")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0, 2f, 0);

    [Header("距离")]
    public float distance = 15f;

    [Header("旋转")]
    public float rotationSpeed = 120f;
    public float verticalAngle = 30f;
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 60f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        Vector3 dir = (transform.position - GetTargetPosition()).normalized;
        yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(-dir.y) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        UpdateCameraPosition();
    }

    private Vector3 GetTargetPosition()
    {
        return target.position + targetOffset;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPos = GetTargetPosition();

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 dir = rotation * Vector3.back;

        Vector3 desiredPos = targetPos + dir * distance;
        transform.position = desiredPos;
        transform.LookAt(targetPos);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDistance(float newDistance)
    {
        distance = newDistance;
    }
}
