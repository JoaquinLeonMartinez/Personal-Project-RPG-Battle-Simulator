using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] GameObject stat;
    [SerializeField] Color lowColor;
    [SerializeField] Color mediumColor;
    [SerializeField] Color highColor;
    [SerializeField] Color reallyHighColor;
    //TODO: Esto deberia ser una constante (y los valores del update color tambien)
    int maxStatValue = 785;

    public void SetStat(int statValue)
    {
        float statNormalized = (float)statValue / (float)maxStatValue;

        stat.transform.localScale = new Vector3(statNormalized, 1f);
        UpdateColor(statNormalized);
    }

    public void UpdateColor(float curHp)
    {
        if (curHp > 0.65f)
        {
            stat.GetComponent<Image>().color = reallyHighColor;
        }
        else if (curHp > 0.4f)
        {
            stat.GetComponent<Image>().color = mediumColor;
        }
        else if (curHp > 0.2f)
        {
            stat.GetComponent<Image>().color = mediumColor;
        }
        else
        {
            stat.GetComponent<Image>().color = lowColor;
        }
    }
}
