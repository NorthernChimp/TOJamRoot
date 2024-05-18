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
    public static Transform instance;
    float timeSinceJump = 0f;
    bool grounded = false;
    bool atBottom = false;
    bool holdingSpace = false;
    Animator anim;
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contact = collision.GetContact(0).point;
        Vector3 diff = transform.position - contact;diff.z = 0f;diff = diff.normalized;
        if(diff == Vector3.up) 
        {
            transform.position = contact + diff.normalized * MainScript.brickWidth * 2.0f;
            grounded = true;
            anim.Play("Player_Run");
        }
        else
        {
            transform.position = contact + diff.normalized * MainScript.brickWidth * 3.5f + Vector3.up * MainScript.brickWidth * 0.5f;
            grounded = false;
            atBottom = false;
            if (diff.normalized.y > 0.75f && timeSinceJump > 3f)
            {
                
                timeSinceJump = 3f;
                anim.Play("Player_JumpFall");
                
            }
            else
            {
            }
        }
        if(diff.y < 0f)
        {
            if(timeSinceJump < 3f) { timeSinceJump = 3f; anim.Play("Player_JumpFall"); }
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
        instance = transform;
        anim = GetComponent<Animator>();
        affectors = new List<PlayerSettingsAffector>();
        transform.localScale = MainScript.brickScale;
        rbody = GetComponent<Rigidbody2D>();
        currentSettings = new PlayerSettings();
        defaultSettings = new PlayerSettings();
        anim.Play("Player_Run");
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
            //Debug.Log("grounded");
            if(!atBottom)
            {
                //Physics2D.Raycast
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, MainScript.brickWidth * 2.5f, LayerMask.GetMask("collider"));
                //Debug.DrawRay(transform.position, Vector3.down * MainScript.brickWidth * 0.55f); 
                //RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * MainScript.brickWidth * 0.501f, Vector2.down, MainScript.brickWidth * 5f);
                if (hit)
                {
                    //Debug.Log(hit.collider.gameObject);
                    Vector3 pos = transform.position;
                    pos.y = hit.point.y + MainScript.brickWidth * 2f;
                    transform.position = pos;

                }
                else
                {
                    //Debug.Log("grounded is false");
                    grounded = false;
                    timeSinceJump = 3f;
                    anim.Play("Player_JumpFall");
                }
                
            }
            if (grounded)
            {
                //Debug.Log("in here at " + Time.time + " and " + holdingSpace);
                if (holdingSpace)
                {
                    grounded = false;
                    atBottom = false;
                    timeSinceJump = -3f;
                    anim.Play("Player_JumpStart");
                }
            }
        }
        else
        {
            //Debug.Log("not grounded");
            if(timeSinceJump > 3f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, MainScript.brickWidth * 3f, LayerMask.GetMask("collider"));
                //RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * MainScript.brickWidth * 0.501f, Vector2.down, MainScript.brickWidth * 5f);
                if (hit)
                {
                    //Debug.Log(hit.collider.gameObject.name);
                    anim.Play("Player_Run");
                    grounded = true;
                    Vector3 pos = transform.position;
                    pos.y = hit.point.y + MainScript.brickWidth * 2f;
                    transform.position = pos;
                }
            }
            if (!grounded)
            {
                //Debug.Log(timeSinceJump);
                ySpeed = MarioFunction(timeSinceJump) * MainScript.brickWidth * 2.135f;
                timeSinceJump += timePassed * 12f;
                if (timeSinceJump > 6f) { timeSinceJump = 6f; }
            }
        }
        rbody.velocity = new Vector2(xSpeed,ySpeed);
        if (transform.position.y < Screen.height * -0.005f + MainScript.brickWidth * 3.25f) 
        { 
            Vector3 v = transform.position;v.y = Screen.height * -0.005f + MainScript.brickWidth * 3.25f;
            transform.position = v;
            if (timeSinceJump >= 3f) 
            {
                grounded = true; 
                atBottom = true; 
                anim.Play("Player_Run"); 
            } 
        }
        else if(transform.position.y > Screen.height * 0.005f - MainScript.brickWidth) 
        { 
            Vector3 v = transform.position;v.y = Screen.height * 0.005f - MainScript.brickWidth; 
            transform.position = v; 
        }
        return temp;
    }
    float MarioFunction(float f) { return (((f ) * (f )) * -1f) + 9f; }
    // Update is called once per frame
    void Update()
    {
        //holdingSpace = Input.GetKey(KeyCode.Space);
        holdingSpace = Input.touchCount > 0;
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
