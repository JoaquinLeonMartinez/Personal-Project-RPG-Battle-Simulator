using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovesSlotLoader : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text typeText;
    [SerializeField] Text categoryText;
    [SerializeField] Text powerText;
    [SerializeField] Text accuracyText;

    [SerializeField] Color highLightedColor;

    public void SetDataMove(MoveBase move)
    {
        nameText.text = move.Name;
        typeText.text = move.Type.ToString();
        categoryText.text = move.Category.ToString();
        powerText.text = move.Power.ToString();
        accuracyText.text = move.Accurarcy.ToString();
    }

    public void SetDefaultDataMove()
    {
        nameText.text = "-";
        typeText.text = "-";
        categoryText.text = "-";
        powerText.text = "-";
        accuracyText.text = "-";
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            nameText.color = highLightedColor;
            typeText.color = highLightedColor;
            categoryText.color = highLightedColor;
            powerText.color = highLightedColor;
            accuracyText.color = highLightedColor;
        }
        else
        {
            nameText.color = Color.black;
            typeText.color = Color.black;
            categoryText.color = Color.black;
            powerText.color = Color.black;
            accuracyText.color = Color.black;
        }
    }
}
