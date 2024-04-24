using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public delegate void Func();

    public static event Func OnGameOver;
    public static event Func OnGameStart;

    public static event Func OnEnterCinematic;
    public static event Func OnExitCinematic;

    public static GameController instance = null;
    public static bool isGameStart = false;


    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }
	public void GameStart(float waitStart){
	
		StartCoroutine (WaitStartGame (waitStart));
	}

    public void GameStart()
    {
        isGameStart = true;

        if (OnGameStart != null) {
            OnGameStart();
        }
    }

	public static void GameOver()
    {
        isGameStart = false;

        if (OnGameOver != null) {
            OnGameOver();
        }
    }

    public static void EnterCinematic(bool value)
    {
        if (value) {
            if (OnEnterCinematic != null) {
                OnEnterCinematic();
            }
        }
        else {
            if (OnExitCinematic != null) {
                OnExitCinematic();
            }
        }
    }

	IEnumerator WaitStartGame(float wait)
	{
		yield return new WaitForSeconds (wait);
		GameStart ();
	}
}
