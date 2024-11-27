using UnityEngine;
using System.Collections;

public class SmoothRotateAroundAxis : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float minRotationAngleX = -45f; 
    [SerializeField] private float maxRotationAngleX = 45f; 
    [SerializeField] private float minRotationAngleY = -45f;
    [SerializeField] private float maxRotationAngleY = 45f; 
    [SerializeField] private float minRotationAngleZ = -45f;
    [SerializeField] private float maxRotationAngleZ = 45f; 
    [SerializeField] private float rotationDuration = 2f; 
    [SerializeField] private float pauseDuration = 0.5f; 

    private Vector3 currentRotation; 

    private void Start()
    {
        StartCoroutine(RotateBetweenAngles());
    }

    private IEnumerator RotateBetweenAngles()
    {
        while (true) 
        {
            float randomYAngle = Random.Range(minRotationAngleY, maxRotationAngleY);

            yield return StartCoroutine(RotateToAngles(new Vector3(maxRotationAngleX, randomYAngle, maxRotationAngleZ)));
            yield return new WaitForSeconds(pauseDuration);
            yield return StartCoroutine(RotateToAngles(new Vector3(minRotationAngleX, randomYAngle, minRotationAngleZ)));
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    private IEnumerator RotateToAngles(Vector3 targetRotation)
    {
        float elapsedTime = 0f;
        Vector3 startingRotation = currentRotation;
        currentRotation = targetRotation;

        while (elapsedTime < rotationDuration)
        {
            float xAngle = Mathf.Lerp(startingRotation.x, currentRotation.x, (elapsedTime / rotationDuration));
            float yAngle = Mathf.Lerp(startingRotation.y, currentRotation.y, (elapsedTime / rotationDuration));
            float zAngle = Mathf.Lerp(startingRotation.z, currentRotation.z, (elapsedTime / rotationDuration));

            transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);

            elapsedTime += Time.deltaTime;
            yield return null; }

        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
