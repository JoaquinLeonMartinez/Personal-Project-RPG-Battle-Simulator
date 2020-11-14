using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveCategory { Physical, Special, Status }
public enum MoveTarget { Foe, Self }

[CreateAssetMenu(fileName ="Move", menuName ="Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accurarcy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accurarcy
    {
        get { return accurarcy; }
    }

    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }

    public int Pp
    {
        get { return pp; }
    }

    public int Priority
    {
        get { return priority; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }

    public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }
}

[System.Serializable]
public class MoveEffects
{
    //Esta clase esta hecha especificamente para los movimientos de estado, boost, etc
    //declaramos la clase StatBoost como apoyo a esta, ya que podriamos hacer un diccionario en lugar de una lista, pero el diccionario no se muestra en el editor de unity :(
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;


    public List<StatBoost> Boosts
    {
        get{ return boosts;}
    }

    public ConditionID Status 
    {
        get { return status; }
    }

    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }

}
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;//probabilidad de que se de este efecto secundario
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }

}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}


