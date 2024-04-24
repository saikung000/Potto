using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClock : MonoBehaviour
{
    [SerializeField]
    Text lblClock;

	[SerializeField]
	Text endTextClock;


    Clock clock;


    void Awake()
    {
        clock = GetComponent<Clock>();
        _Subscribe_Events();
    }

    void Update()
    {
        _UIHandler();
    }

    void Destroy()
    {
        _UnSubscribe_Events();
    }

    void _UIHandler()
    {
        _UpdateUI();
    }

    void _UpdateUI()
    {
        lblClock.text = clock.ToString();
    }

    void _Subscribe_Events()
    {
        GameController.OnGameStart += OnGameStart;
		GameController.OnGameOver += () => 
		{
			clock.Pause();
			endTextClock.text =  clock.ToString();
		};
    }

    void _UnSubscribe_Events()
    {
        GameController.OnGameStart -= OnGameStart;
    }

    void OnGameStart()
    {
        clock.Pause(false);
        clock.StartTick();
    }
}
