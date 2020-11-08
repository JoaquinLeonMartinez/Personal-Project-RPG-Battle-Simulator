using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveCategory { Physical, Special, Other }

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
}
