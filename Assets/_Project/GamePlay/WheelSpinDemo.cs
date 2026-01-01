using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WheelSpinDemo : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private WheelView wheelView;
    [SerializeField] private HeaderZoneView zoneView;
    [SerializeField] private RewardsView rewardView;
    [SerializeField] private RewardShowcaseView showcaseView;
    [SerializeField] private ZoneTargetView zoneTargetView;

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
    }

    private void Spin()
    {
        if (isSpinning)
            return;

        int sliceIndex = wheelResolver.Resolve(currentZoneConfig);
        currentSliceDefinition = currentZoneConfig.Slices[sliceIndex];

        wheelView.OnSpinStart(OnSpinEnd, sliceIndex);
    }

    public void OnSpinEnd()
    {
        isSpinning = false;

        if (currentSliceDefinition.SliceType == WheelSliceType.Bomb)
        {
            Debug.Log("Bomb has been found!");
        }
        else
        {
            int totalAmount = currentRewards.AddReward(currentSliceDefinition.Reward);
            currentZone++;
            currentZoneConfig = PickNewZone();

            wheelView.UpdateUI(currentZoneConfig, currentZone);
            rewardView.UpdateReward(currentSliceDefinition.Reward, totalAmount);
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

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        FindReferences();
        CollectAndSortZoneConfigs();
    }

    private void FindReferences()
    {
        if(wheelView == null)
        {
            var wheelViewRef = FindFirstObjectByType<WheelView>();

            if (wheelViewRef != null)
            {
                wheelView = wheelViewRef;
            }
            else
            {
                Debug.LogError("There is no WheelView Component on the Scene!");
            }
        }
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