using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuState { MainMenu, TeamBuild, PokemonBuild, StatsEditor }

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
    int statSelector;
    int statPoints;
    //Todo: estos valores debrian estar en una clase a parte de constantes
    int iVsLimit = 31;
    int eVsLimit = 252;
    int eVsGlobalLimit = 512;

    bool rightKeyDown;
    bool leftKeyDown;
    float pauseDuration = 0.1f;
    float timerPause;

    [SerializeField] List<Text> mainMenuTexts;
    [SerializeField] GameObject teamBuildScreen;
    [SerializeField] GameObject statsEditorScreen;
    [SerializeField] List<Text> buildPokemonTexts; //name + level + nature + moves + stats
    [SerializeField] List<Text> EVsPokemonTexts;
    [SerializeField] List<Text> IVsPokemonTexts;
    bool isEVsEditor;

    [SerializeField] GameObject pokemonBuildScreen;
    [SerializeField] GameObject mainMenuScreen;

    [SerializeField] GameObject playerController;

    void Start()
    {
        teamBuildScreen.GetComponent<TeamBuildScreen>().Init();
        rightKeyDown = false;
        leftKeyDown = false;
        timerPause = 0;
        isEVsEditor = true;
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
        else if (state == MenuState.StatsEditor)
        {
            HandleUpdatePokemonStats();
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
        UpdateSelection(currentMainMenuOption, mainMenuTexts);


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
        UpdateSelection(currentPokemonOption, buildPokemonTexts);

        //Para esto no hace falta presionar Z
        if (currentPokemonOption == 1) // Level Selector
        {
            LevelSelector();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //TODO
            if (currentPokemonOption == 0) // Go to Pokemon Screen
            {

            }
            else if (currentPokemonOption == 2) // Go to object screen
            {

            }
            else if (currentPokemonOption == 3) // Go to ability Screen
            {

            }
            else if (currentPokemonOption == 4) // Go to moves Screen
            {

            }
            else if (currentPokemonOption == 5) // Go to stats Screen
            {
                GoToStatsScreen();
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ExitPokemonBuildScreen();
        }
    }

    public void HandleUpdatePokemonStats()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            statSelector ++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            statSelector --;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (statSelector == 0)
            {
                isEVsEditor = false;
                EVsPokemonTexts[0].color = Color.black;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (statSelector == 0)
            {
                isEVsEditor = true;
                IVsPokemonTexts[0].color = Color.black;
            }
        }

        if (isEVsEditor)
        {
            statSelector = Mathf.Clamp(statSelector, 0, EVsPokemonTexts.Count - 1);
            UpdateSelection(statSelector, EVsPokemonTexts);
        }
        else
        {
            statSelector = Mathf.Clamp(statSelector, 0, IVsPokemonTexts.Count - 1);
            UpdateSelection(statSelector, IVsPokemonTexts);
        }

        if (statSelector > 0 && statSelector < EVsPokemonTexts.Count - 2)
        {
            if (isEVsEditor)
            {
                StatsSelector(EVsPokemonTexts);
            }
            else
            {
                StatsSelector(IVsPokemonTexts);
            }
        }
        else if (statSelector == EVsPokemonTexts.Count - 1)
        {
            NatureSelector();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //TODO (en principio aqui nada)
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            GoToSelectedPokemon();
        }
    }

    public void StatsSelector(List<Text> statsText)
    {
        
        int limitPoints = iVsLimit;
        if (isEVsEditor)
        {
            limitPoints = eVsLimit;
        }

        Stat choicedStat = Stat.Hp;

        if(statSelector == 1) //hp
        {
            choicedStat = Stat.Hp;
        }
        else if (statSelector == 2) //Attack
        {
            choicedStat = Stat.Attack;
        }
        else if (statSelector == 3) //Defense
        {
            choicedStat = Stat.Defense;
        }
        else if (statSelector == 4) //SpAttack
        {
            choicedStat = Stat.SpAttack;
        }
        else if (statSelector == 5) //SpDefense
        {
            choicedStat = Stat.SpDefense;
        }
        else if (statSelector == 6) //Speed
        {
            choicedStat = Stat.Speed;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftKeyDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightKeyDown = true;
        }

        if (isEVsEditor)
        {
            statPoints = playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].EV[choicedStat];
        }
        else
        {
            statPoints = playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].IV[choicedStat];
        }
        

        if (leftKeyDown)
        {
            if (statPoints > 0)
            {
                if (timerPause <= 0)
                {
                    if (isEVsEditor)
                    {
                        statPoints -= 4;
                        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].EV[choicedStat] = statPoints;
                        RefreshStatEditor();
                    }
                    else
                    {
                        statPoints--;
                        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].IV[choicedStat] = statPoints;
                        RefreshStatEditor();
                    }
                    
                    statsText[statSelector].text = statPoints.ToString();
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
            if (statPoints < limitPoints)
            {
                if (timerPause <= 0)
                {
                    if (isEVsEditor) 
                    {
                        if (eVsGlobalLimit > playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].GetAllEvsSum())
                        {
                            statPoints += 4;
                            playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].EV[choicedStat] = statPoints;
                            RefreshStatEditor();
                        }
                    }
                    else
                    {
                        statPoints ++;
                        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].IV[choicedStat] = statPoints;
                        RefreshStatEditor();
                    }
                    statsText[statSelector].text = statPoints.ToString();
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

    public void RefreshStatEditor()
    {
        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].RefreshStats();
        statsEditorScreen.GetComponent<StatsBuilderScreen>().SetData(playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption]);
    }
    //TODO: Haciendo el componente stats un componente individual aqui no harian falta dos metodos
    public void RefreshPokemonEditor()
    {
        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].RefreshStats();
        //Siempre se llamaria a este:
        pokemonBuildScreen.GetComponent<StatsBuildPanel>().SetData(playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption]);
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
        playerController.GetComponent<PokemonParty>().Pokemons[currentTeamBuildOption].Nature = ((PokemonNature)natureSelector);
        RefreshStatEditor();

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
                    RefreshPokemonEditor();
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
                    RefreshPokemonEditor();
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

    public void UpdateSelection(int selected, List<Text> textsList)
    {
        for (int i = 0; i < textsList.Count; i++)
        {
            if (i == selected)
            {
                textsList[i].color = highlightedColor;
            }
            else
            {
                textsList[i].color = Color.black;
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
        statSelector = 0;
    }

    public void GoToStatsScreen()
    {
        RefreshStatEditor();
        state = MenuState.StatsEditor;
        statsEditorScreen.SetActive(true);
        pokemonBuildScreen.SetActive(false);
    }
}
