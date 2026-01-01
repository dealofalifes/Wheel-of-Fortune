using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FortuneWheel.UI
{
    public class RewardShowcaseView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI rewardNameText;
        [SerializeField] private TextMeshProUGUI rewardAmountText;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private Image rewardGlowEffectImage;
        [SerializeField] private Button claimButton;
        [SerializeField] private CanvasGroup containerCanvasGroup;
        [SerializeField] private CanvasGroup claimButtonCanvasGroup;

        [Header("Timings")]
        [SerializeField] private float containerFadeDuration = 0.4f;
        [SerializeField] private float claimButtonDelay = 0.6f;
        [SerializeField] private float claimButtonFadeDuration = 0.3f;

        [Header("Glow Rotation")]
        [SerializeField] private float glowRotationSpeed = 60f; // degrees per second

        [Header("Reward Pulse")]
        [SerializeField] private float pulseScaleMultiplier = 1.08f;
        [SerializeField] private float pulseDuration = 0.8f;

        private Coroutine openRoutine;
        private Coroutine glowRoutine;
        private Coroutine pulseRoutine;
        private Coroutine closeRoutine;

        private Vector3 rewardIconBaseScale;

        public void Init()
        {
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(OnClaimButtonClicked);

            rewardIconBaseScale = rewardIcon.transform.localScale;
            ResetVisualState();
        }

        public void Open(RewardDefinition reward, int amount)
        {
            StopAllRunningCoroutines();
            ResetVisualState();

            rewardNameText.text = reward.RewardName;
            rewardAmountText.text = "x" + amount;
            rewardIcon.sprite = reward.ShowcaseIcon;

            rewardGlowEffectImage.color = reward.UiColor;

            openRoutine = StartCoroutine(OpenSequence());
            glowRoutine = StartCoroutine(GlowRotationLoop());
            pulseRoutine = StartCoroutine(RewardPulseLoop());
        }

        public void Close()
        {
            StopAllRunningCoroutines();
            closeRoutine = StartCoroutine(CloseSequence());
        }

        private void OnClaimButtonClicked()
        {
            Close();
        }

        private IEnumerator OpenSequence()
        {
            containerCanvasGroup.gameObject.SetActive(true);

            // Container fade in
            yield return FadeCanvasGroup(containerCanvasGroup, 0f, 1f, containerFadeDuration);

            // Delay before claim button
            yield return new WaitForSeconds(claimButtonDelay);

            // Claim button fade in
            yield return FadeCanvasGroup(claimButtonCanvasGroup, 0f, 1f, claimButtonFadeDuration);
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
        {
            cg.alpha = from;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }

            cg.alpha = to;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        private IEnumerator GlowRotationLoop()
        {
            while (true)
            {
                rewardGlowEffectImage.transform.Rotate(0f, 0f, -glowRotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator RewardPulseLoop()
        {
            Vector3 targetScale = rewardIconBaseScale * pulseScaleMultiplier;

            while (true)
            {
                yield return ScaleOverTime(rewardIcon.transform, rewardIconBaseScale, targetScale, pulseDuration * 0.5f);
                yield return ScaleOverTime(rewardIcon.transform, targetScale, rewardIconBaseScale, pulseDuration * 0.5f);
            }
        }

        private IEnumerator ScaleOverTime(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                target.localScale = Vector3.Lerp(from, to, t / duration);
                yield return null;
            }
            target.localScale = to;
        }

        private IEnumerator CloseSequence()
        {
            // Disable interaction immediately
            containerCanvasGroup.interactable = false;
            containerCanvasGroup.blocksRaycasts = false;
            claimButtonCanvasGroup.interactable = false;
            claimButtonCanvasGroup.blocksRaycasts = false;

            // Fade out container
            yield return FadeCanvasGroup(
                containerCanvasGroup,
                containerCanvasGroup.alpha,
                0f,
                containerFadeDuration
            );

            // Stop looping effects AFTER visual exit
            StopAllRunningCoroutines();

            ResetVisualState();
            containerCanvasGroup.gameObject.SetActive(false);
        }

        private void StopAllRunningCoroutines()
        {
            if (openRoutine != null) StopCoroutine(openRoutine);
            if (glowRoutine != null) StopCoroutine(glowRoutine);
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);

            openRoutine = null;
            glowRoutine = null;
            pulseRoutine = null;
        }

        private void ResetVisualState()
        {
            containerCanvasGroup.alpha = 0f;
            containerCanvasGroup.interactable = false;
            containerCanvasGroup.blocksRaycasts = false;

            claimButtonCanvasGroup.alpha = 0f;
            claimButtonCanvasGroup.interactable = false;
            claimButtonCanvasGroup.blocksRaycasts = false;

            rewardIcon.transform.localScale = rewardIconBaseScale;
            rewardGlowEffectImage.transform.localRotation = Quaternion.identity;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoBindReferences();
        }

        private void AutoBindReferences()
        {
            if (containerCanvasGroup == null)
                containerCanvasGroup = FortuneComponentFinder.FindCanvasGroupByName("container", transform);

            if (claimButtonCanvasGroup == null)
                claimButtonCanvasGroup = FortuneComponentFinder.FindCanvasGroupByName("claim", transform);

            if (rewardNameText == null)
                rewardNameText = FortuneComponentFinder.FindTextByName("reward_name", transform);

            if (rewardAmountText == null)
                rewardAmountText = FortuneComponentFinder.FindTextByName("reward_amount", transform);

            if (claimButton == null)
                claimButton = FortuneComponentFinder.FindButtonByName("claim", transform);

            if (rewardIcon == null)
                rewardIcon = FortuneComponentFinder.FindImageByName("reward_icon", transform);

            if (rewardGlowEffectImage == null)
                rewardGlowEffectImage = FortuneComponentFinder.FindImageByName("glow_effect", transform);
        }
#endif
    }
}

