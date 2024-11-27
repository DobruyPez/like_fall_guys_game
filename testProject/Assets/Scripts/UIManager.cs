using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; 
    [SerializeField] private Slider hpSlider;                   

    private void Start()
    {
        if (playerController != null && hpSlider != null)
        {
            hpSlider.maxValue = playerController.GetMaxHP();
            hpSlider.value = playerController.GetCurrentHP();
        }
        else
        {
            Debug.LogError("PlayerController or hpSlider is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        if (playerController != null && hpSlider != null)
        {
            hpSlider.value = playerController.GetCurrentHP();
        }
    }
}
