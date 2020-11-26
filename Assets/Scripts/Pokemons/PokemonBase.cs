using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [SerializeField] int pokedexId;

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
    public int PokedexId 
    { 
        get { return pokedexId; } 
    }
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
    Hp,
    Accurarcy,
    Evasion
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

public enum PokemonNature
{
    Hardy,
    Bold,
    Modest,
    Calm,
    Timid,
    Lonely,
    Docile,
    Mild,
    Gentle,
    Hasty,
    Adamant,
    Impish,
    Bashful,
    Careful,
    Jolly,
    Naughty,
    Lax,
    Rash,
    Quirky,
    Naive,
    Brave,
    Relaxed,
    Quiet,
    Sassy,
    Serious
}

public class NatureEffect
{
    static float[][] chart =
    {   
        //                    Att  Def  SpAt SpDf Spe
        /*Hardy*/new float[] { 1f , 1f , 1f , 1f , 1f },
        /*Bold*/new float[] { 0.9f , 1.1f , 1f , 1f , 1f },
        /*Modest*/new float[] { 0.9f , 1f , 1.1f , 1f , 1f },
        /*Calm*/new float[] { 0.9f , 1f , 1f , 1.1f , 1f },
        /*Timid*/new float[] { 0.9f , 1f , 1f , 1f , 1.1f },
        /*Lonely*/new float[] { 1.1f , 0.9f , 1f , 1f , 1f },
        /*Docile*/new float[] { 1f , 1f , 1f , 1f , 1f },
        /*Mild*/new float[] { 1f , 0.9f , 1.1f , 1f , 1f },
        /*Gentle*/new float[] { 1f , 0.9f , 1f , 1.1f , 1f },
        /*Hasty*/new float[] { 1f , 0.9f , 1f , 1f , 1.1f },
        /*Adamant*/new float[] { 1.1f , 1f , 0.9f , 1f , 1f },
        /*Impish*/new float[] { 1f , 1.1f , 0.9f , 1f , 1f },
        /*Bashful*/new float[] { 1f , 1f , 1f , 1f , 1f },
        /*Careful*/new float[] { 1f , 1f , 0.9f , 1.1f , 1f },
        /*Jolly*/new float[] { 1f , 1f , 0.9f , 1f , 1.1f },
        /*Naughty*/new float[] { 1.1f , 1f , 1f , 0.9f , 1f },
        /*Lax*/new float[] { 1f , 1.1f , 1f , 0.9f , 1f },
        /*Rash*/new float[] { 1f , 1f , 1.1f , 0.9f , 1f },
        /*Quirky*/new float[] { 1f , 1f , 1f , 1f , 1f },
        /*Naive*/new float[] { 1f , 1f , 1f , 0.9f , 1.1f },
        /*Brave*/new float[] { 1.1f , 1f , 1f , 1f , 0.9f },
        /*Relaxed*/new float[] { 1f , 1.1f , 1f , 1f , 0.9f },
        /*Quiet*/new float[] { 1f , 1f , 1.1f , 1f , 0.9f },
        /*Sassy*/new float[] { 1f , 1f , 1f , 1.1f , 0.9f },
        /*Serious*/new float[] { 1f , 1f , 1f , 1f , 1f },
    };

    public static float GetNatureModifier(PokemonNature pokemonNature, Stat stat)
    {
        int row = (int)pokemonNature;
        int col = (int)stat;

        return chart[row][col];
    }
}