using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class WheelView : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private WheelUIElement[] slots = new WheelUIElement[8];

    [Header("UI")]
    [SerializeField] private RectTransform wheelRoot;
    [SerializeField] private Image wheelImage;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Sprite[] wheelSprites;
    [SerializeField] private Sprite[] indicatorSprites;
    [SerializeField] private Button spinButton;

    [Header("Flow")]
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private float settleBackDuration = 1f;
    [SerializeField] private int extraRotations = 3;

    [Header("Runtime & Debug")]
    [SerializeField] private WheelSpinDemo wheelSpinDemo;

    private Tween spinTween;
    public void Init(WheelSpinDemo wheelSpinDemo, Action onSpinClicked, ZoneSpinConfig currentZone)
    {
        this.wheelSpinDemo = wheelSpinDemo;
        spinButton.onClick.AddListener(()=> onSpinClicked?.Invoke());
    }

    public void UpdateUI(ZoneSpinConfig currentZone, int currentZoneIndex)
    {
        UpdateSlotElements(currentZone);
        UpdateWheelSkin(currentZoneIndex);
    }

    public void OnSpinStart(Action onSpinEnd, int sliceIndex)
    {
        spinButton.interactable = false;

        const int sliceCount = 8;
        float anglePerSlice = 360f / sliceCount;

        // Pointer offset (2 slices)
        //float offset = anglePerSlice * 2f;

        float targetAngle = sliceIndex * anglePerSlice;
        float mainSpinAngle = extraRotations * 360f - targetAngle; //- offset;

        Debug.Log("targetAngle: " + targetAngle + " / mainSpinAngle: " + mainSpinAngle);
        float overshootAngle = UnityEngine.Random.Range(5f, 15f);
        float wobbleAngle = 3f;

        spinTween?.Kill();

        Sequence seq = DOTween.Sequence();

        // 1. FAST START + MAIN SPIN
        seq.Append(
            wheelRoot.DORotate(
                new Vector3(0, 0, -(mainSpinAngle + overshootAngle)),
                spinDuration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuart) // important
        );

        // 2. MICRO VIBRATION (left-right-left-stop)
        seq.Append(
            wheelRoot.DORotate(
                new Vector3(0, 0, -(mainSpinAngle - wobbleAngle)),
                settleBackDuration,
                RotateMode.Fast
            ).SetEase(Ease.InOutSine)
        );

        seq.Append(
            wheelRoot.DORotate(
                new Vector3(0, 0, -(mainSpinAngle + wobbleAngle * 0.5f)),
                settleBackDuration / 2f,
                RotateMode.Fast
            ).SetEase(Ease.InOutSine)
        );

        seq.Append(
            wheelRoot.DORotate(
                new Vector3(0, 0, -mainSpinAngle),
                settleBackDuration / 2f,
                RotateMode.Fast
            ).SetEase(Ease.OutSine)
        );

        seq.OnComplete(() =>
        {
            spinButton.interactable = true;
            onSpinEnd?.Invoke();
        });

        spinTween = seq;
    }

    private void UpdateSlotElements(ZoneSpinConfig currentZone)
    {
        int length = slots.Length;
        for (int i = 0; i < length; i++)
        {
            var slideDef = currentZone.Slices[i];
            slots[i].UpdateUI(slideDef);
        }
    }

    private void OnDestroy()
    {
        spinButton?.onClick.RemoveAllListeners();
    }

    private void UpdateWheelSkin(int currentZone)
    {
        if (currentZone % 30 == 0)
        {
            wheelImage.sprite = wheelSprites[(int)WheelType.Gold];
            indicatorImage.sprite = indicatorSprites[(int)WheelType.Gold];
        }
        else if (currentZone % 5 == 0)
        {
            wheelImage.sprite = wheelSprites[(int)WheelType.Silver];
            indicatorImage.sprite = indicatorSprites[(int)WheelType.Silver];
        }
        else
        {
            wheelImage.sprite = wheelSprites[(int)WheelType.Bronze];
            indicatorImage.sprite = indicatorSprites[(int)WheelType.Bronze];
        }
    }

    public enum WheelType
    {
        Bronze,
        Silver,
        Gold,
    }
}
