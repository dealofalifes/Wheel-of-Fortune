using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FortuneWheel.UI
{
    public class ZoneTargetView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI superZoneLevelText;
        [SerializeField] private TextMeshProUGUI safeZoneLevelText;

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

            safeZoneLevelText.text = nextSafeZone.ToString();
            superZoneLevelText.text = nextSuperZone.ToString();
        }

        private int GetNextOrCurrentZone(int currentZone, int mod)
        {
            if (mod <= 0)
                return currentZone;

            return ((currentZone + mod - 1) / mod) * mod;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoBindReferences();
        }

        private void AutoBindReferences()
        {
            if (superZoneLevelText == null)
                superZoneLevelText = FortuneComponentFinder.FindTextByName("super_zone_level", transform);

            if (safeZoneLevelText == null)
                safeZoneLevelText = FortuneComponentFinder.FindTextByName("safe_zone_level", transform);
        }
#endif
    }
}
