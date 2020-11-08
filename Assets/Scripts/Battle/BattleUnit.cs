using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    //[SerializeField] PokemonBase _base; //ESTO EN UN FUTURO DEBERA VENIR MARCADO POR UN SCRIPT, DE MOMENTO LO HAREMOS DESDE EL EDITOR
    //[SerializeField] int level;
    [SerializeField] bool isPlayer;

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition; //utilizamos local position para que sea respectiva al canvas y no al mundo
        originalColor = image.color;
    }

    public Pokemon Pokemon { get; set; }
    public void SetUp(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayer)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }

        image.color = originalColor;
        PlayEnerAnimation();
    }

    //TODO: Todos estos valores de las animacion deberian parametrizarse
    public void PlayEnerAnimation()
    {
        if (isPlayer)
        {
            image.transform.localPosition = new Vector3(-600f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(600f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 2f); //parametros: Destino y tiempo
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f)) ; //color y duracion de la transicion
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0F, 0.5f));
        //esto deja el BattleUnit abajo, pero en realidad da igual porque cuando se seleccione otro pokemon se volvera a setear
    }

}
