using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovesSelectorScreen : MonoBehaviour
{
    [SerializeField] List<Text> currentMovesTexts;
    [SerializeField] List<MovesSlotLoader> movesSlots;
    [SerializeField] Image frontSprite;
    int selectedSlot = 0;

    public void SetData(Pokemon pokemon)
    {
        frontSprite.sprite = pokemon.Base.FrontSprite;
        SetCurrentMoveNames(pokemon.Moves);
        SetSlotsData(selectedSlot, pokemon);
    }

    public void SetCurrentMoveNames(List<Move> moves)
    {
        for (int i = 0; i < currentMovesTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                currentMovesTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                currentMovesTexts[i].text = "-";
            }
        }
    }

    public void SetMovesSlots(List<LearnableMove> moves)
    {

        for (int i = 0; i < movesSlots.Count; i++)
        {
            if (i < moves.Count)
            {
                movesSlots[i].SetDataMove(moves[i].Base);
            }
            else
            {
                movesSlots[i].SetDefaultDataMove();
            }
        }
    }

    public int GetMoveSlotsLength()
    {
        return movesSlots.Count;
    }

    public void SetSlotsData(int selectedSlot, Pokemon pokemon)
    {
        this.selectedSlot = selectedSlot;
        for (int i = 0; i < movesSlots.Count; i++)
        {
            if (i < pokemon.Moves.Count)
            {
                movesSlots[i].SetDataMove(pokemon.Base.LearnableMoves[selectedSlot + i].Base);
            }
            else
            {
                movesSlots[i].SetDefaultDataMove();
            }
        }
    }

    public void SetSlotSelected(int slotSelected)
    {
        for (int i = 0; i < movesSlots.Count; i++)
        {
            if (i == slotSelected)
            {
                movesSlots[i].SetSelected(true);
            }
            else
            {
                movesSlots[i].SetSelected(false);
            }
        }
    }
}
