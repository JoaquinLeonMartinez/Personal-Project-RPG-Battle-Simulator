using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }

public enum BattleAction { Move, SwitchPokemon, UseItem, Run, None}

public class BattleSystem : MonoBehaviour
{
    //Slot de player
    [SerializeField] BattleUnit playerUnit;
    //Slot del enemigo
    [SerializeField] BattleUnit enemyUnit;
    //Caja de dialogo (esto gestiona todo el tema de dialogos)
    [SerializeField] BattleDialogBox dialogBox;
    //Pantalla de cambiar de pokemon
    [SerializeField] PartyScreen partyScreen;
    //Evento con el que indicaremos si la batalla ha terminado, actualmente se le pasa un parámetro que no estamos utilizando. En un futuro se utilizaa para indicar si gana o pierde
    public event Action<bool> OnBattleOver; //true si ganas, false si pierdes
    // Estado de en el que se encuentra la batalla
    BattleState state;
    //Estado previo, actualmente solo se utiliza para el cambio de pokemon, con esto difreneciamos si es un cambio voluntario o por muerte d eun pokemon
    BattleState? prevState;
    //iterador para elegir la accion que quieres realizar (atacar, mochila, cambiar de pokemon, etc)
    int currentAction;
    //iterador para seleccionar el movimiento
    int currentMove;
    //iterador de la pantalla de seleccion de pokemon cuando cambias de pokemon
    int currentMember;
    //Equipo del jugador
    PokemonParty playerParty;
    //Equipo del rival
    PokemonParty enemyParty;

    //Joaquin system
    bool isBattleOver = false;

    //Slots del player
    [SerializeField] List<BattleUnit> player1Slots;
    //Slots del enemigo
    [SerializeField] List<BattleUnit> player2Slots;
    //All slots
    List<BattleUnit> gameSlots;
    //Sorted:
    List<BattleUnit> gameSlotsSorted;

    //Equipo del jugador
    PokemonParty player1Team;
    //Equipo del rival
    PokemonParty player2Team;



    //List<BattleAction> battleActions;

    int currentSlot;
    //end Joaquin system

    /*
     * Estos metodos se encargan de iniciar el battle system
     */
    #region InitRegion
    /*
     * Primer metodo que es llamado al iniciar la clase, asigna los equipos al jugador y al enemigo, resetea los iteradores y llama al setup para que haga el resto
     */
    public void StartBattle(PokemonParty playerParty, PokemonParty enemyParty)
    {
        this.playerParty = playerParty;
        this.enemyParty = enemyParty;
        currentAction = 0;
        dialogBox.UpdateActionSelection(currentAction);
        currentMove = 0;
        currentMember = 0;
        StartCoroutine(SetupBattle());
    }
    /*
     * Gestiona quien sera el primer pokemon de cada jugador y actualiza la caja de dialogo
     */
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
    #endregion
    /*
     * En esta parte del codigo agrupamos las funciones que se encargan de gestionar el movimiento en los menus y el update de la clase, que indica cual de los handle estamos utilizando en funcion del estado en el que nos encontramos
     */
    #region HandleSection 
    /*
     * Update (falso) de la clase, se llama cada frame e indica que handle se debe seguir (es invocado por el Game controller)
     */
    public void HandleUpdate()
    {
        //Debug.Log($"Battle State = {state}");

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

        //Debug.Log("State = " + state);
    }
    /*
     * Se encarga de la gestion de las acctiones disponibles para el jugador
     */
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

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if (currentAction == 1)
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
                BattleOver(false); //Pierdes automaticamente
            }
        }
    }
    /*
     * Se encarga de la gestion de la pantalla de seleccionar otro pokemon para salir a la batalla
     */
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

        //currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
        currentMember = Mathf.Clamp(currentMember, 0, player1Team.Pokemons.Count - 1);
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Sale a combatir el pokemon seleccionado
            //var selectedMember = playerParty.Pokemons[currentMember];
            var selectedMember = player1Team.Pokemons[currentMember];
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
                //StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
                player1Slots[currentSlot].battleAction = BattleAction.SwitchPokemon;
            }
            else //En este caso significa que ha muerto el anterior
            {
                //state = BattleState.Busy;wdas
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //Vuelve hacia atras (solo si tu pokemon esta vivo)
            if (playerUnit.Pokemon.CurrentHP > 0)
            {
                partyScreen.gameObject.SetActive(false);
                ActionSelection();
            }
        }

    }
    /*
     * Se encarga de gestionar la seleccion del movimiento
     */
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
                //StartCoroutine(RunTurns(BattleAction.Move));
                player1Slots[currentSlot].Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
                player1Slots[currentSlot].Pokemon.CurrentMove.target = player2Slots[0]; //TODO: Dejar elegir esto en un futuro
                player1Slots[currentSlot].battleAction = BattleAction.Move;
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }
    #endregion

    /*
     * Metodos que se encargan de la gestion del menu
     */
    #region Menu
    /*
     * Actualiza el estado del juego y la UI para entrar al estado de seleccionar accion
     */
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true); ;
    }
    /*
     * Actualiza el estado del juego y la UI para entrar al estado de seleccinar un nuevo pokemon
     */
    void OpenParyScreen()
    {
        state = BattleState.PartyScreen;
        //partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.SetPartyData(player1Team.Pokemons);
        dialogBox.EnableActionSelector(false);
        partyScreen.gameObject.SetActive(true);
    }
    /*
     * Actualiza el estado del juego y la UI para entrar al estado de seleccionar movimiento
     */
    void MoveSelection()
    {
        dialogBox.SetMoveNames(player1Slots[currentSlot].Pokemon.Moves);

        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);      
    }
    #endregion
    /*
     * Metodos que se utilizan en la gestion del turno
     */
    #region Battle
    /*
     * Metodo que se encarga de toda la gestion del turno, que movimiento ira primero, cuando se dara cada efecto, etc
     */
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
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            //lo guardamos por si muere en el primer turno
            var secondPokemon = secondUnit.Pokemon;

            //First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            //Aqui en un futuro ira un "afterMove" para el tema de recoils y demas
            //yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }

            //Check if the second pokemon is still alive
            if (secondPokemon.CurrentHP > 0)
            {
                //Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                //Aqui en un futuro ira un "afterMove" para el tema de recoils y demas
                //yield return RunAfterTurn(secondUnit);
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
            //Aqui en un futuro ira un "afterMove" para el tema de recoils y demas
            //yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
        }

        //Check AfterTurn (Status Conditions, Weather, Fields, etc)
        //Check if they are still alive
        //check speed (to know the order)
        bool playerGoesFirstAfter = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
        var firstUnitAfter = (playerGoesFirstAfter) ? playerUnit : enemyUnit;
        var secondUnitAfter = (playerGoesFirstAfter) ? enemyUnit : playerUnit;
        //No hace falta comprobar si estan vivos porque estamos cogiendo el pokemon directamente del slot
        yield return RunAfterTurn(firstUnitAfter);
        yield return RunAfterTurn(secondUnitAfter);

        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }
    

    /*
     *  Metodo que dado un movimiento, el pokemon que lo hace y el target, ejecuta el movimiento
     */
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //Comprobamos si esta congelado, paralizado, etc 
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();

        if (!canRunMove) //Si no puede se acaba su turno
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            //Acutalizamos la vida por si estaba confuso y se ha golpado a si mismo
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }

        //UI
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used { move.Base.Name}");

        //Comprobamos si no ha fallado
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(Settings.animationDuration);

            targetUnit.PlayHitAnimation();

            //Comprobamos si es de categoria de estados (ni especial ni fisico)
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else //Si entra aqui es que es un movimiento de ataque, ya sea especial o fisico
            {
                //Hacemos el daño que corresponda
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);

                //Comprobamos si tiene algún efecto secundario
                if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.CurrentHP > 0)
                {
                    foreach (var secondary in move.Base.SecondaryEffects)
                    {
                        var rnd = UnityEngine.Random.Range(1, 101);
                        if (rnd <= secondary.Chance)
                        {
                            yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                        }

                    }
                }
            }

            //Comprobamos si el objetivo se ha debilitado
            if (targetUnit.Pokemon.CurrentHP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.name} fainted");

                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(Settings.pauseDuration);

                //Tenemos eliminarlo de la lista de pendientes en los ataques
                DeleteFaintedPk(gameSlotsSorted, sourceUnit.Pokemon.CurrentMove.target);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name}'s attack missed");
        }
    }

    /*
    IEnumerator RunMove_OLD(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
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
            yield return new WaitForSeconds(Settings.animationDuration);

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
                    var rnd = UnityEngine.Random.Range(1, 101);
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

                yield return new WaitForSeconds(Settings.pauseDuration);
                yield return CheckForBattleOver(targetUnit);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name}'s attack missed");
        }
    }
    */
    /*
     * Metodo encargado de gestionar todos los eventos que ocurren al finalizar un turno o movimiento como por ejemplo los efectos de algunos estados alterados
     */
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

            yield return new WaitForSeconds(Settings.pauseDuration);
        }
    }

    public void DeleteFaintedPk(List<BattleUnit> gameSlotsSorted, BattleUnit toDelete)
    {
        int position = -1;
        for (int i = 0; i < gameSlotsSorted.Count && position < 0; i++)
        {
            if (gameSlotsSorted[i] == toDelete)
            {
                position = i;
            }
        }

        if (position != -1) // si no lo encuentra es porque este pokemon ya hizo su turno antes
        {
            gameSlotsSorted.RemoveAt(position);
        }
        
    }

    /*
     * Metodo encargado de gestionar los efectos secundarios de los ataques, como bajadas de stats o estados alterados
     */
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
            EffectResult result = target.SetStatus(effects.Status);

            if (result == EffectResult.AlreadyOne)
            {
                yield return dialogBox.TypeDialog($"{source.CurrentMove.Base.Name} had no effect!");
            }
            else if (result == EffectResult.Inmune)
            {
                yield return dialogBox.TypeDialog($"{target.Base.Name} it's inmune!");
            }
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
    #endregion
    /*
     * Metodos que utilizan los que gestionan la batalla y/o menu
     */
    #region Utils
    /*
     * Metodo encargado de gestionar el cambio de pokemon una vez es seleccionado el nuevo, actualiza la UI y el pokemon
     */
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        //Reset volatile status and boosts in current pokemon
        player1Slots[currentSlot].Pokemon.ResetVolatileStatus(); //TODO: Esto no debe ser en la posicion 0
        player1Slots[currentSlot].Pokemon.ResetStateBoosts();

        //Check if current pokemon is still alive
        if (player1Slots[currentSlot].Pokemon.CurrentHP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {player1Slots[0].Pokemon.Base.Name}");
            player1Slots[0].PlayFaintAnimation(); //TODO. Esto es de momento hasta que haya una animacion de cambiar
            yield return new WaitForSeconds(Settings.pauseDuration);
        }

        //Set up new pokemon in battle
        player1Slots[currentSlot].SetUp(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");
        yield return new WaitForSeconds(Settings.pauseDuration);


        //Debug Info------------------------------------------
        //Debug.Log("New pokemon info:");
        //MyDebug.ShowPokemonState(playerUnit.Pokemon);
        //----------------------------------------------------
        //TODO: Cambiamos el orden del equipo (Para esto deberiamos poder conservar el orden del equipo original, como de momento es una movida pasamos del tema)
    }

    /*
    IEnumerator SwitchPokemon_OLD(Pokemon newPokemon)
    {
        //Debug Info------------------------------------------
        //Debug.Log("Current pokemon info:");
        //MyDebug.ShowPokemonState(playerUnit.Pokemon);
        //----------------------------------------------------

        //Reset volatile status and boosts in current pokemon
        playerUnit.Pokemon.ResetVolatileStatus();
        playerUnit.Pokemon.ResetStateBoosts();

        //Check if current pokemon is still alive
        if (playerUnit.Pokemon.CurrentHP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation(); //TODO. Esto es de momento hasta que haya una animacion de cambiar
            yield return new WaitForSeconds(Settings.pauseDuration);
        }

        //Set up new pokemon in battle
        playerUnit.SetUp(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");

        //Debug Info------------------------------------------
        //Debug.Log("New pokemon info:");
        //MyDebug.ShowPokemonState(playerUnit.Pokemon);
        //----------------------------------------------------

        //TODO: Cambiamos el orden del equipo (Para esto deberiamos poder conservar el orden del equipo original, como de momento es una movida pasamos del tema)

        state = BattleState.RunningTurn;
    }
    */
    /*
     * Metodo encargado de determinar si una batalla ha terminado (cuando a alguno de los dos no les quedan pokemons se acaba
     */
    IEnumerator CheckForBattleOver(BattleUnit faintedUnit) //simplificar este metodo en la medida de lo posible!!
    {
        //Comprobamos si es player o no
        int pkAlive = 0;

        if (faintedUnit.IsPlayer)
        {
            pkAlive = getNumPokemonsAlive(player1Team);

            if (pkAlive == 0) // Gana el enemigo
            {
                BattleOver(false);//false si pierdes, true si ganas
            }
            else if (pkAlive < player1Slots.Count) //no le quedan pokemons que no esten en campo
            {
                //TODO: Nada (capar de alguna forma que no pueda seleccionar este slot ya)
                player1Slots[currentSlot].enabled = false;
            }
            else
            {
                player1Slots[currentSlot].battleAction = BattleAction.None;

                OpenParyScreen();

                while (player1Slots[currentSlot].Pokemon.CurrentHP <= 0)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(Settings.pauseDuration);
            }
        }
        else
        {
            pkAlive = getNumPokemonsAlive(player2Team);

            if (pkAlive == 0) //no le quedan pokemons
            {
                BattleOver(true); //Ganas tu
            }
            else if (pkAlive < player2Slots.Count) //no le quedan pokemons que no esten en campo
            {
                //TODO: Nada (capar de alguna forma que no pueda seleccionar este slot ya)
                player2Slots[currentSlot].enabled = false;
            }
            else
            {
                Pokemon nextPokemon= player2Team.GetHealthyPokemon();

                player2Slots[currentSlot].SetUp(nextPokemon); //TODO: Necesito saber que slot es

                yield return (dialogBox.TypeDialog($"Next enemy pokemon is {nextPokemon.Base.Name}."));

                yield return new WaitForSeconds(Settings.pauseDuration);
            }

        }
    }

    public int getNumPokemonsAlive(PokemonParty player1Team)
    {
        int pokemonsAlive = 0;

        for (int i = 0; i < player1Team.Pokemons.Count; i++)
        {
            if (player1Team.Pokemons[i].CurrentHP > 0)
            {
                pokemonsAlive++;
            }
        }

        return pokemonsAlive;
    }

    /*
     * En base a la precision y cambio en estadisticas de evasion y precision de los afectados indica si golpea el movimiento
     */
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
    /*
     * Indica que la batalla se ha acabado y deja todo preparado para la siguiente
     */
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        //resteamos el estado de los pokemons
        player1Team.Pokemons.ForEach(p => p.OnBattleOver());
        player2Team.Pokemons.ForEach(p => p.OnBattleOver());

        //Esto es temporal, es para que si juegas varias veces con el entrenador este tenga los pokemons curados
        player1Team.HealthParty();
        player2Team.HealthParty();
        OnBattleOver(won); //Evento
    }

    /*
    void BattleOver_OLD(bool won)
    {
        state = BattleState.BattleOver;
        //resteamos el estado de los pokemons
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        enemyParty.Pokemons.ForEach(p => p.OnBattleOver());

        //Esto es temporal, es para que si juegas varias veces con el entrenador este tenga los pokemons curados
        enemyParty.HealthParty();
        playerParty.HealthParty();
        OnBattleOver(won); //Evento
    }
    */

    /*
     * Muestra en el dialogo de batalla todos los efectos que ha tenido el movimiento
     */
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
    /*
     * Muestra en la pantalla de dialogo los estados alterados del pokemon
     */
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    #endregion


    public void Setup(PokemonParty player1Team, PokemonParty player2Team)
    {
        //Game
        this.player1Team = player1Team;
        this.player2Team = player2Team;

        //UI
        currentAction = 0;
        dialogBox.UpdateActionSelection(currentAction);
        currentMove = 0;
        currentMember = 0;

        //Start!
        StartCoroutine(ManageGame());
    }

    /*
     * Manger del transcurso del juego
     */
    IEnumerator ManageGame()
    {
        InitSetupPokemons();

        yield return CheckHabilities();

        while (!isBattleOver)
        {
            yield return ActionSelector();

            yield return ManageTurn();

            yield return CheckEndEffects();

            yield return SelectNewPokemon(); //Si ha muerto alguno durante el combate

            Debug.Log("Como la batalla no ha acabado vamos a por otra ronda");
        }

    }

    void InitSetupPokemons()
    {
        //Iniciamos los slots de ambos jugadores con los pokemons correspondientes
        foreach (var slot in player1Slots)
        {
            //TODO (para dobles): Este metodo devuelve el primer pokemon del equipo que este vivo, habra que hacer otro metodo que devuelva tambien el segundo
            slot.SetUp(player1Team.GetHealthyPokemon());
        }

        foreach (var slot in player2Slots)
        {
            slot.SetUp(player2Team.GetHealthyPokemon());
        }

        //Iniciamos la pantalla de party selection
        partyScreen.Init();

        /*
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        ActionSelection();
        */
    }

    IEnumerator CheckHabilities()
    {
        yield return null;
    }

    IEnumerator ActionSelector()
    {
        ResetSlotsActions();
        //TODO: En el futuro aqui habra que meter la posibilidad de ir hacia atras (en el hadle selection meter un onpress X y poner el current a -1)
        for (int slot = 0; slot < player1Slots.Count; slot++)
        {
            currentSlot = slot;
            ActionSelection();

            while (player1Slots[currentSlot].battleAction == BattleAction.None)
            {
                yield return null;
            }
        }
    }

    public void ResetSlotsActions()
    {
        for (int i = 0; i < player1Slots.Count; i++)
        {
            player1Slots[0].battleAction = BattleAction.None;
        }
    }

    IEnumerator ManageTurn()
    {
        state = BattleState.RunningTurn;

        //Ponemos las prioridades
        SetupPriorities();

        //Ordenamos por prioridad y speed a igualdad de prioridad
        SortSlots();


        while (gameSlotsSorted.Count > 0) //los que vayan atacando o haciendo su turno los vamos borrando
        {
            //Tener en cuenta que si matas a un pokemon hay que eliminarlo de esta lista

            if (gameSlotsSorted[0].GetPriority() == Settings.switchPriority)
            {
                var selectedPokemon = player1Team.Pokemons[currentMember]; //TODO: El current member debe ser un atributo del slot o guardar una lista con ellos (no siempre sera del player1)
                yield return SwitchPokemon(selectedPokemon); //TODO: El switch pokemon debe ser a un slot en concreto, ahora mismo es generico
            }
            else if (gameSlotsSorted[0].GetPriority() == Settings.useItemPriority)
            {
                //TODO
            }
            else //Aqui se engloban todos los ataques
            {
                yield return RunMove(gameSlotsSorted[0], gameSlotsSorted[0].Pokemon.CurrentMove.target, gameSlotsSorted[0].Pokemon.CurrentMove);

                //yield return RunAfterTurn(gameSlotsSorted[0]); //esto será para el tema del recoil y demas, de momento solo lo ponemos al final del turno, no al final del movimiento
            }

            gameSlotsSorted.RemoveAt(0); //siempre eliminamos el 0
            //RefreshSpeeds(gameSlotsSorted); //Habra que adaptar este metodo o hacer uno similar para el ordenar en el propio turno (este solo debe tener en cuenta las velocidades, ya que la prioridad no va a cambiar)
        }
    }

    IEnumerator CheckEndEffects()
    {
        yield return null;
    }

    IEnumerator SelectNewPokemon()
    {
        //Comprobamos si se ha muerto algun pokemon

        //Nuestros
        for (int i = 0; i < player1Slots.Count; i++)
        {
            if (player1Slots[i].Pokemon.CurrentHP <= 0)
            {
                currentSlot = i;
                yield return CheckForBattleOver(player1Slots[i]);
            }
        }

        //Enemigo
        for (int i = 0; i < player2Slots.Count; i++)
        {
            if (player2Slots[i].Pokemon.CurrentHP <= 0)
            {
                currentSlot = i;
                yield return CheckForBattleOver(player2Slots[i]);
            }
        }
    }

    void SetupPriorities()
    {
        //el player 1 ya viene preparado, hay que preparar el player 2, en un futuro esto deberia hacerse antes
        SetupPlayer2Turn();

        //Juntamos los slots, ya nos da igual que sea de player 1 o 2, a partir de ahora trabajamos con game Slots
        MergeSlots();

        //Tanto tuyas como del enemigo
        SetPriorities(gameSlots);

        DebugPrintPriorities();
    }

    void SortSlots()
    {
        //ordenar por prioridad y velocidad, a igualdad de velocidad un random
        List<BattleUnit> gameSlotsOrderedByPriority = new List<BattleUnit>();

        //Recorremos las prioridades y dentro de ellas las velocidades
        for (int priority = Settings.maxPriority; priority >= Settings.minPriority; priority--) //-8 es el limite de prioridades por abajo y 7 es el limite por arriba
        {
            for (int i = 0; i < gameSlots.Count; i++)
            {
                if (gameSlots[i].GetPriority() == priority)
                {
                    gameSlotsOrderedByPriority.Add(gameSlots[i]); //con esto los tenemos ordenados por prioridad
                }
            }
        }

        RefreshSpeeds(gameSlotsOrderedByPriority);

        DebugPrintOrder();
    }

    void RefreshSpeeds(List<BattleUnit> gameSlotsOrderedByPriority)
    {
        List<BattleUnit> auxList = new List<BattleUnit>();
        int currentPriority = Settings.maxPriority;
        int lastPriority = Settings.maxPriority;
        gameSlotsSorted = new List<BattleUnit>(); //reiniciamos esta lista para que no coja nada del turno anterior

        for (int i = 0; i < gameSlotsOrderedByPriority.Count; i++)
        {
            currentPriority = gameSlotsOrderedByPriority[i].GetPriority();
            if (currentPriority == lastPriority)//añadimos a la lista
            {
                auxList.Add(gameSlotsOrderedByPriority[i]);
            }
            else //significa que cambiamos de prioridad
            {
                OrderBySpeed(auxList); //esto añade a la lista final los de este rango de prioridad

                auxList.Clear();
                auxList.Add(gameSlotsOrderedByPriority[i]); //el primero del siguiente rango
            }

            lastPriority = currentPriority;
        }

        OrderBySpeed(auxList); //los que queden con la ultima prioridad tambien los añadimos
        auxList.Clear();
    }

    public void DebugPrintOrder()
    {
        for (int i = 0; i < gameSlotsSorted.Count; i++)
        {
            Debug.Log($"El slot {i} tiene prioridad {gameSlotsSorted[i].GetPriority()} y una velocidad de {gameSlotsSorted[i].Pokemon.Speed}");
        }
    }

    public void DebugPrintPriorities()
    {
        for (int i = 0; i < gameSlots.Count; i++)
        {
            Debug.Log($"El slot {i} tiene prioridad {gameSlots[i].GetPriority()}");
        }
    }

    void OrderBySpeed(List<BattleUnit> inputList) //esto ya lo añade a la definitivas
    {
        while (inputList.Count != 0) 
        {
            int position = GetFaster(inputList);
            gameSlotsSorted.Add(inputList[position]);
            inputList.RemoveAt(position);
        }
    }

    public int GetFaster(List<BattleUnit> inputList)
    {
        int position = 0;
        int best = inputList[position].Pokemon.Speed;
        BattleUnit faster = inputList[position];
        
        for (int i = 1; i < inputList.Count; i++)
        {
            if (best < inputList[i].Pokemon.Speed)
            {
                //faster = inputList[i];
                best = inputList[i].Pokemon.Speed;
                position = i;
            }
            else if (best == inputList[i].Pokemon.Speed)
            {
                if (UnityEngine.Random.Range(0, 101) > 50)// A igualdad de condiciones es un random
                {
                    //faster = inputList[i];
                    position = i;
                }
            }
        }

        return position;
    }

    public void SetupPlayer2Turn()
    {
        for (int i = 0; i < player2Slots.Count; i++)
        {
            player2Slots[i].Pokemon.CurrentMove = player2Slots[i].Pokemon.GetRandomMove();
            player2Slots[i].Pokemon.CurrentMove.target = player1Slots[UnityEngine.Random.Range(0, player1Slots.Count)];
            player2Slots[i].battleAction = BattleAction.Move;
        }
    }

    public void MergeSlots()
    {
        gameSlots = new List<BattleUnit>();

        foreach (var slot in player1Slots)
        {
            gameSlots.Add(slot);
        }

        foreach (var slot in player2Slots)
        {
            gameSlots.Add(slot);
        }
    }

    public void SetPriorities(List<BattleUnit> currentSlots)
    {
        for (int i = 0; i < currentSlots.Count; i++)
        {
            switch (currentSlots[i].battleAction)
            {
                case BattleAction.Move:
                    currentSlots[i].SetPriority(currentSlots[i].Pokemon.CurrentMove.Base.Priority);
                    break;
                case BattleAction.SwitchPokemon:
                    currentSlots[i].SetPriority(Settings.switchPriority);
                    break;
                case BattleAction.UseItem:
                    currentSlots[i].SetPriority(Settings.useItemPriority);
                    break;
                case BattleAction.Run:
                    currentSlots[i].SetPriority(Settings.runPriority);
                    break;
                case BattleAction.None:
                    //esto no deberia ocurrir nunca
                    break;
            }
        }
    }
}
