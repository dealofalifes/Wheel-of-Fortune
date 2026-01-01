using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel.UI
{
    public class WheelUIElement : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardAmountText;

        public void UpdateUI(WheelSliceDefinition sliceDef)
        {
            rewardIcon.sprite = sliceDef.Reward.ShowcaseIcon;
            rewardAmountText.text = "x" + sliceDef.Reward.Amount.ToString();
        }
    }
}
