using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Timer
{
    public enum Behavior
    {
        NONE = 0, //Resets
        NO_RESET = 1 //Has to be manually reset
    }

    [SerializeField] float lowerMaxValue;
    [SerializeField] float upperMaxValue; //use as threshold if max is supposed to become random everytime
    float timer;
    Action onDone;
    Behavior behavior;

    public Timer(float maxValue, Action onDone, float upperMaxValue = 0, Behavior behavior = Behavior.NONE)
    {
        timer = 0;
        lowerMaxValue = maxValue;
        this.upperMaxValue = upperMaxValue;
        this.onDone = onDone;
        this.behavior = behavior;
    }

    public bool Tick()
    {
        timer += Time.deltaTime;
        if(timer >= lowerMaxValue)
        {
            onDone();
            if (behavior != Behavior.NO_RESET)
            {
                timer = timer - lowerMaxValue;
                SetRandomMaxValue();
            }
            return true;
        }
        return false;
    }
    public void Reset()
    {
        timer = 0;
        SetRandomMaxValue();
    }
    public void Extend(float value)
    {
        lowerMaxValue += value;
    }
    void SetRandomMaxValue()
    {
        if (upperMaxValue > lowerMaxValue)
        {
            lowerMaxValue = Random.Range(lowerMaxValue, upperMaxValue);
        }
    }
    public void Fill()
    {
        timer = lowerMaxValue;
    }
    public bool IsDone()
    {
        return timer >= lowerMaxValue;
    }
}
