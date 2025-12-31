using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunRewardState
{
    private readonly Dictionary<RewardType, int> _rewards;

    public RunRewardState()
    {
        _rewards = new Dictionary<RewardType, int>();
    }

    public void AddReward(RewardDefinition reward)
    {
        if (!_rewards.ContainsKey(reward.Type))
            _rewards[reward.Type] = 0;

        _rewards[reward.Type] += reward.Amount;
    }

    public void Clear()
    {
        _rewards.Clear();
    }
}