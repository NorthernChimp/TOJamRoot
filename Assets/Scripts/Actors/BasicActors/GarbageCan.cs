using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCan : MonoBehaviour,Actor
{
    bool hasBeenCollected = false;
    public Sprite[] possibleSprites;
    SpriteRenderer render;
    public List<GameEvent> SetupActor()
    {
        List<GameEvent> temp = new List<GameEvent>();
        render = GetComponent<SpriteRenderer>();
        render.sprite = possibleSprites[GetRandomIntBetween(0, possibleSprites.Length - 1)];
        hasBeenCollected = false;
        transform.localScale = MainScript.brickScale * 4f;
        return temp;
    }
    int GetRandomIntBetween(int min, int max) { return (int)(Random.Range((float)min, (float)(max + 1))); }
    public List<GameEvent> UpdateActor(float timePassed)
    {
        List<GameEvent> temp = new List<GameEvent>();
        Vector3 diff = Player.instance.position - transform.position;diff.z = 0f;
        if(diff.magnitude < MainScript.brickWidth * 2f && !hasBeenCollected)
        {
            hasBeenCollected = true;
            MainScript.instance.ItemCollected();
            temp.Add(GameEvent.GetRemoveActorEvent(this));
        }
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
