using UnityEngine;

public class DestroyIfBelowY : MonoBehaviour
{
    [Header("Threshold Settings")]
    [SerializeField] private float thresholdY = -5f; 

    private void Update()
    {
        if (transform.position.y < thresholdY)
        {
            Destroy(gameObject); 
        }
    }
}
