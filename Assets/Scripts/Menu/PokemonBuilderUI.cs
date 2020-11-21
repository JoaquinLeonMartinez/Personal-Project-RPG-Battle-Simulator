using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBuilderUI : MonoBehaviour
{
    [SerializeField] Image frontSprite;
    [SerializeField] Text nameText;
    //[SerializeField] Text levelText;
    //[SerializeField] Text nature;
    //[SerializeField] List<Text> moveTexts;
    //[SerializeField] List<Text> statTexts;

    public void Init()
    {

    }

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        nameText.text = pokemon.Base.Name;
        //levelText.text = $"Lvl {pokemon.Level}";
    }
}
