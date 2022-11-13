using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : Singleton<TitleManager>
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private void OnEnable()
    {
        Cursor.visible = true;
        textMeshProUGUI.text = "Á¡¼ö\n" + PlayerPrefs.GetInt("Score", 0);
    }
    public void GameStart()
    {
        SceneManager.LoadScene("InGame");
    }
    public void GameHelp()
    {
        SceneManager.LoadScene("StoryCutScene");
    }
    public void GameQuit()
    {
        Application.Quit();
    }
}
