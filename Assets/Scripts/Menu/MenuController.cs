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

    [SerializeField] List<Text> mainMenuTexts;
    [SerializeField] GameObject teamBuildScreen;

    [SerializeField] GameObject pokemonBuildScreen;
    [SerializeField] GameObject mainMenuScreen;

    [SerializeField] GameObject playerController;

    void Start()
    {
        teamBuildScreen.GetComponent<TeamBuildScreen>().Init();
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //TODO
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitPokemonBuildScreen();
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
        teamBuildScreen.GetComponent<TeamBuildScreen>().SetPartyData(playerController.GetComponent<PokemonParty>().Pokemons);
        teamBuildScreen.gameObject.SetActive(true);
        pokemonBuildScreen.gameObject.SetActive(false);
        state = MenuState.TeamBuild;
    }
}
