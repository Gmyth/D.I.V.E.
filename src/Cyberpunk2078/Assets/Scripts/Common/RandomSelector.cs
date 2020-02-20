using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public class RandomSelector : ISerializationCallbackReceiver
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


[Serializable] public class RandomSelector<T> : ISerializationCallbackReceiver
{
    [Serializable] public struct WeightData
    {
        public T data;
        public int weight;


        public WeightData(KeyValuePair<T, int> pair) : this(pair.Key, pair.Value)
        {
        }

        public WeightData(T data, int weight)
        {
            this.data = data;
            this.weight = weight;
        }
    }


    [SerializeField] protected int n;
    [SerializeField] protected bool repeats;
    [SerializeField] protected WeightData[] weights;

    protected int sumWeights;
    protected KeyValuePair<T, int>[] thresholds;


    public List<T> Select()
    {
        List<T> result = new List<T>(n);

        if (repeats || n == 1)
        {
            for (int i = 0; i < n; ++i)
                result.Add(Select(UnityEngine.Random.Range(0f, sumWeights)));
        }
        else
            throw new NotImplementedException();


        return result;
    }


    private T Select(float value)
    {
        int l = 0;
        int r = thresholds.Length - 1;

        while (l < r)
        {
            int m = l + (r - l) / 2;
            int w = thresholds[m].Value;

            if (value == w)
                return thresholds[m].Key;
            else if (value < w)
                r = m;
            else
                l = m + 1;
        }


        return thresholds[l].Key;
    }


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (weights != null)
        {
            sumWeights = 0;

            thresholds = new KeyValuePair<T, int>[weights.Length];
            for (int i = 0; i < weights.Length; ++i)
            {
                sumWeights += weights[i].weight;
                thresholds[i] = new KeyValuePair<T, int>(weights[i].data, sumWeights);
            }

            weights = null;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (weights == null)
        {
            weights = new WeightData[thresholds == null ? 0 : thresholds.Length];

            if (weights.Length > 0)
            {
                for (int i = weights.Length - 1; i > 0;)
                    weights[i] = new WeightData(thresholds[i].Key, thresholds[i].Value - thresholds[--i].Value);
                weights[0] = new WeightData(thresholds[0]);

                sumWeights = 0;
            }


            thresholds = null;
        }
    }
}


[Serializable] public class BehaviorSelector : ISerializationCallbackReceiver
{
    [Serializable] public struct WeightData
    {
        public string data;
        public int weight;


        public WeightData(KeyValuePair<string, int> pair) : this(pair.Key, pair.Value)
        {
        }

        public WeightData(string data, int weight)
        {
            this.data = data;
            this.weight = weight;
        }
    }


    [SerializeField] protected int n;
    [SerializeField] protected bool repeats;
    [SerializeField] protected WeightData[] weights;

    protected int sumWeights;
    protected KeyValuePair<string, int>[] thresholds;


    public List<string> Select()
    {
        List<string> result = new List<string>(n);

        if (repeats || n == 1)
        {
            for (int i = 0; i < n; ++i)
                result.Add(Select(UnityEngine.Random.Range(0f, sumWeights)));
        }
        else
            throw new NotImplementedException();


        return result;
    }


    private string Select(float value)
    {
        int l = 0;
        int r = thresholds.Length - 1;

        while (l < r)
        {
            int m = l + (r - l) / 2;
            int w = thresholds[m].Value;

            if (value == w)
                return thresholds[m].Key;
            else if (value < w)
                r = m;
            else
                l = m + 1;
        }


        return thresholds[l].Key;
    }


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (weights != null)
        {
            sumWeights = 0;

            thresholds = new KeyValuePair<string, int>[weights.Length];
            for (int i = 0; i < weights.Length; ++i)
            {
                sumWeights += weights[i].weight;
                thresholds[i] = new KeyValuePair<string, int>(weights[i].data, sumWeights);
            }

            weights = null;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (weights == null)
        {
            weights = new WeightData[thresholds == null ? 0 : thresholds.Length];

            if (weights.Length > 0)
            {
                for (int i = weights.Length - 1; i > 0;)
                    weights[i] = new WeightData(thresholds[i].Key, thresholds[i].Value - thresholds[--i].Value);
                weights[0] = new WeightData(thresholds[0]);

                sumWeights = 0;
            }


            thresholds = null;
        }
    }
}

