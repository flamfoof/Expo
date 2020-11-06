using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] private string timerMinutes, timerSeconds, timerSeconds100;

    private float startTime;
    private float stopTime;
    private float timerTime;
    private bool isRunning = false;

    public delegate void SendAnalytics();
    public static event SendAnalytics sendAnalytics;

    void Start()
    {
        TimerReset();
        TimerStart();
    }

    public void TimerStart()
    {
        if (!isRunning)
        {
            Debug.Log("START");
            isRunning = true;
            startTime = Time.time;
        }
    }

    public void TimerStop()
    {
        if (isRunning)
        {
            Debug.Log("STOP");
            isRunning = false;
            stopTime = timerTime;
        }
    }

    public void TimerReset()
    {
        Debug.Log("RESET");
        stopTime = 0;
        isRunning = false;
        timerMinutes = timerSeconds = timerSeconds100 = "00";
    }

    void Update()
    {
        timerTime = stopTime + (Time.time - startTime);
        int minutesInt = (int)timerTime / 60;
        int secondsInt = (int)timerTime % 60;
        int seconds100Int = (int)(Mathf.Floor((timerTime - (secondsInt + minutesInt * 60)) * 100));

        if (isRunning)
        {
            timerMinutes = (minutesInt < 10) ? "0" + minutesInt : minutesInt.ToString();
            timerSeconds = (secondsInt < 10) ? "0" + secondsInt : secondsInt.ToString();
            timerSeconds100 = (seconds100Int < 10) ? "0" + seconds100Int : seconds100Int.ToString();

            if (minutesInt >= 5)
            {
                TimerReset();
                sendAnalytics?.Invoke();
                TimerStart();
            }
        }
    }
}