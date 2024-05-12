using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCan : MonoBehaviour,Actor
{
    public List<GameEvent> SetupActor()
    {
        List<GameEvent> temp = new List<GameEvent>();
        transform.localScale = MainScript.brickScale;
        return temp;
    }
    public List<GameEvent> UpdateActor(float timePassed)
    {
        List<GameEvent> temp = new List<GameEvent>();
        if(MainScript.IsPointLeftOfCamera(transform.position + Vector3.right * Screen.width * 0.001f))
        {
            temp.Add(GameEvent.GetRemoveActorEvent(this));
        }
        return temp;
    }
    public Transform GetTransform()
    {
        
        return transform;
    }

}
