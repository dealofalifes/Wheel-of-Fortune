using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel.UI
{
    public class ZoneLevelUIElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image highlight;

        public void SetLevel(int level, Color zoneColor)
        {
            levelText.color = zoneColor;
            levelText.text = level > 0 ? level.ToString() : "";
            highlight.color = zoneColor;
            highlight.gameObject.SetActive(false);
        }

        public void SetActive(bool active)
        {
            highlight.gameObject.SetActive(active);
        }
    }
}
