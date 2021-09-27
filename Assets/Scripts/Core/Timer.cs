using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public enum State
    {
        Wait, Runing, Stoped
    }

    float timer = 0;
    float endSec = 0;
    public event Action timerStoped;
    public event Action<float, float> timerChanged;
    public State state = State.Wait;
    public void SetTimer(float endSec, Action<float, float> timerChangeAction, Action timerStopAction)
    {
        this.endSec = Math.Max(1f, endSec);
        timerStoped += timerStopAction;
        timerChanged += timerChangeAction;
    }

    public void BeginTick()
    {
        timer = 0;
        state = State.Runing;
    }

    public void Stop()
    {
        timer = 0;
        state = State.Stoped;
        timerStoped?.Invoke();
    }

    public void Pause()
    {
        state = State.Wait;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Runing)
        {
            timer += Time.deltaTime;
            timerChanged?.Invoke(timer, endSec);
            if (timer >= endSec)
            {
                Stop();
            }
        }
    }
}
