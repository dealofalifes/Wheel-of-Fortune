using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneTargetView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI superZoneText;
    [SerializeField] private TextMeshProUGUI safeZoneText;

    public void Init()
    {
        //for later
    }

    public void UpdateUI(int currentZone)
    {
        int safeMod = WheelSpinDemo.safeMod;
        int superMod = WheelSpinDemo.superMod;

        int nextSafeZone = GetNextOrCurrentZone(currentZone, safeMod);
        int nextSuperZone = GetNextOrCurrentZone(currentZone, superMod);

        // Super zone has priority: safe zone cannot equal super zone
        if (nextSafeZone == nextSuperZone)
            nextSafeZone += safeMod;

        safeZoneText.text = nextSafeZone.ToString();
        superZoneText.text = nextSuperZone.ToString();
    }

    private int GetNextOrCurrentZone(int currentZone, int mod)
    {
        if (mod <= 0)
            return currentZone;

        return ((currentZone + mod - 1) / mod) * mod;
    }
}
