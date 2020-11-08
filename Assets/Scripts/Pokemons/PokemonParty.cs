using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{

    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        //funcion Linq --> es como un bucle pero en una linea, en este caso devuelve los pokemons con vida (y al poner el FirstOrDefault solo devuelve el primero
        return pokemons.Where(x => x.CurrentHP > 0).FirstOrDefault();
    }

    public void HealthParty()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Health(pokemon.MaxHP);
        }
    }
}
