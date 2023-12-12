using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LangM
{
    public static int lang;//0 = PT; 1 = EN
}

public class LanguageManager : MonoBehaviour
{
    public void SetLang(int i)
    {
        LangM.lang = i;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SplashScreen");
    }
}
