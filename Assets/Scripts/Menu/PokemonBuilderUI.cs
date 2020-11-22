using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonBuilderUI : MonoBehaviour
{
    [SerializeField] Image frontSprite;
    [SerializeField] Text nameInput;
    [SerializeField] Text levelInput;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Text type1;
    [SerializeField] Text type2;

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        nameInput.text = pokemon.Base.Name;
        levelInput.text = pokemon.Level.ToString();

        type1.text = pokemon.Base.Type1.ToString();
        if (pokemon.Base.Type2 != PokemonType.None)
        {
            type2.text = pokemon.Base.Type2.ToString();
            type2.gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            type2.gameObject.transform.parent.gameObject.SetActive(false);
        }

        SetMoveNames(pokemon);
    }

    public void SetMoveNames(Pokemon pokemon)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < pokemon.Moves.Count)
            {
                moveTexts[i].text = pokemon.Moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

}
