using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuildMemberUI : MonoBehaviour
{
    [SerializeField] Image spritePokemon;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text nature;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] StatsBuildPanel stats;

    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;

    public void setData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        spritePokemon.sprite = pokemon.Base.FrontSprite;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        nature.text = pokemon.Nature.ToString(); //Comprobar quee esto funcione

        //Completar movimientos y stats
        SetMoveNames();
        stats.SetData(pokemon);
    }

    public void SetMoveNames()
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < _pokemon.Moves.Count)
            {
                moveTexts[i].text = _pokemon.Moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
