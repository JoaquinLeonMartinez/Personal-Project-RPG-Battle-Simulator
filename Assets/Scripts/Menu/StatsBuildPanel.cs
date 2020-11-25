using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBuildPanel : MonoBehaviour
{

    [SerializeField] List<Text> statTextsBase;
    [SerializeField] List<Text> statTextsEVs;
    [SerializeField] List<Text> statTextsIvs;
    [SerializeField] List<Text> statTextsTotal;
    [SerializeField] List<StatBar> statBars;

    //TODO: Estos colores deberian se constantes
    [SerializeField] Color highStatColor;
    [SerializeField] Color lowStatColor;

    Dictionary<int, Stat> dictionary;

    public void SetData(Pokemon pokemon)
    {
        dictionary = new Dictionary<int, Stat>();
        dictionary.Add(0, Stat.Hp);
        dictionary.Add(1, Stat.Attack);
        dictionary.Add(2, Stat.Defense);
        dictionary.Add(3, Stat.SpAttack);
        dictionary.Add(4, Stat.SpDefense);
        dictionary.Add(5, Stat.Speed);

        for (int i = 0; i < statTextsEVs.Count; i++)
        {
            statTextsEVs[i].text = pokemon.EV[dictionary[i]].ToString();
            statTextsIvs[i].text = pokemon.IV[dictionary[i]].ToString();
            statTextsTotal[i].text = pokemon.Stats[dictionary[i]].ToString();
            statBars[i].SetStat(pokemon.Stats[dictionary[i]]);
        }

        statTextsBase[0].text = pokemon.Base.Hp.ToString();
        statTextsBase[1].text = pokemon.Base.Attack.ToString();
        statTextsBase[2].text = pokemon.Base.Defense.ToString();
        statTextsBase[3].text = pokemon.Base.SpAttack.ToString();
        statTextsBase[4].text = pokemon.Base.SpDefense.ToString();
        statTextsBase[5].text = pokemon.Base.Speed.ToString();

        setStatColor(pokemon);
    }

    public void setStatColor(Pokemon pokemon)
    {
        for (int i = 1; i < statTextsTotal.Count; i++)
        {
            if (NatureEffect.GetNatureModifier(pokemon.Nature, dictionary[i]) == 1.1f)
            {
                statTextsTotal[i].color = highStatColor;
            }
            else if (NatureEffect.GetNatureModifier(pokemon.Nature, dictionary[i]) == 0.9f)
            {
                statTextsTotal[i].color = lowStatColor;
            }
            else
            {
                statTextsTotal[i].color = Color.black;
            }
        }
    }
}
