using System;
using System.Collections.Generic;
using UnityEngine;
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
    
    MaxUltimateEnergy_c0 = 0x800,
    
    FeverDecay_c0 = 0x900,
    
    SightRange_c0 = 0xA00,


    Damage_c0 = 0x1000,

    Knockback_c0 = 0x2000,


    MaxFatigue_c0 = 0xE000,
    MaxFatigue_c1 = 0xE001,
    MaxFatigue_c2 = 0xE002,
    MaxFatigue_m0 = 0xE020,
    
    MaxKs_c0 = 0xA110,
    KsDecay_c0 = 0xA210, // Kill Streak Decay

    PDCoolDown_c0 = 0xB100, // PowerDash CoolDown

    OspReward_c0 = 0xF210,

    FeverReward_c0 = 0xF310,

    Fatigue_p0 = 0xF410,
    Fatigue_p1 = 0xF411,
}


public enum StatisticType : int
{
    WalkSpeed = 0x1,
    JumpPower = 0x2,
    MaxHp = 0x3,
    MaxSp = 0x4,
    MaxOsp = 0x6,
    OspRecovery = 0x7,
    MaxUltimateEnergy = 0x8,
    FeverDecay = 0x9,
    SightRange = 0xA,

    Damage = 0x10,
    Knowback = 0x20,
    
    KsDecay = 0xA1,
    KsCount = 0xA2,
    MaxKs = 0xA3,
    
    PDCoolDown = 0xB1,
    
    MaxFatigue = 0xE0,

    Hp = 0xF0,
    Sp = 0xF1,
    Osp = 0xF2,
    UltimateEnergy = 0xF3,
    Fatigue = 0xF4,
}


public struct StatisticModificationResult
{
    public readonly float previousValue;
    public readonly float currentValue;


    public StatisticModificationResult(float previousValue, float currentValue)
    {
        this.previousValue = previousValue;
        this.currentValue = currentValue;
    }
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


            case StatisticType.MaxSp:
                return AttributeSet.Sum(AttributeType.MaxSp_c0, attributeSets);


            case StatisticType.MaxOsp:
                return AttributeSet.Sum(AttributeType.MaxOsp_c0, attributeSets);


            case StatisticType.MaxUltimateEnergy:
                return AttributeSet.Sum(AttributeType.MaxUltimateEnergy_c0, attributeSets);


            case StatisticType.FeverDecay:
                return AttributeSet.Sum(AttributeType.FeverDecay_c0, attributeSets);


            case StatisticType.SightRange:
                return AttributeSet.Sum(AttributeType.SightRange_c0, attributeSets);


            case StatisticType.MaxFatigue:
                return AttributeSet.Sum(AttributeType.MaxFatigue_c0, attributeSets) + AttributeSet.Sum(AttributeType.MaxFatigue_m0, attributeSets) * AttributeSet.Sum(AttributeType.MaxFatigue_c1, attributeSets);

            
            case StatisticType.PDCoolDown:
                return AttributeSet.Sum(AttributeType.PDCoolDown_c0, attributeSets);

            
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
            attributeSet.OnAttributeChange.AddListener(UpdateChangedStatistics);

        UpdateChangedStatistics(this.attributeSets);
    }

    ~StatisticSystem()
    {
        foreach (IAttributeCollection attributeSet in attributeSets)
            if (attributeSet != null && attributeSet.OnAttributeChange != null)
                attributeSet.OnAttributeChange.RemoveListener(UpdateChangedStatistics);
    }


    public StatisticModificationResult Modify(StatisticType statistic, float value)
    {
        float previousValue = this[statistic];


        this[statistic] += value;


        return new StatisticModificationResult(previousValue, this[statistic]);
    }

    public StatisticModificationResult Modify(StatisticType statistic, float value, float min)
    {
        float previousValue = this[statistic];


        this[statistic] = Mathf.Max(min, this[statistic] + value);


        return new StatisticModificationResult(previousValue, this[statistic]);
    }

    public StatisticModificationResult Modify(StatisticType statistic, float value, float min, float max)
    {
        float previousValue = this[statistic];


        this[statistic] = Mathf.Clamp(this[statistic] + value, min, max);


        return new StatisticModificationResult(previousValue, this[statistic]);
    }


    public float Sum(AttributeType type)
    {
        return AttributeSet.Sum(type, attributeSets);
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
