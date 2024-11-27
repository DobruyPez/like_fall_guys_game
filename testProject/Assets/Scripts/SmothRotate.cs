using UnityEngine;
using System.Collections;

public class SmoothRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float minRotationAngleX = -45f; 
    [SerializeField] private float maxRotationAngleX = 45f; 
    [SerializeField] private float minRotationAngleY = -45f;
    [SerializeField] private float maxRotationAngleY = 45f; 
    [SerializeField] private float minRotationAngleZ = -45f;
    [SerializeField] private float maxRotationAngleZ = 45f; 
    [SerializeField] private float rotationDuration = 2f; 
    [SerializeField] private float rotationSpeed = 1f; 

    [Header("Rotation Axes")]
    [SerializeField] private bool rotateAroundX = true;
    [SerializeField] private bool rotateAroundY = true;
    [SerializeField] private bool rotateAroundZ = true;

    private Quaternion initialRotation; 

    private void Start()
    {
        initialRotation = transform.rotation;

        StartCoroutine(RotateToRandomAngle());
    }

    private IEnumerator RotateToRandomAngle()
    {
        while (true) 
        {
            float randomAngleX = rotateAroundX ? Random.Range(minRotationAngleX, maxRotationAngleX) : 0f;
            float randomAngleY = rotateAroundY ? Random.Range(minRotationAngleY, maxRotationAngleY) : 0f;
            float randomAngleZ = rotateAroundZ ? Random.Range(minRotationAngleZ, maxRotationAngleZ) : 0f;

            Quaternion targetRotation = initialRotation * Quaternion.Euler(randomAngleX, randomAngleY, randomAngleZ); 

            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (elapsedTime / rotationDuration) * rotationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null; 
            }

            transform.rotation = targetRotation;

            yield return new WaitForSeconds(0.5f); 
        }
    }
}
