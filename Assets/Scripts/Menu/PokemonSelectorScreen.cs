using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSelectorScreen : MonoBehaviour
{
    [SerializeField] List<Text> statTextsBase;
    [SerializeField] List<StatBar> statBars;
    [SerializeField] Image frontSprite;
    [SerializeField] Text nameText;
    [SerializeField] Text type1Text;
    [SerializeField] Text type2Text;

    [SerializeField] Pokedex pokedex;
    [SerializeField] List<PokemonSlotLoader> pokemonSlots;

    private void Start()
    {
        SetSlotSelected();
    }
    public void SetData(PokemonBase pokemon)
    {
        frontSprite.sprite = pokemon.FrontSprite;
        nameText.text = pokemon.Name;
        type1Text.text = pokemon.Type1.ToString();
        type2Text.text = pokemon.Type2.ToString();

        statBars[0].SetStatBase(pokemon.Hp);
        statBars[1].SetStatBase(pokemon.Attack);
        statBars[2].SetStatBase(pokemon.Defense);
        statBars[3].SetStatBase(pokemon.SpAttack);
        statBars[4].SetStatBase(pokemon.SpDefense);
        statBars[5].SetStatBase(pokemon.Speed);

        statTextsBase[0].text = pokemon.Hp.ToString();
        statTextsBase[1].text = pokemon.Attack.ToString();
        statTextsBase[2].text = pokemon.Defense.ToString();
        statTextsBase[3].text = pokemon.SpAttack.ToString();
        statTextsBase[4].text = pokemon.SpDefense.ToString();
        statTextsBase[5].text = pokemon.Speed.ToString();

    }

    public void SetSlotsData(int selectedPokemon)
    {
        int pokedexID = selectedPokemon - 4;
        for (int i = 0; i < pokemonSlots.Count; i++)
        {
            pokemonSlots[i].SetData(pokedex.GetPokemonById(pokedexID));
            pokedexID++;
        }

        SetData(pokedex.GetPokemonById(selectedPokemon));//suponemos que este nunca es null (ya que de serlo no podriamos haber llamado a este metodo)
    }

    public int GetPokedexLength()
    {
        return pokedex.GetPokedexLength();
    }

    public void SetSlotSelected()
    {
        pokemonSlots[4].SetSelected(true);
    }

    public PokemonBase GetPokemonById(int pokedexID)
    {
        return pokedex.GetPokemonById(pokedexID);
    }
}
