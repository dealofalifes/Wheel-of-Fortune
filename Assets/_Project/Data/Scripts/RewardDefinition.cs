using UnityEngine;

namespace FortuneWheel
{
    [CreateAssetMenu(
    fileName = "reward_",
    menuName = "WheelGame/Reward Definition"
)]
    public class RewardDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id;
        public string RewardName;
        public Sprite ShowcaseIcon;
        public Sprite RewardIcon;

        [Header("Reward Value")]
        public RewardType Type;
        public int Amount;

        [Header("Presentation")]
        public Color UiColor;
    }

    public enum RewardType
    {
        Cash,
        Coin,
        ArmorPoint,
        KnifePoint,
        RiflePoint,
        ShotgunPoint,
        SniperPoint,
        CommonChest,
        RareChest,
        UniqueChest,
    }
}