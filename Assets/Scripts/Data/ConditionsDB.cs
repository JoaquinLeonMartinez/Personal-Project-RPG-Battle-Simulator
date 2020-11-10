using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{

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
        }

    };

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
