using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuildMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text nature;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> statTexts;

    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;

    public void setData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        //nature.text = nameof(pokemon.Nature); //Comprobar quee esto funcione

        //Completar movimientos y stats
        SetMoveNames();
        SetStats();
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

    public void SetStats()
    {
        //poner de uno en uno
        statTexts[0].text = $"Stat - Base - EVs - IVs - Total";
        statTexts[1].text = $"Hp - {_pokemon.Base.Hp} - {_pokemon.EV[Stat.Hp]} - {_pokemon.IV[Stat.Hp]} - {_pokemon.Stats[Stat.Hp]}";
        statTexts[2].text = $"Att - {_pokemon.Base.Attack} - {_pokemon.EV[Stat.Attack]} - {_pokemon.IV[Stat.Attack]} - {_pokemon.Stats[Stat.Attack]}";
        statTexts[3].text = $"Def - {_pokemon.Base.Defense} - {_pokemon.EV[Stat.Defense]} - {_pokemon.IV[Stat.Defense]} - {_pokemon.Stats[Stat.Defense]}";
        statTexts[4].text = $"SpAt - {_pokemon.Base.SpAttack} - {_pokemon.EV[Stat.SpAttack]} - {_pokemon.IV[Stat.SpAttack]} - {_pokemon.Stats[Stat.SpAttack]}";
        statTexts[5].text = $"SpDf - {_pokemon.Base.SpDefense} - {_pokemon.EV[Stat.SpDefense]} - {_pokemon.IV[Stat.SpDefense]} - {_pokemon.Stats[Stat.SpDefense]}";
        statTexts[6].text = $"Spe - {_pokemon.Base.Speed} - {_pokemon.EV[Stat.Speed]} - {_pokemon.IV[Stat.Speed]} - {_pokemon.Stats[Stat.Speed]}";

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
