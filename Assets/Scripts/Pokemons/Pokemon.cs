using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public Move CurrentMove { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatBoosts { get; private set; } //el integer valdra entre -6 y 6 debido a que es lo maximo que se puede boostear una caracteristica

    public Condition Status { get; private set; }

    public int StatusTime { get; set; } //numero de turnos que un pokemon tendra un estado alterado, por ejemplo dormido

    public Condition VolatileStatus { get; private set; }

    public int VolatileStatusTime { get; set; } //numero de turnos que un pokemon tendra un estado alterado, por ejemplo dormido

    public Queue<string> StatusChanges { get; private set; }

    public bool HpChanged { get; set; }

    float criticalHitRate = 6.25f;

    public event System.Action OnStatusChanged;

    public void Init()
    {
        //Por defecto el pokemon tendra los ultimos movimientos que aprende por nivel

        Moves = new List<Move>();
        StatusChanges = new Queue<string>();

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

        CalculateStats();
        this.CurrentHP = MaxHP;
        ResetStateBoosts();
        Status = null;
        VolatileStatus = null;

    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return; //si ya tiene un estado alterado no puede poner otro
        //TODO: Aqui habria que añadir un mensaje no?

        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return; //si ya tiene un estado alterado no puede poner otro
        //TODO: tal y como esta hecho ahora no puede tener mas de un status volatile, esto no es correcto

        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void ResetVolatileStatus()
    {
        VolatileStatus = null;
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }


    public void ResetStateBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accurarcy, 0},
            {Stat.Evasion, 0}
        };
    }

    public void CalculateStats()
    {
        //TODO: A esto habra que añadir los IVS y los EVs: formula original: https://bulbapedia.bulbagarden.net/wiki/Statistic#:~:text=When%20a%20Pok%C3%A9mon%20grows%20a,individual%20value%20and%20effort%20value.
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * 2 * Level) / 100) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * 2 * Level) / 100) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * 2 * Level) / 100) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * 2 * Level) / 100) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * 2 * Level) / 100) + 5);
        Stats.Add(Stat.Accurarcy, 1);

        MaxHP = Mathf.FloorToInt((Base.Hp * 2 * Level) / 100) + Level + 10;
    }

    public int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //TODO: Apply boosts and drops
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }


        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)//esta funcion se utiliza cuando recibes un ataque que baja o sube las estadisticas, el paramtro es la estadistica a modificar
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if(boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}`s {stat} rose!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}`s {stat} fell!");
            }

            //Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); } 
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); } 
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); } 
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); } 
    }

    public int MaxHP { get; private set;}

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

        UpdateHP(damage);
        return damageDetails;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;

        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this); //.? indica que solo se llamara a OnAfterTurn cuando no sea null
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, movesWithPP.Count);  
        return movesWithPP[r]; 
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStateBoosts();
    }

    public void UpdateHP(int damage)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, MaxHP);
        HpChanged = true;
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


