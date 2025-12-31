using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelUIElement : MonoBehaviour
{
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardAmountText;

    public void UpdateUI(WheelSliceDefinition sliceDef)
    {
        rewardIcon.sprite = sliceDef.Reward.ShowcaseIcon;
        rewardAmountText.text = "x" + sliceDef.Reward.Amount.ToString();
    }
}
