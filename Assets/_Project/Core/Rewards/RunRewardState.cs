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

    public int AddReward(RewardDefinition reward)
    {
        if (!_rewards.ContainsKey(reward.Type))
            _rewards[reward.Type] = 0;

        _rewards[reward.Type] += reward.Amount;

        return _rewards[reward.Type];
    }

    public Dictionary<RewardType, int> GetRewards()
    {
        return new Dictionary<RewardType, int>(_rewards);
    }

    public void Clear()
    {
        _rewards.Clear();
    }
}