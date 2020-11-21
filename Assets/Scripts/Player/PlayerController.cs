using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //De momento esto no lo vamos a utilizar
    /*
     * Esta clase seria el controlador del personaje, pero de momento no va a haber personaje de modo que se utilizara para gestionar el main menu (que sera un link a comenzar la batalla de momento)
     * 
     */

    //Esta clase ya esta referenciada desde el gameController, de modo que para evitar referencias circulares seguremos el patron Observer, declararemos un evento y el gameController tendra un listener

    public event Action OnBattleStart;

    void Start()
    {
        
    }

    
    public void HandleUpdate()
    {
        
    }

    /* Esto ha pasado al MenuController, ya que de momento el player no lo usaremos para nada
    public void StartBattle()
    {
        //lanzamos el evento para que ya quien sea que lo reciba (el GameController) haga lo que considere necesario.
        OnBattleStart();
    }
    */
}
