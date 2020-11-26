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
    Dictionary<int, Stat> dictionary;

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        nameText.text = pokemon.Base.Name;
        type1Text.text = pokemon.Base.Type1.ToString();
        type2Text.text = pokemon.Base.Type2.ToString();


        dictionary = new Dictionary<int, Stat>();
        dictionary.Add(0, Stat.Hp);
        dictionary.Add(1, Stat.Attack);
        dictionary.Add(2, Stat.Defense);
        dictionary.Add(3, Stat.SpAttack);
        dictionary.Add(4, Stat.SpDefense);
        dictionary.Add(5, Stat.Speed);

        for (int i = 0; i < statTextsBase.Count; i++)
        {
            statBars[i].SetStatBase(pokemon.Stats[dictionary[i]]);
        }

        statTextsBase[0].text = pokemon.Base.Hp.ToString();
        statTextsBase[1].text = pokemon.Base.Attack.ToString();
        statTextsBase[2].text = pokemon.Base.Defense.ToString();
        statTextsBase[3].text = pokemon.Base.SpAttack.ToString();
        statTextsBase[4].text = pokemon.Base.SpDefense.ToString();
        statTextsBase[5].text = pokemon.Base.Speed.ToString();

    }
}
