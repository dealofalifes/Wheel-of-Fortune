using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortuneWheel
{
    [CreateAssetMenu(
    fileName = "slice_",
    menuName = "WheelGame/Wheel Slice"
)]
    public class WheelSliceDefinition : ScriptableObject
    {
        [Header("Slice Type")]
        public WheelSliceType SliceType;

        [Header("Reward (Only if Reward)")]
        public RewardDefinition Reward;

        [Header("Weight")]
        [Tooltip("Relative chance weight")]
        public int Weight = 1;
    }

    public enum WheelSliceType
    {
        Reward,
        Bomb
    }
}
