using System.Collections;
using UnityEngine;

public class CampFire : SwitchObject
{
    [SerializeField] private float fuel = 100f;
    [SerializeField] private float decrease = 0.5f;
    private void Update()
    {
        if (fuel > 0 && OnOff)
        {
            fuel -= decrease * Time.deltaTime;
        }
        if (fuel <= 0)
        {
            OnOff = false;
            someThing.SetActive(OnOff);
        }
    }
}