using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    float maxSeconds;

    [SerializeField]
    bool isStart;

    [SerializeField]
    bool isPause;


    public bool IsStart { get { return isStart; } }
    public bool IsPause { get { return isPause; } }
    public bool IsFinished { get { return currentTime <= 0.0f; } }
    public float Seconds { get { return currentTime; } set { currentTime = value; } }
    public float MaxSeconds { get { return maxSeconds; } set { maxSeconds = value; } }


    float currentTime;
    

    void Awake()
    {
        currentTime = maxSeconds;
    }

    void Update()
    {
        _TimerHandler();
    }

    void _TimerHandler()
    {
        if (IsFinished) {
            Stop();
            return;
        }

        if (!isPause && isStart) {
            _Tick();
        }
    }

    void _Tick()
    {
        currentTime -= Time.deltaTime;
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

    public void CountDown()
    {
        currentTime = maxSeconds;
        isStart = true;
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
