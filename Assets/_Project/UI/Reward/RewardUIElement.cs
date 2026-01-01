using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIElement : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardAmountText;

    public void UpdateUI(RewardDefinition reward, int amount)
    {
        rewardIcon.sprite = reward.RewardIcon;
        rewardAmountText.text = "x" + amount.ToString();
    }
}
