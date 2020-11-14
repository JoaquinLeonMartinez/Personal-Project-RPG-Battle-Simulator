using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }

public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver; //true si ganas, false si pierdes

    BattleState state;
    BattleState? prevState;
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

        Debug.Log("State = " + state);
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
                prevState = state;
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

            if (prevState == BattleState.ActionSelection) //En este caso cambia porque quiere, de modo que tras el cambio el enemigo atacara
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else //En este caso significa que ha muerto
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
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
                StartCoroutine(RunTurns(BattleAction.Move));
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
        if (playerUnit.Pokemon.CurrentHP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation(); //TODO. Esto es de momento hasta que haya una animacion de cambiar
            yield return new WaitForSeconds(2f);
        }

        playerUnit.SetUp(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");

        //TODO: Cambiamos el orden del equipo (Para esto deberiamos poder conservar el orden del equipo original, como de momento es una movida pasamos del tema)

        state = BattleState.RunningTurn;
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

        move.PP--;
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

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name}'s attack missed");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
        {
            yield break;
        }
        //Con esto esperamos a que el efecto secundario del ataque no haga efecto hasta que (en caso de haber matado al enemigo) salga el nuevo pokemon (cosa que en los juegos actuales no es asi creo recordar)
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

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

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //Check who goes first
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if(enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }

            var firstUnit = (playerGoesFirst)? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            //lo guardamos por si muere en el primer turno
            var secondPokemon = secondUnit.Pokemon;

            //First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) 
            {
                yield break;
            }

            //Antes de ir con el segundo turno, tenemos que chequear que el pokemon del segundo turno no haya muerto en el primero:
            if (secondPokemon.CurrentHP > 0)
            {
                //Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }

            //Enemy Turn
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }

        if (state != BattleState.BattleOver)
        {
            ActionSelection(); //TODO: SOSPECHOSO: El problema es que esta llegando aqui sin esperar a que selecciones el nuevo pokemon (comprobar si pasa solo cuando te matan atacando tu el ultimo o es siempre)
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

                state = BattleState.RunningTurn;

                //ActionSelection();
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
}
