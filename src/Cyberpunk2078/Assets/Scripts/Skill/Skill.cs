using System;


public class Skill
{
    public int Id { get; private set; }
    public int Level { get; private set; }


    public virtual void Activate(Dummy caster, params Dummy[] targets) { }
}


public class ActiveSkill : Skill
{
}


public class PassiveSkill : Skill
{
}
