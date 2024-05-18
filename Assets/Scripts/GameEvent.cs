using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent 
{
    GameEventType ty; public GameEventType GetEventType() { return ty; }
    int amt; public int GetAmt() { return amt; }
    int dmg; public int GetDmg() { return dmg; }
    int reference; public int GetReference() { return reference; }
    float f; public float GetFloat() { return f; }
    float f2; public float GetFloat2() { return f2; }
    bool b; public bool GetBool() { return b; }
    string prefabName; public string GetPrefabName() { return prefabName; }
    Actor a; public Actor GetActor() { return a; }

    PlayerSettingsAffector affector;public PlayerSettingsAffector GetAffector() { return affector; }
    Vector3 pos; public Vector3 GetPos() { return pos; }
    GameEvent(GameEventType pe,int amou)
    {
        ty = pe;
        amt = amou;
    }
    GameEvent(GameEventType pe,PlayerSettingsAffector aff)
    {
        ty = pe;
        affector = aff;
    }
    public static GameEvent GetAffectorEvent(PlayerSettingsAffector a){return new GameEvent(GameEventType.applyStatusAffector, a);}
    public GameEvent(GameEventType pe, Actor act) { a = act;ty = pe; }
    public static GameEvent GetRemoveActorEvent(Actor act) { return new GameEvent(GameEventType.removeActor, act); }
    public static GameEvent GetCreateColliderEvent(Vector3 posit,float width,float height,bool boo) { return new GameEvent(GameEventType.createCollider, width, height, boo,posit); }
    public GameEvent(GameEventType pe,float width,float height,bool boo,Vector3 posit)
    {
        pos = posit;
        b = boo;
        ty = pe;
        f = width;
        f2 = height;
    }
    public static GameEvent CreateActorEvent(string prefab,Vector3 pos) { return new GameEvent(GameEventType.createActor, prefab,pos); }
    GameEvent(GameEventType pe,string s,Vector3 p)
    {
        pos = p;
        prefabName = s;
        ty = pe;
    }
}
public enum GameEventType { none,createActor,createEffect,removeActor,createCollider,playSound,applyStatusAffector}
