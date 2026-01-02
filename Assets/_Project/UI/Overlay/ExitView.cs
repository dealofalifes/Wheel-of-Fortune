using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    public class ExitView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button collectRewardsButton;
        [SerializeField] private Button goBackButton;

        [SerializeField] private CanvasGroup containerCanvasGroup;

        [Header("Timings")]
        [SerializeField] private float containerFadeDuration = 0.4f;

        private Coroutine openRoutine;
        private Coroutine closeRoutine;

        public void Init(Action onCollectRewards)
        {
            collectRewardsButton.onClick.RemoveAllListeners();
            collectRewardsButton.onClick.AddListener(() => { Close(); onCollectRewards?.Invoke(); });

            goBackButton.onClick.RemoveAllListeners();
            goBackButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            StopAllRunningCoroutines();
            ResetVisualState();

            openRoutine = StartCoroutine(OpenSequence());
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

            openRoutine = null;
        }

        private void ResetVisualState()
        {
            containerCanvasGroup.alpha = 0f;
            containerCanvasGroup.interactable = false;
            containerCanvasGroup.blocksRaycasts = false;
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

            if (collectRewardsButton == null)
                collectRewardsButton = FortuneComponentFinder.FindButtonByName("collect", transform);

            if (goBackButton == null)
                goBackButton = FortuneComponentFinder.FindButtonByName("goback", transform);
        }
#endif
    }
}
