using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBuilderUI : MonoBehaviour
{
    [SerializeField] Image frontSprite;

    //[SerializeField] Text nameText;
    //[SerializeField] Text levelText;
    //[SerializeField] Text natureText;
    //[SerializeField] List<Text> moveTexts;
    //[SerializeField] List<Text> statTexts;
    [SerializeField] Text nameInput;
    [SerializeField] Text levelInput;
    [SerializeField] Text natureInput;

    public void Init()
    {

    }

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        //nameText.text = pokemon.Base.Name;
        //levelText.text = pokemon.Level.ToString(); ;
        //natureText.text = pokemon.Nature.ToString();
        //levelText.text = $"Lvl {pokemon.Level}";
        nameInput.text = pokemon.Base.Name;
        levelInput.text = pokemon.Level.ToString();
        natureInput.text = pokemon.Nature.ToString();
    }

}
