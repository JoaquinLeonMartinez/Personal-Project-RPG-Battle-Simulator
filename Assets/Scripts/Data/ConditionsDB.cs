using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                //vamos a hacer esto con una "landa" funcion:
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    //Efecto del veneno
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                //vamos a hacer esto con una "landa" funcion:
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    //Efecto de la quemadura
                    pokemon.UpdateHP(pokemon.MaxHP/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                //vamos a hacer esto con una "landa" funcion:
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)//tenemos un 25% de no atacar (1/4)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}`s paralyzed and can´t move");
                        return false;
                    }
                    return true;//esto significa que atacara
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                //vamos a hacer esto con una "landa" funcion:
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)//tenemos un 25% de descongelarnos
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}`s is not frozen anymore");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}`s is frozen, will not attack");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                //vamos a hacer esto con una "landa" funcion:
                OnStart = (Pokemon pokemon) =>
                {
                    //Sleep for 1-3 turns
                    pokemon.StatusTime=Random.Range(1,4);
                    Debug.Log($"Va a estar dormido durante {pokemon.StatusTime} turnos");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    { 
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        pokemon.CureStatus();
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false;
                }
            }
        },
        //Volatile Status Conditions:
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused",
                //vamos a hacer esto con una "landa" funcion:
                OnStart = (Pokemon pokemon) =>
                {
                    //Sleep for 1-3 turns
                    pokemon.VolatileStatusTime=Random.Range(1,5);
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not confuse anymore!");
                        pokemon.CureVolatileStatus();
                        return true;
                    }

                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                    //Probabilidad de no atacar
                    if (Random.Range(1,3) == 1) //50% de atacar
                    {
                        return true;
                    }
                    pokemon.UpdateHP(pokemon.MaxHP / 8);
                    pokemon.StatusChanges.Enqueue($"It hurt itself due to confusion");
                    return false;
                }
            }
        }

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion//todo: add here other volatile status like love or flinch
}
