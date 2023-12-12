using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using TMPro;

public class Player : MonoBehaviour
{
    //Partes
    public GameObject[] playerParts; //0 body, 1 head, 2 arm

    //Valor maximo da rotacao Z possivel
    public float[] maxRot; //-30 a 30

    //Forca de inclinacao para quando o objeto ja está caindo
    public float inclForce; //-2 a 2

    //Forca que o player aplica de inclinacao
    public float playerForce;

    //Forca fantasma que e aplicada de vez em quando e com um valor aleatorio
    public float[] ghostForce;
    public float[] targetGhostForce;
    public float[] ghostLastSide;
    public float[] ghostTimeCurrent;
    public float[] ghostTimeTarget;

    //Rotacao que sera passada para o objeto como eixo Z
    public float[] finalRot;

    //Game Over
    bool fallMode;
    bool sideFall;
    float fallSpeed = 1;
    float respawnTime = 0;
    [SerializeField] GameObject respawnObj;
    bool toRespawn;
    float deathValue = 10;
    bool nextRespawnSide;
    [SerializeField] SpriteRenderer respawnPoison;

    //Gargle Mode
    bool gargleMode;
    bool gargleModeAct;
    bool gargleModeAnim;
    [SerializeField] GameObject gargleObj;
    [SerializeField] GameObject normalGameObj;
    [SerializeField] SpriteRenderer gargleGirl;
    [SerializeField] Sprite[] gargleSide;
    bool gargleSideR;
    float gargleValue = 5;
    [SerializeField] float gargleMaxValue;
    bool gargleFinish;
    float poisonTime;

    [SerializeField] SpriteRenderer garglePot;

    //Timer & ML
    [SerializeField] GameObject timerObj;
    [SerializeField] GameObject mlObj;

    //WinScreen
    [SerializeField] GameObject winScreen;
    [SerializeField] TextMeshProUGUI infoScore;
    [SerializeField] TextMeshProUGUI notaFinal;

    //PauseScreen
    [SerializeField] GameObject pauseScreen;

    //Difficulty
    float inclForceD;
    float playerForceD;
    float ghostForceD;
    float deathValueMultD;
    float deathValueDecreaseD;
    float gargleValueMultD;
    float gargleValueDecreaseD;
    [SerializeField] PoisonLine poisonLine;
    int potMaxTimeD;
    string nameD;

    //SnakeChange
    [SerializeField] SpriteRenderer snakeHandBoySR;
    [SerializeField] Sprite[] snakeHandBoyS;
    [SerializeField] SpriteRenderer gargleRefillSR;
    [SerializeField] Sprite[] gargleRefillS;
    [SerializeField] SpriteRenderer potionSR;
    [SerializeField] Sprite[] potionS;
    [SerializeField] Animator snakeHandA;
    [SerializeField] Animator gargleSpew;


    private void Start()
    {
        GMT.normalMode = false;
        GMT.playerInGame = false;
        GMT.gargleMode = false;
        SetRandomSnake();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }

        if(GMT.gargleMode && !gargleMode)
        {
            gargleMode = true;
        }

        if (!gargleModeAct && GMT.playerInGame)
        {
            if (!fallMode)
            {
                if (finalRot[2] > 0)
                {
                    inclForce += Time.deltaTime * 20;
                }
                else if (finalRot[2] < 0)
                {
                    inclForce -= Time.deltaTime * 20;
                }
                inclForce = Mathf.Clamp(inclForce, -15, 15);

                //Define a forca de inclinacao aplicada pelo player
                float playerValue = GMT.HorizontalInput();
                if (playerValue != 0) playerForce += (-playerValue * Time.deltaTime * 100);
                else playerForce += ((playerForce > 1 ? -1 : playerForce < -1 ? 1 : 0) * Time.deltaTime * 100);
                playerForce = Mathf.Clamp(playerForce, -100, 100);

                //Aplica a forca fantasma
                ghostTimeCurrent[2] += Time.deltaTime;
                ghostForce[2] = Mathf.Lerp(ghostForce[2], targetGhostForce[2], Mathf.Clamp01(ghostTimeCurrent[2] / ghostTimeTarget[2]));

                //Aplica a forca fantasma do corpo nao controlavel
                for (int i = 0; i < 2; i++)
                {
                    ghostTimeCurrent[i] += Time.deltaTime;
                    ghostForce[i] = Mathf.Lerp(ghostForce[i], targetGhostForce[i], Mathf.Clamp01(ghostTimeCurrent[i] / ghostTimeTarget[i]));

                    finalRot[i] += ghostForce[i] * Time.deltaTime;
                    finalRot[i] = Mathf.Clamp(finalRot[i], -maxRot[i], maxRot[i]);
                }

                if (playerParts[0].transform.position.y > -0.5f) finalRot[2] += ((inclForce * inclForceD) + (playerForce * playerForceD) + (ghostForce[2] * ghostForceD)) * Time.deltaTime;
                else finalRot[2] = 0;
                finalRot[2] = Mathf.Clamp(finalRot[2], -maxRot[2], maxRot[2]);

                playerParts[0].transform.rotation = Quaternion.Euler(0, 0, finalRot[0]);
                playerParts[1].transform.rotation = Quaternion.Euler(0, 0, finalRot[1]);
                playerParts[2].transform.rotation = Quaternion.Euler(0, 0, finalRot[2]);

                if (finalRot[2] > 40 || finalRot[2] < -40)
                {
                    respawnTime = 0;
                    toRespawn = false;
                    fallMode = true;
                    GMT.normalMode = false;
                    sideFall = finalRot[2] > 0 ? true : false;
                    Debug.Log("GAME OVER!!!");
                }

                if (playerParts[0].transform.position.y < 0)
                {
                    playerParts[0].transform.position += Vector3.up * Time.deltaTime * 8;
                }
            }

            if (fallMode)
            {
                if (respawnTime < 1) FallMode();
                if (respawnTime >= 0.5f && !toRespawn)
                {
                    respawnObj.SetActive(true);
                    deathValue = 0;
                    toRespawn = true;
                    for (int i = 0; i < 3; i++)
                    {
                        playerParts[0].transform.position = new(0, -5, 1);
                        playerParts[i].transform.rotation = Quaternion.Euler(0, 0, 1);
                        finalRot[i] = 0;
                    }
                    playerForce = 0;
                }
                if (toRespawn)
                {
                    if (deathValue > 20)
                    {
                        respawnObj.SetActive(false);
                        fallMode = false;
                        GMT.normalMode = true;
                    }

                    if(deathValue > 0) deathValue -= Time.deltaTime * deathValueDecreaseD;
                    if (nextRespawnSide && GMT.HorizontalInput() > 0)
                    {
                        nextRespawnSide = false;
                        deathValue += deathValueMultD;
                    }
                    if (!nextRespawnSide && GMT.HorizontalInput() < 0)
                    {
                        nextRespawnSide = true;
                        deathValue += deathValueMultD;
                    }

                    respawnPoison.size = new(1, 1 - Mathf.Clamp01(deathValue / 20));
                }
            }
        }

        if(gargleMode && !fallMode && !gargleModeAct && !gargleModeAnim)
        {
            gargleModeAnim = true;
            GarglePlayAnim();
        }

        if(gargleMode && gargleModeAct)
        {
            GargleUpdate();
        }
    }

    void GhostForce()
    {
        ghostTimeCurrent[2] = 0;
        ghostLastSide[2] *= -1;
        switch (GMT.difficultyID)
        {
            case 0://Facil
                ghostTimeTarget[2] = Random.Range(2f, 3f);
                break;
            case 1://Normal
                ghostTimeTarget[2] = Random.Range(1f, 3f);
                break;
            case 2://Dificil
                ghostTimeTarget[2] = Random.Range(1f, 2f);
                break;
            case 3://Insano
                ghostTimeTarget[2] = Random.Range(0.5f, 1.5f);
                break;
            default:
                ghostTimeTarget[2] = Random.Range(2f, 4f);
                break;

        }
        targetGhostForce[2] = Random.Range(10, 15) * ghostLastSide[2];
        Invoke(nameof(GhostForce), ghostTimeTarget[2]);
    }

    void HeadForce()
    {
        ghostTimeCurrent[1] = 0;
        ghostLastSide[1] *= -1;
        ghostTimeTarget[1] = Random.Range(1f, 3f);
        targetGhostForce[1] = Random.Range(2, 10) * ghostLastSide[1];
        Invoke(nameof(HeadForce), ghostTimeTarget[1]);
    }

    void BodyForce()
    {
        ghostTimeCurrent[0] = 0;
        ghostLastSide[0] *= -1;
        ghostTimeTarget[0] = Random.Range(2f, 5f);
        targetGhostForce[0] = Random.Range(2, 10) * ghostLastSide[0];
        Invoke(nameof(BodyForce), ghostTimeTarget[0]);
    }

    void FallMode()
    {
        //Tremer tela...
        fallSpeed += Time.deltaTime * 2;
        playerParts[0].transform.position += Vector3.down * Time.deltaTime * 5;
        respawnTime += Time.deltaTime;
        if (sideFall)finalRot[0] += fallSpeed * Time.deltaTime * 50;
        if(!sideFall)finalRot[0] -= fallSpeed * Time.deltaTime * 50;
        playerParts[0].transform.rotation = Quaternion.Euler(0, 0, finalRot[0]);
    }

    public void GarglePlay()
    {
        gargleObj.SetActive(true);
        normalGameObj.SetActive(false);
        gargleModeAct = true;
    }

    public void GarglePlayAnim()
    {
        GetComponent<Animator>().SetTrigger("ToGargle");
        timerObj.GetComponent<Animator>().SetTrigger("Hide");
        mlObj.GetComponent<Animator>().SetTrigger("Hide");
    }

    void GargleUpdate()
    {
        if(gargleValue > 5 && gargleValue < gargleMaxValue+1) gargleValue -= Time.deltaTime * gargleValueDecreaseD;

        if(!gargleFinish) poisonTime += Time.deltaTime * 1000;

        if (gargleSideR && GMT.HorizontalInput() > 0)
        {
            gargleSideR = false;
            gargleGirl.sprite = gargleSide[0];
            gargleValue += gargleValueMultD;
        }
        if (!gargleSideR && GMT.HorizontalInput() < 0)
        {
            gargleSideR = true;
            gargleGirl.sprite = gargleSide[1];
            gargleValue += gargleValueMultD;
        }
        if(gargleValue > gargleMaxValue && !gargleFinish)
        {
            gargleFinish = true;
            Debug.Log("YOU WIN!!!");

            int minutes = (int)(poisonTime / 60000);
            int seconds = (int)((poisonTime / 1000) % 60);
            int milliseconds = (int)(poisonTime % 1000);
            if (minutes < 0) minutes = 0; if (seconds < 0) seconds = 0; if(milliseconds < 0) milliseconds = 0;
            string gameTimeText = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
            GMT.poisonTime = gameTimeText;
            GMT.poisonTimeScore = (int)poisonTime;

            gargleObj.GetComponent<Animator>().SetTrigger("Finish");
            gargleGirl.GetComponent<Animator>().enabled = true;
            gargleSpew.SetInteger("ID", GMT.snakeID);
        }

        float potVsize = Mathf.Clamp(((gargleValue / gargleMaxValue) * 0.75f), 0, 0.77f);
        garglePot.size = new(0.2f, potVsize);
    }

    public void PlayGame()
    {
        SetDifficultyValues();
        GhostForce();
        BodyForce();
        HeadForce();
        GMT.normalMode = true;
        GMT.playerInGame = true;

        timerObj.SetActive(true);
        mlObj.SetActive(true);
    }

    void SetDifficultyValues()
    {
        switch (GMT.difficultyID)
        {
            case 0://Facil
                inclForceD = 1f;
                playerForceD = 1.25f;
                ghostForceD = 1f;
                deathValueMultD = 3f;
                deathValueDecreaseD = 8f;
                gargleValueMultD = 2.5f;
                gargleValueDecreaseD = 10f;//10
                poisonLine.SetDifficultyTime(Random.Range(20, 30));
                potMaxTimeD = 5000;
                nameD = LangM.lang == 0 ? "Fácil" : "Easy";
                break; 
            case 1://Normal
                inclForceD = 1.5f;
                playerForceD = 3f;
                ghostForceD = 1.5f;
                deathValueMultD = 2.25f;
                deathValueDecreaseD = 11f;
                gargleValueMultD = 2.25f;
                gargleValueDecreaseD = 12.5f;//6
                poisonLine.SetDifficultyTime(Random.Range(30, 50));
                potMaxTimeD = 5000;
                nameD = LangM.lang == 0 ? "Normal" : "Normal";
                break;
            case 2://Dificil
                inclForceD = 3f;
                playerForceD = 8f;
                ghostForceD = 3f;
                deathValueMultD = 2f;
                deathValueDecreaseD = 12f;
                gargleValueMultD = 2f;
                gargleValueDecreaseD = 14f;//7
                poisonLine.SetDifficultyTime(Random.Range(50, 70));
                potMaxTimeD = 7000;
                nameD = LangM.lang == 0 ? "Difícil" : "Hard";
                break;
            case 3://Insano
                inclForceD = 5f;
                playerForceD = 10f;
                ghostForceD = 4.5f;
                deathValueMultD = 1.75f;
                deathValueDecreaseD = 13f;
                gargleValueMultD = 1.75f;
                gargleValueDecreaseD = 15f;//8
                poisonLine.SetDifficultyTime(Random.Range(70, 90));
                potMaxTimeD = 8000;
                nameD = LangM.lang == 0 ? "Insano" : "Insane";
                break;
            default://Normal
                inclForceD = 1.5f;
                playerForceD = 3f;
                ghostForceD = 1.5f;
                deathValueMultD = 2.25f;
                deathValueDecreaseD = 11f;
                gargleValueMultD = 2.25f;
                gargleValueDecreaseD = 12.5f;//6
                poisonLine.SetDifficultyTime(Random.Range(30, 50));
                potMaxTimeD = 5000;
                nameD = LangM.lang == 0 ? "Normal" : "Normal";
                break;
        }
        
    }

    public void WinGame()
    {
        string cp = LangM.lang == 0 ? "Veneno Coletado" : "Collected Poison";
        string pt = LangM.lang == 0 ? "Tempo de Preparo" : "Preparation Time";
        string df = LangM.lang == 0 ? "Dificuldade" : "Difficulty";
        winScreen.SetActive(true);
        infoScore.text = $"{cp}: {GMT.mlCollect} ml / {GMT.snakeMaxTime/10} ml\n  {pt}: {GMT.poisonTime}\n   {df}: {nameD}";
        notaFinal.text = CalculateScore();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        GMT.normalMode = false;
        GMT.playerInGame = false;
        GMT.gargleMode = false;
        SceneManager.LoadScene("SplashScreen");
    }

    string CalculateScore()
    {
        string nota = "";
        int notaI = 0;
        int pt = GMT.mlCollect;
        int pts = GMT.poisonTimeScore;
        notaI = (pt * 1000) / (int)GMT.snakeMaxTime;
        if (pts > potMaxTimeD) notaI -= (pts - potMaxTimeD) / 1000;
        if (pts < potMaxTimeD) notaI += (potMaxTimeD - pts) / 1000;

        Debug.Log(pt);
        Debug.Log(pts);
        Debug.Log(notaI);
        Debug.Log(GMT.mlCollect);
        Debug.Log(GMT.poisonTimeScore);

        if(notaI > 98)
        {
            nota = "SSS";
        }
        else if (notaI > 95) 
        {
            nota = "SS";
        }
        else if (notaI > 90)
        {
            nota = "S";
        }
        else if (notaI > 80)
        {
            nota = "A";
        }
        else if (notaI > 70)
        {
            nota = "B";
        }
        else if (notaI > 60)
        {
            nota = "C";
        }
        else if (notaI > 50)
        {
            nota = "D";
        }
        else if (notaI > 40)
        {
            nota = "E";
        }
        else
        {
            nota = "F";
        }

        return nota;
    }

    public void PauseGame()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
        else if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
    }

    void SetRandomSnake()
    {
        GMT.snakeID = Random.Range(0, 6);
        snakeHandBoySR.sprite = snakeHandBoyS[GMT.snakeID];
        gargleRefillSR.sprite = gargleRefillS[GMT.snakeID];
        potionSR.sprite = potionS[GMT.snakeID];
        snakeHandA.SetInteger("ID", GMT.snakeID);
    }
}
