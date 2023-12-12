using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GMT
{
    public static bool normalMode;
    public static bool gargleMode;
    public static bool playerInGame;

    public static int score;
    public static int highScore;
    public static string grade; //"A","S","F"

    public static int mlCollect;
    public static string poisonTime;
    public static int poisonTimeScore;

    public static int difficultyID;
    public static float snakeMaxTime;

    public static int snakeID;

    public static float HorizontalInput()
    {
        //return Input.touchCount > 0 ? (Input.GetTouch(0).position.x < Screen.width / 2 ? -1 : 1) : Input.GetAxisRaw("Horizontal");

        float horizontalInput = 0;

        if (Input.touchCount > 0)
        {
            horizontalInput = Input.GetTouch(0).position.x < Screen.width / 2 ? -1 : 1;
        }
        else
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        return horizontalInput;
    }
}

public class GameManagerT : MonoBehaviour
{
    [SerializeField] AudioSource[] music;//0 menu, 1 game
    [SerializeField] float[] musicVolume;//0 menu, 1 game

    void Update()
    {
        if (musicVolume[0] < 0.75f) musicVolume[0] += Time.deltaTime;
        if (musicVolume[1] < 0.75f) musicVolume[1] += Time.deltaTime;

        music[0].volume = musicVolume[0];
        music[1].volume = musicVolume[1];
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SelectDifficulty(int i)
    {
        GMT.difficultyID = i;
    }
}
