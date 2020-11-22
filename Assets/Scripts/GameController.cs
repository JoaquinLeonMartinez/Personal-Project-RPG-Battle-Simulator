using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    //STATE DESIGN PATTERN : Es un patron de diseño que se utiliza para gestionar los distintos controladores dentro del juego
    //[SerializeField] PlayerController playerController; //De momento nada
    [SerializeField] MenuController menuController;
    [SerializeField] GameObject playerController;
    [SerializeField] GameObject enemyController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] GameObject mainMenu;

    public bool isTrainerBattle;
     
    GameState state;

    private void Awake()
    {
        ConditionsDB.Init();
    }

    private void Start()
    {
        menuController.OnBattleStart += StartBattle; //nos subscrivimos al evento e indicamos que accion realizaremos 
        battleSystem.OnBattleOver += EndBattle;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }

    }

    void StartBattle(bool isTrainerBattle)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainMenu.SetActive(false);
        this.isTrainerBattle = isTrainerBattle;

        var playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty enemyParty = null;
        //Aqui deberia diferenciar entre si es un entrenador o un pokemon salvaje
        if (isTrainerBattle)
        {
            enemyParty = enemyController.GetComponent<PokemonParty>();
        }
        else
        {
            //De momento solo hay un pokemon salvaje por encuentro, pero dejamos que sea con una lista por si hubiera mas
            List<Pokemon> wildPokemonsAux = new List<Pokemon>();
            wildPokemonsAux.Add(enemyController.GetComponent<MapArea>().GetRandomPokemon());
            enemyParty = new PokemonParty(wildPokemonsAux);
        }
        
        battleSystem.StartBattle(playerParty, enemyParty);
        //playerController.gameObject.SetActive(false);
        //En caso de realmente pasar del juego a la pantalla de batalla habria que cambiar la camara, ya que no serían la misma, pero en este caso solo tenemos una
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        if (won)
        {
            //TODO
        }
        else
        {
            //TODO
        }
    }

    //Metodo temporal mientras no haya juego y solo menu
    public void ActiveTrainerBattle()
    {
        isTrainerBattle = true;
    }
    //Metodo temporal mientras no haya juego y solo menu
    public void DeactiveTrainerBattle()
    {
        isTrainerBattle = false;
    }

    public void EndGame()
    {
        Application.Quit();
    }

}
