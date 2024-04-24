using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField]
    bool isStart;

    [SerializeField]
    bool isPause;


    public bool IsStart { get { return isStart; } }
    public bool IsPause { get { return isPause; } }


    float currentTime;


    void Awake()
    {
        currentTime = 0.0f;
    }

    void Update()
    {
        _TimerHandler();
    }

    void _TimerHandler()
    {
        if (isStart && !isPause) {
            _Tick();
        }
    }

    void _Tick()
    {
        currentTime += Time.deltaTime;
    }

    public override string ToString()
    {
        var minute = (int)(currentTime / 60.0f);
        var seconds =  (int)(currentTime - (minute * 60.0f));

        var minuteString = (minute > 9) ? minute.ToString() : string.Format("0{0}", minute);
        var secondsString = (seconds > 9) ? seconds.ToString() : string.Format("0{0}", seconds);

        var result = string.Format("{0}:{1}", minuteString, secondsString);

        return result;
    }

    public void StartTick()
    {
        isStart = true;
        currentTime = 0.0f;
    }

    public void Pause(bool value = true)
    {
        isPause = value;
    }

    public void Stop()
    {
        isPause = false;
        isStart = false;
        currentTime = 0.0f;
    }
}
