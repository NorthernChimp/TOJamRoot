using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter
{
    float endTime;
    float currentTime;
    bool hasFinished = false;public bool HasFinished() { return hasFinished; }
    bool ends = true;
    public Counter(float timeToEnd)
    {
        endTime = timeToEnd;
        if(timeToEnd == 0f) { ends = false; }
        currentTime = 0f;
    }
    public void ResetCounter() { hasFinished = false; currentTime = 0f; }
    public bool UpdateCounter(float timePassed)
    {
        currentTime += timePassed;
        if(currentTime >= endTime && ends) { currentTime = endTime; hasFinished = true; }
        return hasFinished;
    }
    public float GetPercentDone() { return (currentTime / endTime); }
}
