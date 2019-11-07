using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatusEffectDurationStackType
{
    Min,
    Seperate,
    Max,
}


[Serializable] public class StatusEffectData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private int maxNumStacks;
    [SerializeField] private StatusEffectDurationStackType durationStackType;
    [SerializeField] private AttributeSet attributes;


    public override int Index
    {
        get
        {
            return id;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public int MaxNumStacks
    {
        get
        {
            return maxNumStacks;
        }
    }

    public StatusEffectDurationStackType DurationStackType
    {
        get
        {
            return durationStackType;
        }
    }

    public AttributeSet Attributes
    {
        get
        {
            return attributes;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}


public class StatusEffect : IAttributeCollection, IComparable
{
    public StatusEffectData Data { get; internal set; }

    public int NumStacks { get; internal set; }

    public float EndTime { get; internal set; }
    

    public EventOnAttributeChange OnAttributeChange { get; internal set; } = new EventOnAttributeChange();

    //public float this[int id]
    //{
    //    get
    //    {
    //        return attributes[id] * NumStacks;
    //    }
    //}

    public float this[AttributeType type]
    {
        get
        {
            return Data.Attributes[type] * NumStacks;
        }
    }

    private StatusEffect()
    {
    }

    public StatusEffect(StatusEffectData data, float duration, int numStacks = 1)
    {
        Data = data;
        NumStacks = numStacks;
        EndTime = Time.time + duration;
    }


    public bool ReachMaxNumStacks()
    {
        return NumStacks == Data.MaxNumStacks;
    }


    public int Stack(StatusEffect other)
    {
        int n = NumStacks;


        if (n != Data.MaxNumStacks)
            NumStacks = Math.Min(NumStacks + other.NumStacks, Data.MaxNumStacks);


        switch (Data.DurationStackType)
        {
            case StatusEffectDurationStackType.Min:
                EndTime = Mathf.Min(EndTime, other.EndTime);
                break;


            case StatusEffectDurationStackType.Max:
                EndTime = Mathf.Max(EndTime, other.EndTime);
                break;
        }


        return NumStacks - n;
    }


    public int CompareTo(StatusEffect other)
    {
        return EndTime.CompareTo(other.EndTime);
    }

    public int CompareTo(object obj)
    {
        return CompareTo((StatusEffect)obj);
    }


    public IEnumerator<KeyValuePair<AttributeType, float>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }


    public class Enumerator : IEnumerator<KeyValuePair<AttributeType, float>>
    {
        private int numStacks;
        private IEnumerator<KeyValuePair<AttributeType, float>> attributeSetEnumerator;


        public Enumerator(StatusEffect statusEffect)
        {
            numStacks = statusEffect.NumStacks;
            attributeSetEnumerator = statusEffect.Data.Attributes.GetEnumerator();
        }


        public KeyValuePair<AttributeType, float> Current
        {
            get
            {
                return new KeyValuePair<AttributeType, float>(attributeSetEnumerator.Current.Key, attributeSetEnumerator.Current.Value * numStacks);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }


        public void Dispose() {}

        public bool MoveNext()
        {
            return attributeSetEnumerator.MoveNext();
        }

        public void Reset()
        {
            attributeSetEnumerator.Reset();
        }
    }
}
