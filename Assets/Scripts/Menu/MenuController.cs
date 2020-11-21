using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuState { MainMenu, TeamBuild, PokemonBuild }

public class MenuController : MonoBehaviour
{
    [SerializeField] Color highlightedColor;

    public event Action<bool> OnBattleStart;

    MenuState state;

    int currentMainMenuOption;
    int currentTeamBuildOption;
    int currentPokemonOption;
    int levelSelector;
    int natureSelector;

    bool rightKeyDown;
    bool leftKeyDown;
    float pauseDuration = 0.1f;
    float timerPause;

    [SerializeField] List<Text> mainMenuTexts;
    [SerializeField] GameObject teamBuildScreen;
    [SerializeField] List<Text> buildPokemonTexts; //name + level + nature + moves + stats

    [SerializeField] GameObject pokemonBuildScreen;
    [SerializeField] GameObject mainMenuScreen;

    [SerializeField] GameObject playerController;

    void Start()
    {
        teamBuildScreen.GetComponent<TeamBuildScreen>().Init();
        rightKeyDown = false;
        leftKeyDown = false;
        timerPause = 0;
    }


    void Update()
    {
        
    }

    public void StartBattle(bool isTrainerBattle)
    {
        //lanzamos el evento para que ya quien sea que lo reciba (el GameController) haga lo que considere necesario.
        OnBattleStart(isTrainerBattle);
    }

    public void HandleUpdate()
    {
        if (state == MenuState.MainMenu)
        {
            HandleUpdateMainMenu();
        }
        else if (state == MenuState.TeamBuild)
        {
            HandleUpdateTeamBuild();
        }
        else if (state == MenuState.PokemonBuild)
        {
            HandleUpdatePokemonBuild();
        }
    }

    public void HandleUpdateMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMainMenuOption ++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMainMenuOption --;
        }

        currentMainMenuOption = Mathf.Clamp(currentMainMenuOption, 0, 2);
        UpdateMainMenuSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentMainMenuOption == 0)
            {
                //Battle with trainer
                TrainerBattle();
            }
            else if (currentMainMenuOption == 1)
            {
                //Battle with wild pokemon
                WildPokemonBattle();
            }
            else if (currentMainMenuOption == 2)
            {
                //Teambuild
                TeamBuild();
            }

        }
    }

    public void HandleUpdateTeamBuild()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentTeamBuildOption++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTeamBuildOption--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentTeamBuildOption += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentTeamBuildOption -= 2;
        }

        currentTeamBuildOption = Mathf.Clamp(currentTeamBuildOption, 0, playerController.GetComponent<PokemonParty>().Pokemons.Count - 1);
        teamBuildScreen.GetComponent<TeamBuildScreen>().UpdateMemberSelection(currentTeamBuildOption);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GoToSelectedPokemon();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitTeamBuildScreen();
        }
    }

    public void HandleUpdatePokemonBuild()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentPokemonOption++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentPokemonOption--;
        }

        currentPokemonOption = Mathf.Clamp(currentPokemonOption, 0, buildPokemonTexts.Count - 1);
        UpdateBuildPokemonSelection(currentPokemonOption);

        //Para esto no hace falta presionar Z
        if (currentPokemonOption == 1) // Level Selector
        {
            LevelSelector();
        }
        else if (currentPokemonOption == 2) // Active Nature selector
        {
            NatureSelector();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //TODO
            if (currentPokemonOption == 0) // Go to Pokemon Screen
            {

            }
            else if (currentPokemonOption == 3) // Go to menu Screen
            {

            }
            else if (currentPokemonOption == 4) // Go to stats Screen
            {

            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitPokemonBuildScreen();
        }
    }

    public void NatureSelector()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftKeyDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightKeyDown = true;
        }

        if (leftKeyDown)
        {
            if (timerPause <= 0)
            {
                natureSelector--;
                timerPause = pauseDuration;
            }
            else
            {
                timerPause -= Time.deltaTime;
            }
        }
        if (rightKeyDown)
        {
            if (timerPause <= 0)
            {
                natureSelector++;
                timerPause = pauseDuration;
            }
            else
            {
                timerPause -= Time.deltaTime;
            }
        }

        natureSelector = Mathf.Clamp(natureSelector, 0, Enum.GetNames(typeof(PokemonNature)).Length - 1);
        buildPokemonTexts[currentPokemonOption].text = ((PokemonNature)natureSelector).ToString();
        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Nature = ((PokemonNature)natureSelector);

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftKeyDown = false;
            timerPause = 0;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightKeyDown = false;
            timerPause = 0;
        }
    }
    public void LevelSelector()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftKeyDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightKeyDown = true;
        }

        if (leftKeyDown)
        {
            if (levelSelector > 0)
            {
                if (timerPause <= 0)
                {
                    levelSelector--;
                    buildPokemonTexts[currentPokemonOption].text = levelSelector.ToString();
                    playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Level = levelSelector;
                    timerPause = pauseDuration;
                }
                else
                {
                    timerPause -= Time.deltaTime;
                }
            }
        }
        if (rightKeyDown)
        {
            if (levelSelector < 100)
            {
                if (timerPause <= 0)
                {
                    levelSelector++;
                    buildPokemonTexts[currentPokemonOption].text = levelSelector.ToString();
                    playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Level = levelSelector;
                    timerPause = pauseDuration;
                }
                else
                {
                    timerPause -= Time.deltaTime;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftKeyDown = false;
            timerPause = 0;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightKeyDown = false;
            timerPause = 0;
        }

    }
    public void UpdateBuildPokemonSelection(int selected)
    {
        for (int i = 0; i < buildPokemonTexts.Count; i++)
        {
            if (i == selected)
            {
                buildPokemonTexts[i].color = highlightedColor;
            }
            else
            {
                buildPokemonTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMainMenuSelection()
    {
        for (int i = 0; i < mainMenuTexts.Count; i++)
        {
            if (i == currentMainMenuOption)
            {
                mainMenuTexts[i].color = highlightedColor;
            }
            else
            {
                mainMenuTexts[i].color = Color.black;
            }
        }
    }

    public void TrainerBattle()
    {
        StartBattle(true);
    }

    public void WildPokemonBattle()
    {
        StartBattle(false);
    }

    public void TeamBuild()
    {
        state = MenuState.TeamBuild;
        teamBuildScreen.GetComponent<TeamBuildScreen>().SetPartyData(playerController.GetComponent<PokemonParty>().Pokemons);
        mainMenuScreen.SetActive(false);
        teamBuildScreen.gameObject.SetActive(true);
    }

    public void GoToSelectedPokemon() //Habra que ver como llamar a este desde el editor (con el click del raton) y que vaya al adecuado
    {
        teamBuildScreen.gameObject.SetActive(false);
        pokemonBuildScreen.gameObject.SetActive(true);
        pokemonBuildScreen.GetComponent<PokemonBuilderUI>().SetData(playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption]);
        levelSelector = playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Level;
        natureSelector = (int)playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Nature;
        state = MenuState.PokemonBuild;
    }

    public void ExitTeamBuildScreen()
    {
        teamBuildScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(true);
        state = MenuState.MainMenu;
    }

    public void ExitPokemonBuildScreen()
    {
        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].RefreshStats();
        teamBuildScreen.GetComponent<TeamBuildScreen>().SetPartyData(playerController.GetComponent<PokemonParty>().Pokemons);
        teamBuildScreen.gameObject.SetActive(true);
        pokemonBuildScreen.gameObject.SetActive(false);
        state = MenuState.TeamBuild;
        currentPokemonOption = 0;
        levelSelector = 0;
        natureSelector = 0;
    }
}
