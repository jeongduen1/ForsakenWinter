using System.Collections;
using UnityEngine;

public class Campfire : ObjectToggle
{
    [SerializeField] private Transform player;

    [Header("Campfire Settings")]
    [SerializeField] private float heatingDistance = 3f;
    [SerializeField] private float fuelAmount = 10.0f;
    [SerializeField] private float fuelConsumptionRate = 1.0f;
    [SerializeField] private GameObject fireLight;

    private float distance;

    private Coroutine fuelCoroutine;

    private void Update()
    {
        distance = (player.position - (gameObject.transform.position) ).magnitude;
        if (distance <= heatingDistance && isActive)
            PlayerConditionController.Instance.isHeating = true;
        else PlayerConditionController.Instance.isHeating = false;
    }

    public override void OnInteract()
    {
        if (fuelAmount > 0)
        {
            base.OnInteract();
            ToggleFire(isActive);

            if (isActive)
                StartBurning();
            else if (!isActive)
                StopBurning();
        }
    }

    private void ToggleFire(bool state)
    {
        fireLight.SetActive(state);
    }

    private void StartBurning()
    {
        if (fuelCoroutine == null)
            fuelCoroutine = StartCoroutine(BurnFuel());
    }

    private void StopBurning()
    {
        if (fuelCoroutine != null)
        {
            StopCoroutine(fuelCoroutine);
            fuelCoroutine = null;
        }
    }

    private IEnumerator BurnFuel()
    {
        while (fuelAmount > 0)
        {
            fuelAmount -= fuelConsumptionRate * Time.deltaTime;
            yield return null;
        }

        ToggleFire(false);
        isActive = false;
        StopBurning();
    }

    public void AddFuel(float amount)
    {
        fuelAmount += amount;
    }
}
