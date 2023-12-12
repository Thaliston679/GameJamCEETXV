using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PoisonLine : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float gameTime;
    [SerializeField] float poisonTime;
    [SerializeField] bool poisonCollect;
    bool gameOver;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI ml;
    [SerializeField] GameObject gameOverMenu;

    [SerializeField] SpriteRenderer poisonBackground;
    float poisonBackgroundV;

    //Difficulty
    [SerializeField] float snakeMaxTime = 36000;

    void Update()
    {
        if (GMT.playerInGame)
        {
            transform.position = target.position;
            gameTime += Time.deltaTime * 1000f;
        }

        if (GMT.normalMode)
        {
            if (poisonCollect) poisonTime += Time.deltaTime * 1000f;
        }

        if(gameTime >= snakeMaxTime)
        {
            if (GMT.normalMode && !GMT.gargleMode)
            {
                Debug.Log($"{(int)((poisonTime / gameTime) * 100)}% de veneno coletado!");
                GMT.gargleMode = true;
                GMT.mlCollect = (int)(poisonTime / 10);
            }
            if (!GMT.normalMode && !gameOver)
            {
                gameOver = true;
                Debug.Log("Fim de Jogo!...");
                gameOverMenu.SetActive(true);
                //SceneManager.LoadScene("SplashScreen");
            }
        }

        if (poisonCollect) poisonBackgroundV += Time.deltaTime / 10;
        poisonBackground.color = new(0.15f, 0.15f, 0.15f, Mathf.Clamp01(poisonBackgroundV-0.25f)-0.15f);


        float remainingTime = snakeMaxTime - gameTime;
        int minutes = (int)(remainingTime / 60000);
        int seconds = (int)((remainingTime / 1000) % 60);
        if(minutes < 0) minutes = 0; if(seconds < 0) seconds = 0;
        string gameTimeText = string.Format("{0:00}:{1:00}", minutes, seconds);
        timer.text = gameTimeText;
        ml.text = $"{(int)(poisonTime/10)} ml";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boca"))
        {
            poisonCollect = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boca"))
        {
            poisonCollect = false;
        }
    }

    public void SetDifficultyTime(float value)
    {
        snakeMaxTime = (int)value * 1000;
        GMT.snakeMaxTime = snakeMaxTime;
    }
}
