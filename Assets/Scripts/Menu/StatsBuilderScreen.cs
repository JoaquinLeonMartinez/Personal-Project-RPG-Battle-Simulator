using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBuilderScreen : MonoBehaviour
{
    [SerializeField] Image frontSprite;

    [SerializeField] List<Text> statTextsBase;
    [SerializeField] List<Text> statTextsEVs;
    [SerializeField] List<Text> statTextsIvs;
    [SerializeField] List<Text> statTextsTotal;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text abilityText;
    [SerializeField] Text objectText;

    [SerializeField] Text natureInput;

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        nameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        natureInput.text = pokemon.Nature.ToString();
        abilityText.text = $"none";
        objectText.text = $"none";

        statTextsBase[0].text = pokemon.Base.Hp.ToString();
        statTextsBase[1].text = pokemon.Base.Attack.ToString();
        statTextsBase[2].text = pokemon.Base.Defense.ToString();
        statTextsBase[3].text = pokemon.Base.SpAttack.ToString();
        statTextsBase[4].text = pokemon.Base.SpDefense.ToString();
        statTextsBase[5].text = pokemon.Base.Speed.ToString();

        statTextsEVs[0].text = pokemon.EV[Stat.Hp].ToString();
        statTextsEVs[1].text = pokemon.EV[Stat.Attack].ToString();
        statTextsEVs[2].text = pokemon.EV[Stat.Defense].ToString();
        statTextsEVs[3].text = pokemon.EV[Stat.SpAttack].ToString();
        statTextsEVs[4].text = pokemon.EV[Stat.SpDefense].ToString();
        statTextsEVs[5].text = pokemon.EV[Stat.Speed].ToString();

        statTextsIvs[0].text = pokemon.IV[Stat.Hp].ToString();
        statTextsIvs[1].text = pokemon.IV[Stat.Attack].ToString();
        statTextsIvs[2].text = pokemon.IV[Stat.Defense].ToString();
        statTextsIvs[3].text = pokemon.IV[Stat.SpAttack].ToString();
        statTextsIvs[4].text = pokemon.IV[Stat.SpDefense].ToString();
        statTextsIvs[5].text = pokemon.IV[Stat.Speed].ToString();

        statTextsTotal[0].text = pokemon.Stats[Stat.Hp].ToString();
        statTextsTotal[1].text = pokemon.Stats[Stat.Attack].ToString();
        statTextsTotal[2].text = pokemon.Stats[Stat.Defense].ToString();
        statTextsTotal[3].text = pokemon.Stats[Stat.SpAttack].ToString();
        statTextsTotal[4].text = pokemon.Stats[Stat.SpDefense].ToString();
        statTextsTotal[5].text = pokemon.Stats[Stat.Speed].ToString();

    }


}
