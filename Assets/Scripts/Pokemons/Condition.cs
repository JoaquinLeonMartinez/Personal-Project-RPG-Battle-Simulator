using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove { get; set; } //Func es como action pero puedes devolver un valor, en este caso nosotros queremos devolver un boolean
    public Action<Pokemon> OnAfterTurn { get; set; }
}

