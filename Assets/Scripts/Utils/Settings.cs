using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{
    //TODO; EN UN FUTURO ESTO DEBERA LEERSE DE UN FICHERO
    //TODO: SI QUIERO PODER EDITARLAS DESDE EL JUEGO ESTO DEBERA SER UN SINGLETON PERO NO UNA CLASE ESTATICA
    //Velocidad a la que se escriben las frases del cuadro de dialogo
    public static float lettersPerSecond = 60f;
    //Duracion de la pausa entre textos
    public static float pauseDuration = 1f;
    //duracion de la animacion
    public static float animationDuration = 1f;

    public static int switchPriority = 7;
    public static int useItemPriority = 6;
    public static int runPriority = 8;
    public static int maxPriority = 8;
    public static int minPriority = -8;
}
