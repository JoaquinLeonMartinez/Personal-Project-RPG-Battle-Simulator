using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver; //true si ganas, false si pierdes

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    PokemonParty enemyParty;

    public void StartBattle(PokemonParty playerParty, PokemonParty enemyParty)
    {
        this.playerParty = playerParty;
        this.enemyParty = enemyParty;
        currentAction = 0;
        currentMove = 0;
        currentMember = 0;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.SetUp(playerParty.GetHealthyPokemon()); 
        enemyUnit.SetUp(enemyParty.GetHealthyPokemon());

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves); //siempre seran los del player

        //Si un string empieza con un $ puedes referenciar otros valoresd dentro del propio string
        //Si desde una corrutina se quiere iniciar otra, basta con hacer un yield
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

        ActionSelection();
    }

    void HandleActionSelection() 
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;     
        }

        currentAction = Mathf.Clamp(currentAction,0,3);


        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {

            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if(currentAction == 1)
            {
                //Bag
            }
            else if (currentAction == 2)
            {
                //Pokemon Party
                OpenParyScreen();
            }
            else if (currentAction == 3)
            {
                //Run
            }

        }
    }

    void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //sale a combatir el pokemon que sea
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.CurrentHP <= 0)
            {
                partyScreen.SetMessageText("You can´t send out a fainted pokemon");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can´t switch with the same pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //vuelve hacia atras (solo si tu pokemon esta vivo)
            if (playerUnit.Pokemon.CurrentHP > 0)
            {
                partyScreen.gameObject.SetActive(false);
                ActionSelection();
            }
        }

    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerUnit.Pokemon.Moves[currentMove].PP > 0) //TODO: habria que validar que le queden movimientos, sino habra que hacer el movimiento combate
            {
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(PlayerMove());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true); ;
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool defeatedPokemon = !(playerUnit.Pokemon.CurrentHP > 0); //con esto comprobamos si el cambio se debe a que has muerto o si es voluntario
        if (!defeatedPokemon)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation(); //TODO. Esto es de momento hasta que haya una animacion de cambiar
            yield return new WaitForSeconds(2f);
        }

        playerUnit.SetUp(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");

        //TODO: Cambiamos el orden del equipo (Para esto deberiamos poder conservar el orden del equipo original, como de momento es una movida pasamos del tema)

        if (!defeatedPokemon) //esto es que cambias porque quieres, de modo que el enemigo te ataca de free
        {
            StartCoroutine(EnemyMove());
        }
        else // esto es que cambias porque te han matado un pokemon, de modo que no atacas
        {
            // El futuro -->ChooseFirstTurn();
            ActionSelection();
        }
    }

    void OpenParyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        dialogBox.EnableActionSelector(false);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
        
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove; //con esto indicamosque el jeugo esta calculando sus cosillas y que no haga nada mientras tanto

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        //Si la batalla no acaba la fiesta continua:
        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--; //A la hora de seleccionar el movimiento hay que poner la condicion de que no se pueda seleccionar si no le quedan PPs (aqui ya es demasiado tarde proque ya esta seleccionado)
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used { move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.CurrentHP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1,101);
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }

                }
            }

            if (targetUnit.Pokemon.CurrentHP <= 0) //si el target muere
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.name} fainted");

                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);
                yield return CheckForBattleOver(targetUnit);
            }

            sourceUnit.Pokemon.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();//actualizamos el hud por si se reduce la HP del pokemon

            //comprobamos si muere por el estado alterado:
            if (sourceUnit.Pokemon.CurrentHP <= 0)
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name} fainted");

                sourceUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);
                yield return CheckForBattleOver(sourceUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name}'s attack missed");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        //Boosts
        if (effects.Boosts != null) //de momento solo contemplamos los ataques de estado que implican boosts
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else if (moveTarget == MoveTarget.Foe)
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        //Status conditions
        if (effects.Status != ConditionID.none)
        {
            //TODO: Estamos suponiendo que el target del status es el enemigo y no uno mismo
            target.SetStatus(effects.Status);
        }

        //Volatile Status conditions
        if (effects.VolatileStatus != ConditionID.none)
        {
            //TODO: Estamos suponiendo que el target del status es el enemigo y no uno mismo
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        //Mostramos en el dialogo las bajadas y subidas de stats de ambos pokemons
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
        
    }

    IEnumerator CheckForBattleOver(BattleUnit faintedUnit) //simplificar este metodo en la medida de lo posible!!
    {
        if (faintedUnit.IsPlayer)
        {
            //Comprobamos si quedan mas pokemons vivos:
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenParyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            var nextPokemon = enemyParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                //se selecciona el siguiente automaticamente de momento, en un futuro se elegira en el equipo
                //CopyPaste temporal:
                //-----------------------------------------------------------------------------------------------------------------------
                enemyUnit.SetUp(nextPokemon);

                yield return (dialogBox.TypeDialog($"Next enemy pokemon is {nextPokemon.Base.Name}."));

                yield return new WaitForSeconds(2f);

                ActionSelection();
                //-----------------------------------------------------------------------------------------------------------------------
            }
            else
            {
                BattleOver(true);
            }
        }
    }

    //en base a la precision indica si golpear el movimiento
    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
        {
            return true;
        }
        float moveAccurarcy = move.Base.Accurarcy;
        int accurarcy = source.StatBoosts[Stat.Accurarcy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };

        if (accurarcy >= 0)
        {
            moveAccurarcy *= boostValues[accurarcy];
        }
        else
        {
            moveAccurarcy /= boostValues[accurarcy];
        }

        if (evasion >= 0)
        {
            moveAccurarcy /= boostValues[evasion];
        }
        else
        {
            moveAccurarcy *= boostValues[evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccurarcy;
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        //TODO: hay que comprobar que le queden PPs y validar que le queden movimientos, sino habra que hacer el movimiento combate
        var move = enemyUnit.Pokemon.GetRandomMove(); //Esto en el futuro sera una IA

        yield return RunMove(enemyUnit, playerUnit, move);

        //Si la batalla no acaba la fiesta continua:
        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
            

    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        //resteamos el estado de los pokemons
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        enemyParty.Pokemons.ForEach(p => p.OnBattleOver());

        //Esto es temporal, es para que si juegas varias veces con el entrenador este tenga los pokemons curados
        enemyParty.HealthParty();

        OnBattleOver(won);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog($"A critical hit!");
        }
        if (damageDetails.Effectiveness > 1)
        {
            yield return dialogBox.TypeDialog($"It´s super effective!");
        }
        else if (damageDetails.Effectiveness < 1)
        {
            yield return dialogBox.TypeDialog($"It´s not very effective!");
        }
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void ChooseFirstTurn() //ESTO NO ME CONVENCE NADA, PERO DE MOMENTO LO DEJAREMOS ASI
    {
        //TODO:
        //En caso de ser iguales atacas tu primero, pero esto en un futuro sera un random
        //En el futuro se tendran en cuenta las prioridad
        //Tener en cuenta a todos los pokemons de la batalla, no solo a 2 (para cuando haya batallas multiples)

        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
}
