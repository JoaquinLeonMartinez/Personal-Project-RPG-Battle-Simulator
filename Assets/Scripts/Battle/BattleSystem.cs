using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
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
        playerHud.setData(playerUnit.Pokemon);
        enemyHud.setData(enemyUnit.Pokemon);

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
        if (playerUnit.Pokemon.CurrentHP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation(); //TODO. Esto es de momento hasta qu haya una animacion de cambiar
            yield return new WaitForSeconds(2f);
        }

        playerUnit.SetUp(newPokemon);
        playerHud.setData(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");

        StartCoroutine(EnemyMove());

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
        move.PP--; //A la hora de seleccionar el movimiento hay que poner la condicion de que no se pueda seleccionar si no le quedan PPs (aqui ya es demasiado tarde proque ya esta seleccionado)
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used { move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted) //si el enemigo muere
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.name} fainted");

            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);


            //Comprobamos si quedan mas pokemons vivos:
            var nextPokemon = enemyParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                //se selecciona el siguiente automaticamente de momento, en un futuro se elegira en el equipo
                //CopyPaste temporal:
                //-----------------------------------------------------------------------------------------------------------------------
                enemyUnit.SetUp(nextPokemon);
                enemyHud.setData(nextPokemon);

                yield return dialogBox.TypeDialog($"Next enemy pokemon is {nextPokemon.Base.Name}.");

                ActionSelection();
                //-----------------------------------------------------------------------------------------------------------------------
            }
            else
            {
                //Curar a los pokemons del enemigo:
                enemyParty.HealthParty();
                OnBattleOver(true); //ACABA EL JUEGO
            }


            
        }
        else //si el enemigo sobrevive atacara
        {
            StartCoroutine(EnemyMove());
        }

    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        //TODO: hay que comprobar que le queden PPs y validar que le queden movimientos, sino habra que hacer el movimiento combate
        var move = enemyUnit.Pokemon.GetRandomMove(); //Esto en el futuro sera una IA

        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used { move.Base.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted) //SI TU POKEMON MUERE
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.name} fainted");

            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);


            //Comprobamos si quedan mas pokemons vivos:
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                //se selecciona el siguiente automaticamente de momento, en un futuro se elegira en el equipo
                OpenParyScreen();

            }
            else
            {
                OnBattleOver(false); //ACABA EL JUEGO
            }

            

        }
        else //SI SOBREVIVES VUELVES A SELECCIONAR MOVIMIENTO
        {
            ActionSelection();
        }
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
}
