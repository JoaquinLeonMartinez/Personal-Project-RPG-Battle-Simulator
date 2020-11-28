using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokedex : MonoBehaviour
{
    [SerializeField] List<PokemonBase> pokemons;

    public PokemonBase GetPokemonById(int id)
    {
        if (pokemons.Count > id && id >= 0)
        {
            //Debug.Log($"El id es {id} por lo que devolvemos el pokemon correspondiente: {pokemons[id].Name}");
            return pokemons[id];
        }
        else
        {
            //Debug.Log($"El id es {id} por lo que devolvemos null");
            return null;
        }
        
    }

    public int GetPokedexLength()
    {
        return pokemons.Count;
    }

}
