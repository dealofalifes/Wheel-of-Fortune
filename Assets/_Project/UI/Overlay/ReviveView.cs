using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel.UI
{
    public class ReviveView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button continueButton;

        [SerializeField] private Image bombIcon;
        [SerializeField] private Image bombGlowEffect;

        [SerializeField] private CanvasGroup containerCanvasGroup;

        [Header("Timings")]
        [SerializeField] private float containerFadeDuration = 0.4f;

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

        public void Init(Action onRestartGame)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(()=> { Close(); onRestartGame?.Invoke(); });

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(Close);

            rewardIconBaseScale = bombIcon.transform.localScale;
            ResetVisualState();
        }

        public void Open()
        {
            StopAllRunningCoroutines();
            ResetVisualState();

            openRoutine = StartCoroutine(OpenSequence());
            glowRoutine = StartCoroutine(GlowRotationLoop());
            pulseRoutine = StartCoroutine(RewardPulseLoop());
        }

        public void Close()
        {
            StopAllRunningCoroutines();
            closeRoutine = StartCoroutine(CloseSequence());
        }

        private IEnumerator OpenSequence()
        {
            containerCanvasGroup.gameObject.SetActive(true);

            yield return FadeCanvasGroup(containerCanvasGroup, 0f, 1f, containerFadeDuration);
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
                bombGlowEffect.transform.Rotate(0f, 0f, -glowRotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator RewardPulseLoop()
        {
            Vector3 targetScale = rewardIconBaseScale * pulseScaleMultiplier;

            while (true)
            {
                yield return ScaleOverTime(bombIcon.transform, rewardIconBaseScale, targetScale, pulseDuration * 0.5f);
                yield return ScaleOverTime(bombIcon.transform, targetScale, rewardIconBaseScale, pulseDuration * 0.5f);
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

            bombIcon.transform.localScale = rewardIconBaseScale;
            bombGlowEffect.transform.localRotation = Quaternion.identity;
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

            if (restartButton == null)
                restartButton = FortuneComponentFinder.FindButtonByName("restart", transform);

            if (continueButton == null)
                continueButton = FortuneComponentFinder.FindButtonByName("continue", transform);

            if (bombIcon == null)
                bombIcon = FortuneComponentFinder.FindImageByName("bomb_icon", transform);

            if (bombGlowEffect == null)
                bombGlowEffect = FortuneComponentFinder.FindImageByName("glow_effect", transform);
        }
#endif
    }
}
