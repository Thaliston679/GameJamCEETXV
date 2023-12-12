using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryPlay : MonoBehaviour
{
    [SerializeField] Player player;

    public void PlayGame()
    {
        player.PlayGame();
    }

    public void WinGame()
    {
        player.WinGame();
    }
}
