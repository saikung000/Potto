using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    [SerializeField]
    Text lblTimer;


    Timer timer;


    void Awake()
    {
        timer = GetComponent<Timer>();
    }

    void Start()
    {
        timer.CountDown();
    }

    void Update()
    {
        _UIHandler();
    }

    void _UIHandler()
    {
        _UpdateUI();
    }

    void _UpdateUI()
    {
        lblTimer.text = timer.ToString();
    }
}
