using System.Collections;
using System.Collections.Generic;


public class StatusEffectQueue : IAttributeCollection
{
    private AttributeSet sumAttributes;
    private Dictionary<int, StatusEffect> map;

    //private FibonacciHeap<StatusEffect> pq;
    private List<StatusEffect> list;
    private int current;

    public EventOnAttributeChange OnAttributeChange { get; private set; } = new EventOnAttributeChange();


    public float this[AttributeType type]
    {
        get
        {
            return sumAttributes[type];
        }
    }

    public bool IsEmpty
    {
        get
        {
            return current >= list.Count;
        }
    }

    public StatusEffect Front
    {
        get
        {
            return IsEmpty ? null : list[current];
        }
    }


    public StatusEffectQueue()
    {
        sumAttributes = new AttributeSet();

        map = new Dictionary<int, StatusEffect>();
        list = new List<StatusEffect>();
        current = 0;
    }


    public bool Contains(StatusEffect statusEffect)
    {
        return map.ContainsKey(statusEffect.Data.Id);
    }

    public void Push(StatusEffect statusEffect)
    {
        int id = statusEffect.Data.Id;


        switch (statusEffect.Data.DurationStackType)
        {
            case StatusEffectDurationStackType.Seperate:
                throw new System.NotImplementedException();


            default:
                if (map.ContainsKey(id))
                {
                    StatusEffect existedStatusEffect = map[id];

                    float t = existedStatusEffect.EndTime;


                    sumAttributes.Modify(existedStatusEffect.Data.Attributes, OnAttributeChange, existedStatusEffect.Stack(statusEffect));


                    if (t != existedStatusEffect.EndTime)
                    {
                        list.Remove(existedStatusEffect);
                        Insert(existedStatusEffect, current, list.Count - 1);
                    }
                }
                else
                {
                    if (IsEmpty)
                        list.Add(statusEffect);
                    else
                        Insert(statusEffect, current, list.Count - 1);


                    map.Add(id, statusEffect);

                    foreach (KeyValuePair<AttributeType, float> attribute in statusEffect)
                        sumAttributes.Modify(attribute.Key, attribute.Value, OnAttributeChange);
                }
                break;
        }
    }

    public StatusEffect Pop()
    {
        if (IsEmpty)
            return null;


        StatusEffect statusEffect = list[current++];

        map.Remove(statusEffect.Data.Id);


        foreach (KeyValuePair<AttributeType, float> attribute in statusEffect)
            sumAttributes.Modify(attribute.Key, -attribute.Value, OnAttributeChange);


        return statusEffect;
    }

    public StatusEffect Remove(int id)
    {
        StatusEffect statusEffect = null;


        if (map.ContainsKey(id))
        {
            statusEffect = map[id];

            int i = list.IndexOf(statusEffect);
            list.RemoveAt(i);

            if (i < current)
                current--;

            map.Remove(id);

            foreach (KeyValuePair<AttributeType, float> attribute in statusEffect)
                sumAttributes.Modify(attribute.Key, -attribute.Value);
        }


        return statusEffect;
    }


    public override string ToString()
    {
        string s = "";

        for (int i = 0; i < list.Count; i++)
            s += (i == current ? "->" : "  ") + list[i].ToString() + "\n";

        return s;
    }


    private void Insert(StatusEffect statusEffect, int start, int end)
    {
        if (start == end)
        {
            list.Insert(statusEffect.CompareTo(list[start]) < 0 ? start : start + 1, statusEffect);
            return;
        }

        int mid = (start + end + 1) / 2;

        int compare = statusEffect.CompareTo(list[mid]);
        if (compare < 0)
            Insert(statusEffect, start, mid - 1);
        else
            Insert(statusEffect, mid, end);
    }


    public IEnumerator<KeyValuePair<AttributeType, float>> GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }
}
