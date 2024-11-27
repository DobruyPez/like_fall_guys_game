using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    // Делегат для события взрыва
    public delegate void Explosion(float damage);
    public event Explosion OnExplosion;

    [SerializeField] private Color originalColor = Color.white;
    [SerializeField] private Color contactColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color explosionColor = Color.red;
    [SerializeField] private float explosionDuration = 0.2f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float delayBeforeExplosion = 1f;

    private Renderer objectRenderer;
    private bool isInCooldown = false;
    private bool playerIsOnObject = false;
    private Coroutine explosionRoutine;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material.color = originalColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnObject = true; // Set the player is on object flag
            StartCoroutine(HandlePlayerEnter());
        }
    }

    private IEnumerator HandlePlayerEnter()
    {
        // Set the contact color
        objectRenderer.material.color = contactColor;

        // Wait for 1 second
        yield return new WaitForSeconds(delayBeforeExplosion);

        // After the delay, check if the player is still on the object
        if (playerIsOnObject && !isInCooldown)
        {
            // If the player is still in contact, proceed to explosion
            explosionRoutine = StartCoroutine(HandleExplosion());
        }
        else
        {
            // If the player is not in contact, revert to original color
            objectRenderer.material.color = originalColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnObject = false; // Player is no longer on the object
            // Reset the color immediately when the player exits
            objectRenderer.material.color = originalColor;
        }
    }

    private IEnumerator HandleExplosion()
    {
        objectRenderer.material.color = explosionColor;

        // Вызываем событие взрыва
        OnExplosion?.Invoke(damage); // Передаем урон в делегат

        yield return new WaitForSeconds(explosionDuration);

        objectRenderer.material.color = originalColor;
        isInCooldown = true;

        yield return new WaitForSeconds(cooldownTime);

        isInCooldown = false;
        explosionRoutine = null;
    }
}
