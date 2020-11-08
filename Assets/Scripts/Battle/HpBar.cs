using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{

    [SerializeField] GameObject health;

    public void SetHP(float hpNormalixed)
    {
        health.transform.localScale = new Vector3(hpNormalixed, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)//esta HP tiene que venir normalizada
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);
    }
}
