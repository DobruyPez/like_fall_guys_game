using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Camera Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float yPositionFactor = 0.05f;

    [Header("Camera Y Position Limits")]
    [SerializeField] private float maxDiff = 2f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseInput();
        AdjustCameraPosition();
        PreventCameraBelowPlayer();  // Новая логика
        MaintainDistanceFromPlayer();
        AdjustCameraHeight();
        transform.LookAt(playerBody.position);
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation += mouseY;
        yRotation -= mouseX; // Inverted mouse X for left-right rotation
    }

    private void AdjustCameraPosition()
    {
        Vector3 desiredPosition = playerBody.position + Quaternion.Euler(xRotation, yRotation, 0f) * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    private void PreventCameraBelowPlayer()
    {
        if (transform.position.y < playerBody.position.y)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = playerBody.position.y; 
            transform.position = newPosition;
        }
    }

    private void AdjustCameraHeight()
    {
        float yDiff = playerBody.position.y - (transform.position.y + offset.y);
        if (yDiff < maxDiff && xRotation > 0f)
        {
            offset.y += yPositionFactor;
        }
        else if (yDiff > -maxDiff && xRotation < 0f)
        {
            offset.y -= yPositionFactor;
        }
    }

    private void MaintainDistanceFromPlayer()
    {
        float distance = Vector3.Distance(transform.position, playerBody.position);
        if (distance > maxDiff)
        {
            Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;
            transform.position = playerBody.position - directionToPlayer * maxDiff;
        }
    }
}
