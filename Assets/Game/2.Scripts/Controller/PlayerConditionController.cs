using UnityEngine;
using UnityEngine.UI;

public class PlayerConditionController : MonoBehaviour
{
    public static PlayerConditionController Instance { get; private set; }
    [SerializeField, Range(0, 100)] private float temperature = 80f;
    [SerializeField] private float tempDiminutionRate = 0.1f;
    [SerializeField] private float tempincrementRate = 5.0f;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Image frozenImage;
    [SerializeField] private GameObject heatingSign;
    [SerializeField, Range(0, 100)] private float frozen_LowerLim;

    public bool isHeating = false;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleTemperature();
        temperatureSlider.value = temperature;
        frozenImage.color = new Color(1, 1, 1, (100 - (temperature + frozen_LowerLim)) /100);
    }

    private void HandleTemperature()
    {
        heatingSign.SetActive(isHeating);
        if (isHeating)
        {
            if (temperature > 100)
            {
                temperature = 100;
                return;
            }
            temperature += tempincrementRate * Time.deltaTime;
        }
        else
            temperature -= tempDiminutionRate * Time.deltaTime;
    }
}