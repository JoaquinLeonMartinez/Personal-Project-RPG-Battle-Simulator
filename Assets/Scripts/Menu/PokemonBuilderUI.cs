using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBuilderUI : MonoBehaviour
{
    [SerializeField] Image frontSprite;
    [SerializeField] Text nameInput;
    [SerializeField] Text levelInput;


    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        nameInput.text = pokemon.Base.Name;
        levelInput.text = pokemon.Level.ToString();
    }

}
