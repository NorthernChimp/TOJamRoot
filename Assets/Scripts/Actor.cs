using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface Actor
{
    // Start is called before the first frame update
    public List<GameEvent> DealDamage(int amt, Vector3 dir) { return new List<GameEvent>(); }
    public bool TakesDamage() { return false; }//to check if the actor takes damage like a badguy, or not like a hoop
    public bool DestroysBullet() { return false; }//if you do take damage you can choose not to destroy the bullet, like if an enemy is dead his corpse eats up bullets that could hit other bad guys, this allows those bullets to shoot through and hit that enemy
    public List<GameEvent> SetupActor() { return new List<GameEvent>(); }

    public Transform GetTransform();
    public List<GameEvent> UpdateActor(float timePassed) { return new List<GameEvent>(); }
    public void AccelerateActor(Vector3 push) { }
    public bool CanBeAccelerated() { return false; }
    //public List<GameEvent> CleanupActor();
}