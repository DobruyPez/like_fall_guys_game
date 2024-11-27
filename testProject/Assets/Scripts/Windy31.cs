using UnityEngine;
using System.Collections;

public class Windy31 : MonoBehaviour
{
    public delegate void ChangeSpeed(float windSpeed);
    public event ChangeSpeed OnChangeSpeed;

    [SerializeField] private float windStrength = 5f;  
    [SerializeField] private float secondsToWait = 2f; 
    private float currentWindSpeed = 0f;
    private float direction = 1f; 
    private Coroutine windRoutine;

    private void Start()
    {
        windRoutine = StartCoroutine(ChangeWindDirection());
    }

    private IEnumerator ChangeWindDirection()
    {
        while (true)
        {
            direction = Random.Range(0, 2) == 0 ? 1f : -1f;  
            currentWindSpeed = windStrength * direction;
            OnChangeSpeed?.Invoke(currentWindSpeed);  
            yield return new WaitForSeconds(secondsToWait);  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnChangeSpeed?.Invoke(currentWindSpeed);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnChangeSpeed?.Invoke(0f);  
        }
    }

    public float GetCurrentWindSpeed()
    {
        return currentWindSpeed;
    }
}
