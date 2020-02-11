using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public struct RandomSelector : ISerializationCallbackReceiver
{
    [SerializeField] private int n;
    [SerializeField] private bool repeats;
    [SerializeField] private int[] weights;

    private int sumWeights;
    private int[] thresholds;


    public List<int> Select()
    {
        List<int> result = new List<int>(n);

        if (repeats || n == 1)
        {
            for (int i = 0; i < n; ++i)
                result.Add(Select(UnityEngine.Random.Range(0f, sumWeights)));
        }
        else
            throw new NotImplementedException();


        return result;
    }


    private int Select(float value)
    {
        int l = 0;
        int r = thresholds.Length - 1;
        
        while (l < r)
        {
            int m = l + (r - l) / 2;

            int w = thresholds[m];

            if (value == w)
                return m;
            else if (value < w)
                r = m;
            else
                l = m + 1;
        }

        
        return l;
    }


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (weights != null)
        {
            sumWeights = 0;
            
            thresholds = new int[weights.Length];
            for (int i = 0; i < weights.Length; ++i)
            {
                sumWeights += weights[i];
                thresholds[i] = sumWeights;
            }

            weights = null;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (weights == null)
        {
            weights = new int[thresholds.Length];

            if (weights.Length > 0)
            {
                for (int i = weights.Length - 1; i > 0;)
                    weights[i] = thresholds[i] - thresholds[--i];
                weights[0] = thresholds[0];

                sumWeights = 0;
            }


            thresholds = null;
        }
    }
}
