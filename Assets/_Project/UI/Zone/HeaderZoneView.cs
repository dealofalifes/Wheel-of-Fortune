using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortuneWheel.UI
{
    public class HeaderZoneView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform content;
        [SerializeField] private ZoneLevelUIElement itemPrefab;

        [Header("Zone Flow")]
        [SerializeField] private int visibleItemCount = 5;
        [SerializeField] private float itemSpacing = 100f;
        [SerializeField] private float moveDuration = 0.35f;

        [Header("Color Bank")]
        [SerializeField] private Color defaultZoneColor = Color.white;
        [SerializeField] private Color safeZoneColor = Color.green;
        [SerializeField] private Color superZoneColor = Color.yellow;

        private readonly List<ZoneLevelUIElement> _items = new();
        private int _currentZone = 1;
        private int _centerIndex;

        public void Init()
        {
            ClearItems();
            _items.Clear();

            _centerIndex = visibleItemCount / 2;
            _currentZone = 1;

            InitializeItems();
            UpdateVisuals();
        }

        private void InitializeItems()
        {
            for (int i = 0; i < visibleItemCount; i++)
            {
                var item = Instantiate(itemPrefab, content);
                _items.Add(item);

                var rect = item.transform as RectTransform;
                rect.anchoredPosition = new Vector2(i * itemSpacing, 0);
            }
        }

        public void SetLevel(int level)
        {
            if (level <= _currentZone)
                return;

            _currentZone = level;
            AnimateShift();
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                int level = _currentZone + (i - _centerIndex);

                if (level < 1)
                {
                    _items[i].SetLevel(-1, defaultZoneColor); //sets Empty
                }
                else
                {
                    Color zoneColor = defaultZoneColor;
                    if (level % WheelSpinDemo.superMod == 0)
                        zoneColor = superZoneColor;
                    else if (level % WheelSpinDemo.safeMod == 0)
                        zoneColor = safeZoneColor;

                    _items[i].SetLevel(level, zoneColor);
                }

                _items[i].SetActive(i == _centerIndex);
            }
        }

        private void AnimateShift()
        {
            content.DOKill();

            content
                .DOAnchorPosX(-itemSpacing, moveDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    content.anchoredPosition = Vector2.zero;
                    UpdateVisuals();
                });
        }

        private void ClearItems()
        {
            int length = content.childCount;
            for (int i = length - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
    }
}
