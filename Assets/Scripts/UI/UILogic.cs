using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILogic : MonoBehaviour
{
    public void GameStart()
    {
        GameController.isGameStart = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
