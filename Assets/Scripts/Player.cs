using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rbody;
    [SerializeField]
    public PlayerSettings defaultSettings;
    PlayerSettings currentSettings;
    List<PlayerSettingsAffector> affectors;
    float timeSinceJump = 0f;
    bool grounded = false;
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contact = collision.GetContact(0).point;
        Vector3 diff = transform.position - contact;diff.z = 0f;diff = diff.normalized;
        if(diff == Vector3.up) 
        {
            grounded = true;
        }
    }
    void UpdatePlayerSettings(float timePassed)
    {
        currentSettings.CopySettings(defaultSettings);
        for(int i = 0; i < affectors.Count; i++)
        {
            PlayerSettingsAffector a = affectors[i];
            if (!a.UpdateAffector(timePassed))
            {
                ApplySettingsAffector(a);
            }
            else { affectors.RemoveAt(i);i--; }
        }
    }
    void ApplySettingsAffector(PlayerSettingsAffector a)
    {
        switch (a.GetTypeOfAffector())
        {
            case PlayerSettingsAffectorType.addToSpeed:currentSettings.AddSpeed(a.GetAmount()); break;
            case PlayerSettingsAffectorType.addToMaxSpeed:currentSettings.AddSpeed(a.GetAmount()); break;
        }
    }
    public void SetupPlayer()
    {
        affectors = new List<PlayerSettingsAffector>();
        transform.localScale = MainScript.brickScale;
        rbody = GetComponent<Rigidbody2D>();
        currentSettings = new PlayerSettings();
        defaultSettings = new PlayerSettings();
    }
    public List<GameEvent> UpdatePlayer(float timePassed)
    {
        List<GameEvent> temp = new List<GameEvent>();
        UpdatePlayerSettings(timePassed);
        rbody.velocity = Vector2.zero;
        float xSpeed = currentSettings.GetSpeed() * MainScript.brickWidth;
        float ySpeed = 0f;
        if (grounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                grounded = false;
                timeSinceJump = -3f;
            }
        }
        else
        {
            Debug.Log(timeSinceJump);
            ySpeed = MarioFunction(timeSinceJump) * MainScript.brickWidth;
            timeSinceJump += timeSinceJump;
        }
        rbody.velocity = new Vector2(xSpeed,ySpeed);
        if (transform.position.y < Screen.height * -0.005f + MainScript.brickWidth) { Vector3 v = transform.position;v.y = Screen.height * -0.005f + MainScript.brickWidth;transform.position = v; }
        return temp;
    }
    float MarioFunction(float f) { return Mathf.Pow(f, 2f) * -1f + 9f; }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PlayerSettings
{
        
        int maxHealth = 100; public int GetMaxHealth() { return maxHealth; }
        public void AddToMaxHealth(int i) { maxHealth += i; }
        float speed = 6.5f; public float GetSpeed() { return speed; }
        public void AddSpeed(float amt) { speed += amt; }
        float maxSpeed = 125f; public float GetMaxSpeed() { return maxSpeed; }
        public void AddToMaxSpeed(float amt) { maxSpeed += amt; }
        float fireRate = 1f; public float GetFireRate() { return fireRate; }
        public void CopySettings(PlayerSettings s)
        {
            maxHealth = s.GetMaxHealth();
            speed = s.GetSpeed();
            maxSpeed = s.GetMaxSpeed();
        }
    }
    public class PlayerSettingsAffector
    {
        PlayerSettingsAffectorType typeOfAffector; public PlayerSettingsAffectorType GetTypeOfAffector() { return typeOfAffector; }
        float amount; public float GetAmount() { return amount; }
        bool endsOverTime = false;
        Counter endCounter;
        public bool UpdateAffector(float timePassed)
        {
            if (endsOverTime) { return endCounter.UpdateCounter(timePassed); }
            return false;
        }
        public PlayerSettingsAffector(float amt, PlayerSettingsAffectorType typeOfAff, float time)
        {
            typeOfAffector = typeOfAff;
            amount = amt;
            if (time > 0f)
            {
                endsOverTime = true;
                endCounter = new Counter(time);
            }
        }
    }
    public enum PlayerSettingsAffectorType { addToSpeed, addToMultiplier, addToMaxSpeed, addToTimeHeldDownMultiplier, addToFireRate, negateDownwardMomentum }
