using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int _level;

    public PokemonBase Base { 
        get
            {
                return _base;
            }
    }  //base es una palabra reservada de c# al parecer
    public int Level {
        get
        {
            return _level;
        }
    }
    public int CurrentHP { get; set; }

    public List<Move> Moves { get; set; }

    float criticalHitRate = 6.25f;

    public void Init()
    {

        this.CurrentHP = MaxHP; //no debe ser Hp porque entonces tendria la HP base del pokemon y no la real
        //Por defecto el pokemon tendra los ultimos movimientos que aprende por nivel

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= 4)
            {
                break; //Comprobar si con esto pilla los 4 primeros o los ultimos (todo seria ordenar learnable moves de mayor a menor)
            }
        }

    }

    public Pokemon(PokemonBase _pBase, int _level)
    {
        //this.Base = _pBase; //en principio el constructor ya no se utilizara más, por ello crearemos un metodo Init() que hara lo mismo pero se le llamara desde el inspector
        //this.Level = _level;
        /*this.CurrentHP = MaxHP; //no debe ser Hp porque entonces tendria la HP base del pokemon y no la real

        //Por defecto el pokemon tendra los ultimos movimientos que aprende por nivel

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= 4)
            {
                break; //Comprobar si con esto pilla los 4 primeros o los ultimos (todo seria ordenar learnable moves de mayor a menor)
            }
        }
        */
    }

    //Esto podrian ser metodos normales, pero eso seria demasiaddo facil, lo haremos con el get de la propia variable:
    //TODO: A esto habra que añadir los IVS y los EVs: formula original: https://bulbapedia.bulbagarden.net/wiki/Statistic#:~:text=When%20a%20Pok%C3%A9mon%20grows%20a,individual%20value%20and%20effort%20value.
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100) + 5; } //formula del juego original
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100) + 5; } //formula del juego original
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100) + 5; } //formula del juego original
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100) + 5; } //formula del juego original
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100) + 5; } //formula del juego original
    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.Hp * Level) / 100) + Level + 10; } //formula del juego original
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        //Crtic hit prob
        float critical = 1f;
        if (Random.value * 100f <= criticalHitRate)
        {
            critical = 1.5f;
        }

        //Type efectivenes
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        float modifiers = Random.Range(0.85f, 1f) * type * critical;

        var damageDetails = new DamageDetails()
        {
            Effectiveness = type,
            Critical = critical,
            Fainted = false
        };

        //Determinamos la categoría del movimiento:
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? attacker.SpDefense : attacker.Defense;

        //Formula real:
        float a = ((2 * attacker.Level) / 5) + 2;
        float d = ((a * move.Base.Power * (attack / defense)) / 50f) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        this.CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);  
        return Moves[r]; 
    }

    public void Health(int _HP)
    {
        this.CurrentHP += _HP;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }

    public float Critical { get; set; }

    public float Effectiveness { get; set; }

}


