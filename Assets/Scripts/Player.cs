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
    bool atBottom = false;
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contact = collision.GetContact(0).point;
        Vector3 diff = transform.position - contact;diff.z = 0f;diff = diff.normalized;
        if(diff == Vector3.up) 
        {
            transform.position = contact + diff.normalized * MainScript.brickWidth * 0.55f;
            grounded = true;
        }
        if(diff.y < 0f)
        {
            if(timeSinceJump < 0f) { timeSinceJump = 0f; }
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
            //Physics2D.Raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, MainScript.brickWidth * 0.55f, LayerMask.GetMask("collider"));
            Debug.DrawRay(transform.position, Vector3.down * MainScript.brickWidth * 0.55f); 
            //RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * MainScript.brickWidth * 0.501f, Vector2.down, MainScript.brickWidth * 5f);
            if (hit)
            {
                Debug.Log("stillgrounded");
                Vector3 pos = transform.position;
                pos.y = hit.point.y + MainScript.brickWidth * 0.55f;
                transform.position = pos;
                
            }
            else if(!atBottom)
            {
                grounded = false;
                timeSinceJump = 0f;
            }
            if (grounded)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    grounded = false;
                    atBottom = false;
                    timeSinceJump = -3f;
                }
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, MainScript.brickWidth * 0.55f, LayerMask.GetMask("collider"));
            //RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * MainScript.brickWidth * 0.501f, Vector2.down, MainScript.brickWidth * 5f);
            if (hit)
            {
                grounded = true;
                Vector3 pos = transform.position;
                pos.y = hit.point.y + MainScript.brickWidth * 0.55f;
                transform.position = pos;
            }
            if (!grounded)
            {
                ySpeed = MarioFunction(timeSinceJump) * MainScript.brickWidth * 1.35f;
                timeSinceJump += timePassed * 9f;
                if (timeSinceJump > 5f) { timeSinceJump = 5f; }
            }
        }
        rbody.velocity = new Vector2(xSpeed,ySpeed);
        if (transform.position.y < Screen.height * -0.005f + MainScript.brickWidth * 1.5f) { Vector3 v = transform.position;v.y = Screen.height * -0.005f + MainScript.brickWidth * 1.5f;transform.position = v; if (timeSinceJump >= 0f) { grounded = true; atBottom = true; } }
        else if(transform.position.y > Screen.height * 0.005f - MainScript.brickWidth) { Vector3 v = transform.position;v.y = Screen.height * 0.005f - MainScript.brickWidth; transform.position = v; }
        return temp;
    }
    float MarioFunction(float f) { return ((f * f) * -1f) + 9f; }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PlayerSettings
{
        
        int maxHealth = 100; public int GetMaxHealth() { return maxHealth; }
        public void AddToMaxHealth(int i) { maxHealth += i; }
        float speed = 10.20f; public float GetSpeed() { return speed; }
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
