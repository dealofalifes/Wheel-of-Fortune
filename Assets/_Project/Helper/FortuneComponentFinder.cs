
namespace FortuneWheel
{
#if UNITY_EDITOR
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public static class FortuneComponentFinder
    {
        public static Button FindButtonByName(string nameContains, Transform target)
        {
            foreach (var button in target.GetComponentsInChildren<Button>(true))
            {
                if (button.name.Contains(nameContains))
                    return button;
            }

            ShowNullError(target.name + " has missing button with: " + nameContains);
            return null;
        }

        public static CanvasGroup FindCanvasGroupByName(string nameContains, Transform target)
        {
            foreach (var canvasGroup in target.GetComponentsInChildren<CanvasGroup>(true))
            {
                if (canvasGroup.name.Contains(nameContains))
                    return canvasGroup;
            }

            ShowNullError(target.name + " has missing CanvasGroup with: " + nameContains);
            return null;
        }

        public static TextMeshProUGUI FindTextByName(string nameContains, Transform target)
        {
            foreach (var text in target.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (text.name.Contains(nameContains))
                    return text;
            }

            ShowNullError(target.name + " has missing text with: " + nameContains);
            return null;
        }

        public static Image FindImageByName(string nameContains, Transform target)
        {
            foreach (var image in target.GetComponentsInChildren<Image>(true))
            {
                if (image.name.Contains(nameContains))
                    return image;
            }

            ShowNullError(target.name + " has missing image with: " + nameContains);
            return null;
        }

        public static void ShowNullError(string text)
        {
            Debug.LogError(text);
        }
    }
#endif
}
