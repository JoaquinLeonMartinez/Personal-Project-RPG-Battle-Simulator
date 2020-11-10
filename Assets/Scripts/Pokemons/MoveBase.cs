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
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
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

    public int Pp
    {
        get { return pp; }
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
}

[System.Serializable]
public class MoveEffects
{
    //Esta clase esta hecha especificamente para los movimientos de estado, boost, etc
    //declaramos la clase StatBoost como apoyo a esta, ya que podriamos hacer un diccionario en lugar de una lista, pero el diccionario no se muestra en el editor de unity :(
    [SerializeField] List<StatBoost> boosts;

    [SerializeField] ConditionID status;


    public List<StatBoost> Boosts
    {
        get{ return boosts;}
    }

    public ConditionID Status 
    {
        get { return status; }
    } 

}
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}


