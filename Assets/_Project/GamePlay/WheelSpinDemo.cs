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

    [Header("Zone Configs")]
    [SerializeField] private ZoneSpinConfig[] zoneConfigs;

    [Header("Runtime & Debug")]
    [SerializeField] private int currentZone = 1;
    [SerializeField] private bool isSpinning;

    [SerializeField] private ZoneSpinConfig currentZoneConfig;
    [SerializeField] private WheelSliceDefinition currentSliceDefinition;

    private WheelResolver wheelResolver;
    private RunRewardState currentRewards;
    private void Start()
    {
        currentZoneConfig = zoneConfigs[currentZone - 1];

        currentRewards = new();
        wheelResolver = new();
        wheelView.Init(this, Spin, currentZoneConfig);
        wheelView.UpdateUI(currentZoneConfig, currentZone);
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
            currentRewards.AddReward(currentSliceDefinition.Reward);
            currentZone++;
            currentZoneConfig = PickNewZone();
            wheelView.UpdateUI(currentZoneConfig, currentZone);
            Debug.Log("Reward " + currentSliceDefinition.Reward.Id + " has been added! Current Zone: " + currentZone);
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