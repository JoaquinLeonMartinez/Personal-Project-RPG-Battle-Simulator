using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{

    [SerializeField] GameObject health;
    [SerializeField] Color lowHpColor;
    [SerializeField] Color mediumHpColor;
    [SerializeField] Color highHpColor;
    [SerializeField] Text currentHpText;

    public void SetHP(int currentHp, int maxHp)
    {
        float hpNormalized = (float)currentHp / (float)maxHp;

        health.transform.localScale = new Vector3(hpNormalized, 1f);
        UpdateColor(hpNormalized);
        currentHpText.text = $" {currentHp} / {maxHp} ";
    }

    public IEnumerator SetHPSmooth(int currentHp, int maxHp)//esta HP tiene que venir normalizada
    {
        float newHp = (float)currentHp / (float)maxHp;
        float curHp = health.transform.localScale.x;
        //Debug.Log($"curHP = {curHp}");
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
            UpdateColor(curHp);
            currentHpText.text = $" {(int)(curHp*maxHp)} / {maxHp} "; 
        }
        health.transform.localScale = new Vector3(newHp, 1f);
        UpdateColor(newHp);
        currentHpText.text = $" {currentHp} / {maxHp} "; 
    }

    public void UpdateColor(float curHp)
    {
        if (curHp > 0.5f)
        {
            health.GetComponent<Image>().color = highHpColor;
        }
        else if (curHp < 0.5f && curHp > 0.25f)
        {
            health.GetComponent<Image>().color = mediumHpColor;
        }
        else
        {
            health.GetComponent<Image>().color = lowHpColor;
        }
    }
}
