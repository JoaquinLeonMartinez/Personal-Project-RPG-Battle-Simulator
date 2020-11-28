using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSlotLoader : MonoBehaviour
{
    [SerializeField] Text idText;
    [SerializeField] Text nameText;

    [SerializeField] Color highLightedColor;

    public void SetData(PokemonBase pokemon)
    {
        if (pokemon != null)
        {
            idText.text = pokemon.PokedexId.ToString();
            nameText.text = pokemon.Name;
        }
        else
        {
            SetDefaultData();
        }

    }

    public void SetDefaultData()
    {
        idText.text = "?";
        nameText.text = "---";
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            idText.color = highLightedColor;
            nameText.color = highLightedColor;
        }
        else
        {
            idText.color = Color.black;
            nameText.color = Color.black;
        }
    }


}
