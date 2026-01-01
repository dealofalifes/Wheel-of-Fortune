using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel.UI
{
    public class RewardsView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RewardUIElement rewardPrefab;
        [SerializeField] private Transform rewardPrefabsHolder;
        [SerializeField] private Button exitButton;

        private readonly Dictionary<RewardType, RewardUIElement> _rewardElements
            = new Dictionary<RewardType, RewardUIElement>();

        public void Init(Action onExitButtonClicked)
        {
            ClearRewardElements();
            _rewardElements.Clear();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => onExitButtonClicked?.Invoke());
        }

        //To update whole elements at once if necessary
        public void UpdateUI(Dictionary<RewardDefinition, int> rewards)
        {
            foreach (var kvp in rewards)
                UpdateReward(kvp.Key, kvp.Value);
        }

        public void UpdateReward(RewardDefinition rewardDef, int amount)
        {
            if (!_rewardElements.TryGetValue(rewardDef.Type, out var element))
            {
                element = CreateRewardElement(rewardDef.Type);
                _rewardElements.Add(rewardDef.Type, element);
            }

            element.UpdateUI(rewardDef, amount);
            element.gameObject.SetActive(true);
        }

        private RewardUIElement CreateRewardElement(RewardType type)
        {
            return Instantiate(rewardPrefab, rewardPrefabsHolder);
        }

        private void ClearRewardElements()
        {
            int length = rewardPrefabsHolder.childCount;
            for (int i = length - 1; i >= 0; i--)
                Destroy(rewardPrefabsHolder.GetChild(i).gameObject);
        }
    }
}
