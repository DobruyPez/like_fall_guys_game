using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    [Header("Asset List")]
    [SerializeField] private GameObject[] objectsToSpawn; 
    [SerializeField] private float spawnInterval = 2f; 
    [SerializeField] private GameObject targetObject; 
    [SerializeField] private float forceStrengs; 

    private void Start()
    {
        InvokeRepeating(nameof(SpawnRandomObject), 0f, spawnInterval);
    }

    private void SpawnRandomObject()
    {
        if (objectsToSpawn.Length > 0)
        {
            GameObject randomObject = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

            GameObject spawnedObject = Instantiate(randomObject, transform.position, Quaternion.identity);

            if (targetObject != null)
            {
                Vector3 direction = - (targetObject.transform.position - transform.position).normalized;

                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction * forceStrengs, ForceMode.Impulse); // 10f - сила, можно настроить
                }
            }
        }
        else
        {
            Debug.LogWarning("Список объектов для спавна пуст!");
        }
    }
}
