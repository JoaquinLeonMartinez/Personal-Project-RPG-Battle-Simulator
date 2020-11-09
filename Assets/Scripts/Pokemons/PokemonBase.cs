using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base Stats
    [SerializeField] int hp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    //En lugar de hacer getters y setters creamos variables cpara obtener directamente los valores de forma publica
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int Hp
    {
        get { return hp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }
    public int  Level
    {
        get { return level; }
    }
}

public enum PokemonType
{
    None,
    Water,
    Fire,
    Ice,
    Electric,
    Normal,
    Grass,
    Poison,
    Dragon,
    Fighting,
    Bug,
    Rock,
    Psychic,
    Ground,
    Ghost,
    Fairy,
    Flying,
    Steel,
    Dark
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accurarcy
}

public class TypeChart
{
    static float[][] chart =
    {   
        //                    NON  WAT  FIR  ICE  ELE  NOR  GRA  POI  DRA  FIG  BUG  ROC  PSY  GRO  GHO  FAI  FLY  STE   DAR
        /*NONE*/new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
        /*WAT*/ new float[] { 1f ,0.5f, 2f , 1f , 1f , 1f ,0.5f, 1f ,0.5f, 1f , 1f , 2f , 1f , 2f , 1f , 1f , 1f , 1f , 1f },
        /*FIR*/ new float[] { 1f ,0.5f,0.5f, 2f , 1f , 1f , 2f , 1f ,0.5f, 1f , 2f ,0.5f, 1f , 1f , 1f , 1f , 1f , 2f , 1f },
        /*ICE*/ new float[] { 1f ,0.5f,0.5f,0.5f, 1f , 1f , 2f , 1f , 2f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 2f ,0.5f, 1f },
        /*ELE*/ new float[] { 1f , 2f , 1f , 1f ,0.5f, 1f ,0.5f, 1f ,0.5f, 1f , 1f , 1f , 1f , 0f , 1f , 1f , 2f , 1f , 1f },
        /*NOR*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 0f , 1f , 1f ,0.5f, 1f },
        /*GRA*/ new float[] { 1f , 2f ,0.5f, 1f , 1f , 1f ,0.5f,0.5f,0.5f, 1f ,0.5f, 2f , 1f , 2f , 1f , 1f ,0.5f,0.5f, 1f },
        /*POI*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 2f ,0.5f, 1f , 1f , 1f ,0.5f, 1f ,0.5f,0.5f, 2f , 1f , 0f , 1f },
        /*DRA*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f , 1f , 1f , 1f , 0f , 1f ,0.5f, 1f },
        /*FIG*/ new float[] { 1f , 1f , 1f , 2f , 1f , 2f , 1f ,0.5f, 1f , 1f ,0.5f, 2f ,0.5f, 1f , 0f ,0.5f,0.5f, 2f , 2f },
        /*BUG*/ new float[] { 1f , 1f ,0.5f, 1f , 1f , 1f , 2f ,0.5f, 1f ,0.5f, 1f , 1f , 2f , 1f ,0.5f,0.5f,0.5f,0.5f, 2f },
        /*ROC*/ new float[] { 1f , 1f , 2f , 2f , 1f , 1f , 1f , 1f , 1f ,0.5f, 2f , 1f , 1f ,0.5f, 1f , 1f , 2f ,0.5f, 1f },
        /*PSY*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 2f , 1f , 1f ,0.5f, 1f , 1f , 1f , 1f ,0.5f, 0f },
        /*GRO*/ new float[] { 1f , 1f , 2f , 1f , 2f , 1f ,0.5f, 2f , 1f , 1f ,0.5f, 2f , 1f , 1f , 1f , 1f , 0f , 2f , 1f },
        /*GHO*/ new float[] { 1f , 1f , 1f , 1f , 1f , 0f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 2f , 1f , 1f , 1f ,0.5f},
        /*FAI*/ new float[] { 1f , 1f ,0.5f, 1f , 1f , 1f , 1f ,0.5f, 2f , 2f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 2f },
        /*FLY*/ new float[] { 1f , 1f , 1f , 1f ,0.5f, 1f , 2f , 1f , 1f , 2f , 2f ,0.5f, 1f , 1f, 1f , 1f , 1f ,0.5f, 1f },
        /*STE*/ new float[] { 1f ,0.5f,0.5f, 2f ,0.5f, 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f },
        /*DAR*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 2f , 1f , 2f ,0.5f, 1f, 1f, 0.5f }
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None) //al haber añadido el none a la tabla esto no es necesario
        {
            return 1; 
        }

        int row = (int)attackType;
        int col = (int)defenseType;

        return chart[row][col];

    }
}

