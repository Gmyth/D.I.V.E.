using System;
using System.Collections.Generic;
using UnityEngine.Events;


public enum AttributeType : int
{
    WalkSpeed_c0 = 0x100,

    JumpPower_c0 = 0x200,

    MaxHp_c0 = 0x300,

    MaxSp_c0 = 0x400,

    SpRecovery_c0 = 0x500,

    MaxOsp_c0 = 0x600,

    OspRecovery_c0 = 0x700,
    
    MaxHsp_c0 = 0x800,

    SightRange_c0 = 0xA00,

    Damage_c0 = 0x1000,


    Knowback_c0 = 0x2000,

    Sp_c0 = 0xF110,

    Osp_c0 = 0xF210,
    Hsp_c0 = 0xF310
}


public enum StatisticType : int
{
    WalkSpeed = 0x1,
    JumpPower = 0x2,
    MaxHp = 0x3,
    MaxSp = 0x4,
    SpRecovery_c0 = 0x5,
    MaxOsp = 0x6,
    OspRecovery = 0x7,
    SightRange = 0xA,

    Damage = 0x10,
    Knowback = 0x20,

    Hp = 0xF0,
    Sp = 0xF1,
    Osp = 0xF2,
}


public class EventOnStatisticChange : UnityEvent<StatisticType, float, float> { }


public class StatisticSystem
{
    public static float Calculate(StatisticType type, params IAttributeCollection[] attributeSets)
    {
        switch (type)
        {
            case StatisticType.WalkSpeed:
                return AttributeSet.Sum(AttributeType.WalkSpeed_c0, attributeSets);


            case StatisticType.JumpPower:
                return AttributeSet.Sum(AttributeType.JumpPower_c0, attributeSets);


            case StatisticType.MaxHp:
                return AttributeSet.Sum(AttributeType.MaxHp_c0, attributeSets);


            case StatisticType.SightRange:
                return AttributeSet.Sum(AttributeType.SightRange_c0, attributeSets);


            default:
                return 0;
        }
    }

    public static Dictionary<StatisticType, float> CalculateAll(params IAttributeCollection[] attributeSets)
    {
        HashSet<int> ids = new HashSet<int>();
        foreach (IAttributeCollection attributes in attributeSets)
            foreach (KeyValuePair<AttributeType, float> attribute in attributes)
                ids.Add((int)attribute.Key >> 8);

        Dictionary<StatisticType, float> statistics = new Dictionary<StatisticType, float>();

        foreach (int id in ids)
        {
            StatisticType statistic = (StatisticType)id;
            statistics.Add(statistic, Calculate(statistic, attributeSets));
        }

        return statistics;
    }


    /// <summary>
    /// An event triggered whenever a certain statistic in this system changes 
    /// </summary>
    public readonly EventOnStatisticChange onStatisticChange = new EventOnStatisticChange();

    //public EventOnDataChange3<StatusEffect> onStatusEffectChange = new EventOnDataChange3<StatusEffect>(); 

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<StatisticType, float> statistics = new Dictionary<StatisticType, float>();

    /// <summary>
    /// Related attribute sets
    /// </summary>
    private IAttributeCollection[] attributeSets;

    ///// <summary>
    ///// All status effects applied to this system in time order
    ///// </summary>
    //private StatusEffectQueue statusEffects = new StatusEffectQueue();

    public float this[StatisticType type]
    {
        get
        {
            return statistics.ContainsKey(type) ? statistics[type] : 0;
        }

        set
        {
            if (!statistics.ContainsKey(type))
            {
                if (value != 0)
                {
                    statistics.Add(type, value);

                    onStatisticChange.Invoke(type, 0, value);
                }
            }
            else
            {
                float previousValue = statistics[type];

                if (value != previousValue)
                {
                    statistics[type] = value;

                    onStatisticChange.Invoke(type, previousValue, value);
                }
            }
        }
    }


    private StatisticSystem()
    {
    }

    public StatisticSystem(params IAttributeCollection[] attributeSets)
    {
        this.attributeSets = attributeSets;

        foreach (IAttributeCollection attributeSet in attributeSets)
            if (attributeSet.OnAttributeChange != null)
                attributeSet.OnAttributeChange.AddListener(UpdateChangedStatistics);

        UpdateChangedStatistics(this.attributeSets);
    }

    ~StatisticSystem()
    {
        foreach (IAttributeCollection attributeSet in attributeSets)
            if (attributeSet != null && attributeSet.OnAttributeChange != null)
                attributeSet.OnAttributeChange.RemoveListener(UpdateChangedStatistics);
    }


    public float Sum(AttributeType type)
    {
        return AttributeSet.Sum(type, attributeSets); //, statusEffects);
    }

    //    public bool AddStatusEffect(StatusEffect statusEffect)
    //    {
    //        bool isExisted = statusEffects.Contains(statusEffect);

    //        if (statusEffects.Push(statusEffect))
    //        {
    //#if UNITY_EDITOR
    //            Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Add " + statusEffect + "\n" + ToString()));
    //#endif

    //            UpdateChangedStatistics(statusEffect);
    //            onStatusEffectChange.Invoke(isExisted ? 0 : 1, statusEffect);

    //            return true;
    //        }

    //        return false;
    //    }

    //    public StatusEffect RemoveStatusEffect(int id)
    //    {
    //        StatusEffect statusEffect = statusEffects.Remove(id);

    //        if (statusEffect != null)
    //        {
    //#if UNITY_EDITOR
    //            Debug.Log(LogUtility.MakeLogString("StatisticSystem", "Remove " + statusEffect + "\n" + ToString()));
    //#endif

    //            UpdateChangedStatistics(statusEffect);
    //            onStatusEffectChange.Invoke(-1, statusEffect);
    //        }

    //        return statusEffect;
    //    }

    public float CalculateModified(StatisticType type, params IAttributeCollection[] modifiers)
    {
        IAttributeCollection[] args = new IAttributeCollection[attributeSets.Length + modifiers.Length];
        Array.Copy(attributeSets, args, attributeSets.Length);
        Array.Copy(modifiers, 0, args, attributeSets.Length, modifiers.Length);

        return Calculate(type, args);
    }

    public Dictionary<StatisticType, float> CalculateAllModified(params IAttributeCollection[] modifiers)
    {
        IAttributeCollection[] args = new IAttributeCollection[attributeSets.Length + modifiers.Length];
        Array.Copy(attributeSets, args, attributeSets.Length);
        Array.Copy(modifiers, 0, args, attributeSets.Length, modifiers.Length);

        return CalculateAll(args);
    }


    public override string ToString()
    {
        string s = "";

        foreach (KeyValuePair<StatisticType, float> statistic in statistics)
            s += ";" + statistic.Key + ":" + statistic.Value;

        return string.Format("Stat: {0}\nTalent: {1}\n\n", s.Substring(1), attributeSets); //, statusEffects);
    }


    private void UpdateChangedStatistics(AttributeType attributeType, float previousValue, float currentValue)
    {
        StatisticType statisticType = (StatisticType)((int)attributeType >> 8);
        this[statisticType] = Calculate(statisticType, attributeSets);
    }

    private void UpdateChangedStatistics(params IAttributeCollection[] attributeSets)
    {
        HashSet<int> changedStatistics = new HashSet<int>();
        foreach (IAttributeCollection attributeSet in attributeSets)
            foreach (KeyValuePair<AttributeType, float> attribute in attributeSet)
                changedStatistics.Add((int)attribute.Key >> 8);

        foreach (int id in changedStatistics)
        {
            StatisticType statisticType = (StatisticType)id;
            this[statisticType] = Calculate(statisticType, attributeSets);
        }
    }

    //private void UpdateChangedStatistics(List<IAttributeCollection> attributeSets)
    //{
    //    HashSet<int> changedStatistics = new HashSet<int>();
    //    foreach (IAttributeCollection attributes in attributeSets)
    //        foreach (KeyValuePair<AttributeType, float> attribute in attributes)
    //            changedStatistics.Add((int)attribute.Key >> 8);

    //    foreach (int id in changedStatistics)
    //    {
    //        StatisticType statistic = (StatisticType)id;
    //        this[statistic] = CalculateStatistic(statistic);
    //    }
    //}

    private void UpdateChangedStatistics(List<StatusEffect> statusEffects)
    {
        HashSet<int> changedStatistics = new HashSet<int>();
        foreach (StatusEffect statusEffect in statusEffects)
            foreach (KeyValuePair<AttributeType, float> attribute in statusEffect)
                changedStatistics.Add((int)attribute.Key >> 8);

        foreach (int id in changedStatistics)
        {
            StatisticType statisticType = (StatisticType)id;
            this[statisticType] = Calculate(statisticType, attributeSets);
        }
    }
}
