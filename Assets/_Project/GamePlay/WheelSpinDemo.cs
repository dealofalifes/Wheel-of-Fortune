using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FortuneWheel.UI;

namespace FortuneWheel
{
    public class WheelSpinDemo : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private WheelView wheelView;
        [SerializeField] private HeaderZoneView zoneView;
        [SerializeField] private RewardsView rewardView;
        [SerializeField] private RewardShowcaseView showcaseView;
        [SerializeField] private ZoneTargetView zoneTargetView;
        [SerializeField] private ReviveView reviveView;
        [SerializeField] private ExitView exitView;

        [Header("Zone Configs")]
        [SerializeField] private ZoneSpinConfig[] zoneConfigs;

        [Header("Runtime & Debug")]
        [SerializeField] private int currentZone = 1;
        [SerializeField] private bool isSpinning;

        [SerializeField] private ZoneSpinConfig currentZoneConfig;
        [SerializeField] private WheelSliceDefinition currentSliceDefinition;

        public static int safeMod = 5;
        public static int superMod = 30;

        private WheelResolver wheelResolver;
        private RunRewardState currentRewards;
        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            isSpinning = false;

            currentRewards = new();
            wheelResolver = new();

            currentZone = 1;
            currentZoneConfig = zoneConfigs[currentZone - 1];

            wheelView.Init(Spin);
            wheelView.UpdateUI(currentZoneConfig, currentZone);

            rewardView.Init(OnExitButtonClicked);

            zoneView.Init();
            showcaseView.Init();

            zoneTargetView.Init();
            zoneTargetView.UpdateUI(currentZone);

            reviveView.Init(OnRestartButtonClicked);

            exitView.Init(OnCollectRewardsButtonClicked);
        }

        private void Spin()
        {
            if (isSpinning)
                return;

            int sliceIndex = wheelResolver.Resolve(currentZoneConfig);
            currentSliceDefinition = currentZoneConfig.Slices[sliceIndex];

            wheelView.OnSpinStart(OnSpinEnd, sliceIndex);

            rewardView.SetExitStateActive(false);
        }

        public void OnSpinEnd()
        {
            isSpinning = false;

            if (currentSliceDefinition.SliceType == WheelSliceType.Bomb)
            {
                reviveView.Open();
            }
            else
            {
                int totalAmount = currentRewards.AddReward(currentSliceDefinition.Reward);
                currentZone++;
                currentZoneConfig = PickNewZone();

                wheelView.UpdateUI(currentZoneConfig, currentZone);

                rewardView.UpdateReward(currentSliceDefinition.Reward, totalAmount);
                rewardView.SetExitStateActive(IsItSpecialZone());

                zoneView.SetLevel(currentZone);

                showcaseView.Open(currentSliceDefinition.Reward, currentSliceDefinition.Reward.Amount);

                zoneTargetView.UpdateUI(currentZone);
            }
        }

        private ZoneSpinConfig PickNewZone()
        {
            ZoneType nextZoneType = ZoneType.Normal;

            if (currentZone % 30 == 0)
                nextZoneType = ZoneType.Super;
            else if (currentZone % 5 == 0)
                nextZoneType = ZoneType.Safe;

            var candidates = zoneConfigs
                .Where(z => z.ZoneType == nextZoneType)
                .ToArray();

            if (candidates.Length == 0)
            {
                Debug.LogError($"No ZoneSpinConfig found for ZoneType: {nextZoneType}");
                return null;
            }

            int randomIndex = Random.Range(0, candidates.Length);
            return candidates[randomIndex];
        }

        public void OnExitButtonClicked()
        {
            exitView.Open();
        }

        public void OnRestartButtonClicked()
        {
            isSpinning = true;

            Invoke(nameof(StartGame), 1);
        }

        public bool IsItSpecialZone()
        {
            return currentZone % WheelSpinDemo.safeMod == 0 || currentZone % WheelSpinDemo.superMod == 0;
        }

        public void OnCollectRewardsButtonClicked()
        {
            Debug.Log("Rewards has been collected!");
            isSpinning = true;

            Invoke(nameof(StartGame), 1);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FindReferences();
            CollectAndSortZoneConfigs();
        }

        private void FindReferences()
        {
            //Setup Views
            if (wheelView == null)
                wheelView = FindFirstObjectByType<WheelView>();

            if (wheelView == null)
                Debug.LogError("There is no WheelView Component on the Scene!");

            if (zoneView == null)
                zoneView = FindFirstObjectByType<HeaderZoneView>();

            if (zoneView == null)
                Debug.LogError("There is no HeaderZoneView Component on the Scene!");

            if (rewardView == null)
                rewardView = FindFirstObjectByType<RewardsView>();

            if (rewardView == null)
                Debug.LogError("There is no RewardsView Component on the Scene!");

            if (showcaseView == null)
                showcaseView = FindFirstObjectByType<RewardShowcaseView>();

            if (showcaseView == null)
                Debug.LogError("There is no RewardShowcaseView Component on the Scene!");

            if (zoneTargetView == null)
                zoneTargetView = FindFirstObjectByType<ZoneTargetView>();

            if (zoneTargetView == null)
                Debug.LogError("There is no ZoneTargetView Component on the Scene!");

            if (reviveView == null)
                reviveView = FindFirstObjectByType<ReviveView>();

            if (reviveView == null)
                Debug.LogError("There is no ReviveView Component on the Scene!");

            if (exitView == null)
                exitView = FindFirstObjectByType<ExitView>();

            if (exitView == null)
                Debug.LogError("There is no ExitView Component on the Scene!");
        }

        private void CollectAndSortZoneConfigs()
        {
            // Find all ZoneSpinConfig assets in the project
            string[] guids = AssetDatabase.FindAssets("t:ZoneSpinConfig");

            zoneConfigs = guids
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<ZoneSpinConfig>(
                        AssetDatabase.GUIDToAssetPath(guid)))
                .Where(config => config != null)
                .ToArray();

            // Mark object dirty so Unity saves the change
            EditorUtility.SetDirty(this);
        }
#endif
    }
}