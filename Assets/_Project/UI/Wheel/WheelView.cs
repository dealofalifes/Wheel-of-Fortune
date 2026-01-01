using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace FortuneWheel.UI
{
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

        private Tween spinTween;
        public void Init(Action onSpinClicked)
        {
            spinButton.onClick.RemoveAllListeners();
            spinButton.onClick.AddListener(() => onSpinClicked?.Invoke());
        }

        public void UpdateUI(ZoneSpinConfig currentZone, int currentZoneIndex)
        {
            UpdateSlotElements(currentZone);
            UpdateWheelSkin(currentZoneIndex);
        }

        public void OnSpinStart(Action onSpinEnd, int sliceIndex)
        {
            spinButton.interactable = false;

            wheelRoot.localEulerAngles =
                new Vector3(0, 0, Mathf.Repeat(wheelRoot.localEulerAngles.z, 360f));

            const int sliceCount = 8;
            float anglePerSlice = 360f / sliceCount;

            float targetAngle = sliceIndex * anglePerSlice;
            float mainSpinAngle = extraRotations * 360f - targetAngle;

            float wobbleAngle = 15f;

            spinTween?.Kill();

            Sequence seq = DOTween.Sequence();

            // 1. MAIN SPIN (absolute, can exceed 360)
            seq.Append(
                wheelRoot.DORotate(
                    new Vector3(0, 0, -(mainSpinAngle + wobbleAngle)),
                    spinDuration,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.OutFlash) //OutFlash
            );

            // 2. WOBBLE (RELATIVE — THIS IS THE FIX)

            seq.Append(
                wheelRoot.DORotate(
                    new Vector3(0, 0, wobbleAngle),
                    settleBackDuration,
                    RotateMode.LocalAxisAdd
                ).SetEase(Ease.OutElastic)
            );

            //seq.Append(
            //    wheelRoot.DORotate(
            //        new Vector3(0, 0, -wobbleAngle * 1.5f),
            //        settleBackDuration * 0.5f,
            //        RotateMode.LocalAxisAdd
            //    ).SetEase(Ease.InOutSine)
            //);

            //seq.Append(
            //    wheelRoot.DORotate(
            //        new Vector3(0, 0, +wobbleAngle * 0.5f),
            //        settleBackDuration * 0.5f,
            //        RotateMode.LocalAxisAdd
            //    ).SetEase(Ease.OutSine)
            //);

            seq.OnComplete(() =>
            {
                Invoke(nameof(SetSpinButtonInteractable), 0.5f);
                onSpinEnd?.Invoke();
            });

            spinTween = seq;
        }


        private void SetSpinButtonInteractable()
        {
            spinButton.interactable = true;
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
            if (currentZone % WheelSpinDemo.superMod == 0)
            {
                wheelImage.sprite = wheelSprites[(int)WheelType.Gold];
                indicatorImage.sprite = indicatorSprites[(int)WheelType.Gold];
            }
            else if (currentZone % WheelSpinDemo.safeMod == 0)
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoBindReferences();
        }

        private void AutoBindReferences()
        {
            if (spinButton == null)
                spinButton = FortuneComponentFinder.FindButtonByName("spin", transform);

            if (wheelImage == null)
                wheelImage = FortuneComponentFinder.FindImageByName("image_wheel", transform);

            if (indicatorImage == null)
                indicatorImage = FortuneComponentFinder.FindImageByName("image_indicator", transform);
        }
#endif
    }
}
