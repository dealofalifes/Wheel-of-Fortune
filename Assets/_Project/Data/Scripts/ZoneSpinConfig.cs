using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortuneWheel
{
    [CreateAssetMenu(
    fileName = "zone_spin_",
    menuName = "WheelGame/Zone Spin Config"
)]
    public class ZoneSpinConfig : ScriptableObject
    {
        [Header("Zone Info")]
        public ZoneType ZoneType;

        [Header("Wheel Content")]
        public List<WheelSliceDefinition> Slices;
    }

    public enum ZoneType
    {
        Normal,
        Safe,
        Super
    }
}
