using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyDebug
{
    public static void  ShowPokemonState(Pokemon pokemon)
    {
        Debug.Log($"{pokemon.Base.name} info: ");
        Debug.Log($"HP - current: {pokemon.CurrentHP} / original : {pokemon.MaxHP} ");
        Debug.Log($"Attack - current: {pokemon.GetStat(Stat.Attack)} / original : {pokemon.Attack} ");
        Debug.Log($"Defense - current: {pokemon.GetStat(Stat.Defense)} / original : {pokemon.Defense} ");
        Debug.Log($"SpAttack - current: {pokemon.GetStat(Stat.SpAttack)} / original : {pokemon.SpAttack} ");
        Debug.Log($"SpDefense - current: {pokemon.GetStat(Stat.SpDefense)} / original : {pokemon.SpDefense} ");
        Debug.Log($"Speed - current: {pokemon.GetStat(Stat.Speed)} / original : {pokemon.Speed} ");

        if (pokemon.Status != null)
        {
            Debug.Log($"Status: {pokemon.Status.Name}");
        }
        else
        {
            Debug.Log($"Status: none");
        }

        if (pokemon.VolatileStatus != null)
        {
            Debug.Log($"VolatileStatus: {pokemon.VolatileStatus.Name}");
        }
        else
        {
            Debug.Log($"VolatileStatus: none");
        }
    }

    public static void ShowTurnDetails()
    {
        //TODO
    }
}

