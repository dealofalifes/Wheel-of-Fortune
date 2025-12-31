using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WheelResolver
{
    public int Resolve(ZoneSpinConfig config)
    {
        if (config == null || config.Slices.Count == 0)
            return -1;

        int totalWeight = 0;
        for (int i = 0; i < config.Slices.Count; i++)
            totalWeight += config.Slices[i].Weight;

        int roll = Random.Range(0, totalWeight);

        int cumulative = 0;
        for (int i = 0; i < config.Slices.Count; i++)
        {
            cumulative += config.Slices[i].Weight;
            if (roll < cumulative)
                return i;
        }

        return config.Slices.Count - 1;
    }
}