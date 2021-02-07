using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public BattleUnit target;

    public Move(MoveBase _base)
    {
        Base = _base;
        PP = _base.Pp; //de inicio tiene el maximo de PPs
    }

}
