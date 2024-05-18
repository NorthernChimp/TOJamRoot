using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Required for manipulating UI elements

public class MainScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform background;
    public RectTransform handle;
    public DialogBox dialogBox;
    public static float brickWidth;
    public SpriteRenderer goat;
    public SpriteRenderer tojam;
    int dialogBoxRef = 0;
    public string[] headerText;
    public SpriteRenderer mainMenu;
    public string[] bodyText;
    public static Vector3 brickScale;
    public Player player;
    GameObject tile;
    List<List<Tile>> tileLayout;
    List<RunnerSegment> segments;
    List<Actor> actors = new List<Actor>();
    List<ColliderScript> colliders;
    public static MainScript instance;
    public ParallaxEffect[] backgrounds;
    public AudioManager audio;
    bool gamePaused = true;
    int currentSegmentXRef = 0;//the segment reference for the last tile pushed to the furthest right in the game

    //UI Related Stuff
    public Slider progressBar;  // Reference to the progress bar slider
    private int totalCollectibles = 10;  // Set this to your total number of collectible items
    private int collectedItems = 0;
    public static bool playMusic = true;
    public static bool playSound = true;
    void Start()
    {
        SetupGame();
        
    }
    void SetupObjectPool()          //the object pool, which is a completely seperate script is used to avoid instantiating which wastes alot of processing power.
    {                               //basically, rather than creating a game object we create alot of them before we need any and deactivate them.
                                    //then when we want to create the gameobject we take one that we already made and activate it, which is much less processor intense
        List<GameObject> objectsToPurge = new List<GameObject>();
        for (int i = 0; i < 9; i++)
        {
            objectsToPurge.Add(ObjectPool.SpawnObjectFromName("GarbageCan", Vector3.zero, Quaternion.identity));
        }
        for (int i = 0; i < 15; i++)
        {
            objectsToPurge.Add(ObjectPool.SpawnObjectFromName("BoxCollider", Vector3.zero, Quaternion.identity));
        }
        for (int i = 0; i < 15; i++)
        {
            objectsToPurge.Add(ObjectPool.SpawnObjectFromName("CircleCollider", Vector3.zero, Quaternion.identity));
        }
        while (objectsToPurge.Count > 0)
        {
            ObjectPool.ReturnObjectToPool(objectsToPurge[0]);
            objectsToPurge.RemoveAt(0);
        }
    }
    void StartGame()
    {
        gamePaused = false;
        mainMenu.enabled = false;
        tojam.enabled = false;
        goat.enabled = false;
        dialogBox.SetHeader("Welcome to Root!");
        dialogBox.SetBody("Collect Garbage to make a better world!");
        audio.StopMenuMusic();
        audio.PlayMusic();
    }
    void SetupGame()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        background.transform.localScale = new Vector3((Screen.width * 0.01f) / 5.76f, (Screen.height * 0.01f) / 3.24f, 1f);
        RectTransform rect = progressBar.GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,(Screen.height * 0.15f));
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,(Screen.height * 0.8f));
        handle.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,Screen.height * 0.15f);
        float menuHeight = Screen.height * 0.008f;
        mainMenu.transform.localScale = new Vector3(menuHeight / 9.82f, menuHeight / 9.82f, 1f);
        if(PlayerPrefs.GetInt("PlayMusic") == 1) { playMusic = false; }
        if(PlayerPrefs.GetInt("PlaySound") == 1) { playSound = false; }
        if (playMusic) { audio.PlayMenuMusic(); }
        instance = this;
        colliders = new List<ColliderScript>();
        SetupObjectPool();
        float screenHeight = Screen.height * 0.01f; //the height of hte entire screen in unity metres
        brickWidth = screenHeight / 24f;//have 24 bricks depending on height
        float scaleWidth = brickWidth/0.32f;
        tile = Resources.Load("Tile") as GameObject;
        brickScale = new Vector3(scaleWidth, scaleWidth, 1f);//the z value doesn't matter. this is the base scale for every object in teh game when its created. you may want an object to be bigger, but make it bigger based on this value, like brickscale * 1.5 for 50% bigger etc
        int totalBackgroundWidth = (int)((Screen.width * 0.01f) / brickWidth);
        totalBackgroundWidth += 5;//add 3 just to be safe
        Camera.main.orthographicSize = Screen.height * 0.005f;
        Vector3 origin = Vector3.left * (totalBackgroundWidth / 2) * brickWidth + Vector3.down * 11.5f * brickWidth;
        segments = new List<RunnerSegment>();
        int totalSegmentWidth = 0;
        while(totalSegmentWidth <= totalBackgroundWidth)  //you have to fill up the first screen with randomized segments
        {
            int randomWidth = (int)Random.Range(50f, 60f);//randomly select a width for the new segment (must be between 30 and 120, this can change later)
            segments.Add(new RunnerSegment().SetupSegment(randomWidth, origin + Vector3.right * totalSegmentWidth * brickWidth,true)); //setup the runner segment after creating it
            totalSegmentWidth += segments[segments.Count - 1].GetSegmentWidth();  //update the total segment width of all the segments
        }
        foreach (RunnerSegment r in segments) { ProcessGameEvents(r.GetSegmentEvents()); } //go through the list of segments and get all the relevant game events they require
        tileLayout = new List<List<Tile>>();
        for (int y = 0; y < 24; y++)                        //for each block vertically
        {
            tileLayout.Add(new List<Tile>());               //create a new list of tiles
            for (int x = 0; x < totalBackgroundWidth; x++)  // go through every tile horizontally
            {
                Vector3 pos = origin + Vector3.right * x * brickWidth + Vector3.up * y * brickWidth; //this tiles positiong
                Tile t = CreateTile(pos);                                                           //create a tile at that positiong
                int tempX = x;                                              //presume the tiles x reference to be the same as x
                bool hasFoundProperSegment = false;                         //make a bool for confirming we have the x value for the relevant segment
                int segmentsRef = 0;                                        //start with segment ref 0, the first of the segments
                for(int o = 0; !hasFoundProperSegment; o++)
                {
                    segmentsRef = o;                                        //assume the current segment is the proper reference
                    if (segments[o].IsPointWithinSegmentWidth(tempX))       //if the x reference is within the current segment then we have found the proper reference
                    {
                        hasFoundProperSegment = true;                       //segments ref is the proper segment reference now (exit the for loop)
                    }
                    else
                    {
                        tempX -= segments[o].GetSegmentWidth();             //if the x reference is not inside of that segment then the x ref has to minus this segments width and start over on the next segment, minus the segment width and move on to the next segment (by repeating the for loop)
                    }
                }
                t.SetupTile(segments[segmentsRef].GetMapAtPoint(tempX,y));  //using segment ref to get the proper segment this tile belongs to and the temp x value which represents the x value of this tile in that segment and the y value
                tileLayout[y].Add(t);                                       //add it to the layout for later reference
            }
        }
        currentSegmentXRef = totalBackgroundWidth;                          //get the current segment xRef set it to the total backgroundwidth
        bool hasUpdatedXRef = false;                                        //make a bool for when it is the proper value for the current segment
        while (!hasUpdatedXRef)                     
        {
            if (segments[0].IsPointWithinSegmentWidth(currentSegmentXRef)) { hasUpdatedXRef = true; }    //if the total width is in the first segment then there is no other segments and the currentsegmentxref is accurate
            else { currentSegmentXRef -= segments[0].GetSegmentWidth();  segments.RemoveAt(0);  }       //if the segment doesn't contain this x value then remove that segments width and remove it from the list and repeat the while loop on the next segment.
        }
        //foreach (ParallaxEffect p in backgrounds) { p.SetupParallax(); }
        player.SetupPlayer();
    }

    /*************************************************************************
 UI Functions
 **************************************************************************/

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);  // Ensures that there are no duplicate instances
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Only use if you need this instance persisting across scene loads
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            //progressBar.value = (float)collectedItems / totalCollectibles;
            progressBar.value = collectedItems;
            if(progressBar.value == progressBar.maxValue)
            {
                progressBar.value = 0;
                dialogBox.SetBody(bodyText[dialogBoxRef]);
                dialogBox.SetHeader(headerText[dialogBoxRef]);
                dialogBoxRef++;
                if(dialogBoxRef == bodyText.Length) { dialogBoxRef = 0; }
                collectedItems = 0;
            }
        }
    }


    /*************************************************************************
     Item Functions
     **************************************************************************/

    public void CollectItem()
    {
        collectedItems++;
        UpdateProgressBar();
    }

    // Add this method to be called when an item is collected
    public void ItemCollected()
    {
        CollectItem();
    }

    /***************************************************************************
    Tile/Segment functions
    ***************************************************************************/

    Tile CreateTile(Vector3 pos)
    {
        Transform t = Instantiate(tile, pos, Quaternion.identity).transform;
        t.localScale = brickScale;
        Tile til = t.GetComponent<Tile>();
        return til;
    }

    void CreateNextSegment(Vector3 newOrigin,bool introSegment)
    {
        segments.Add(new RunnerSegment().SetupSegment((int)Random.Range(50f, 60f), newOrigin,introSegment));
        currentSegmentXRef = 0;
        //currentSegmentXRef -= segments[0].GetSegmentWidth(); 
        segments.RemoveAt(0);
        ProcessGameEvents(segments[0].GetSegmentEvents());
    }

    void ProcessGameEvents(List<GameEvent> events)
    {
        while(events.Count > 0)
        {
            GameEvent e = events[0];
            switch (e.GetEventType())
            {
                case GameEventType.createActor:CreateActor(e); break;
                case GameEventType.createCollider:CreateCollider(e);break;
                case GameEventType.removeActor:RemoveActor(e);break;
                case GameEventType.playSound:PlaySound(e);break;
                case GameEventType.applyStatusAffector:ApplySettingsAffector(e);break;
            }
            events.RemoveAt(0);
        }
    }
    void ApplySettingsAffector(GameEvent e){player.AddSettingsAffector(e.GetAffector());    }
    void PlaySound(GameEvent e)
    {
        audio.Play(e.GetPrefabName());
    }
    void RemoveActor(GameEvent e)
    {
        Actor a = e.GetActor();
        actors.Remove(a);
        ObjectPool.ReturnObjectToPool(a.GetTransform().gameObject);
    }

    void CreateCollider(GameEvent e)
    {
        string prefabName = "BoxCollider";
        bool isBox = e.GetBool();
        if (!isBox) { prefabName = "CircleCollider"; }
        GameObject g = ObjectPool.SpawnObjectFromName(prefabName, e.GetPos(), Quaternion.identity);
        ColliderScript c = g.GetComponent<ColliderScript>();
        c.SetupCollider(e.GetFloat(), e.GetFloat2());
        colliders.Add(c);
    }

    void CreateActor(GameEvent e)
    {
        Vector3 pos = e.GetPos();
        string prefabName = e.GetPrefabName();
        Transform t = ObjectPool.SpawnObjectFromName(prefabName, pos, Quaternion.identity).transform;
        Actor a = t.GetComponent<Actor>();
        t.gameObject.SetActive(true);
        List<GameEvent> temp = a.SetupActor();
        actors.Add(a);
        ProcessGameEvents(temp);
    }

    public static bool IsPointLeftOfCamera(Vector3 v) 
    {
        Vector3 diff = Camera.main.transform.position - v;
        return diff.x > Screen.width * 0.005f;
    }

    void CheckIfTilesHavePassedScreenLeft()
    {
        float xPos = tileLayout[0][0].transform.position.x;//only x pos matters so we only need to take one tile's xpos to deteremine if the rest in teh column need to be moved
        float leftDiff = transform.position.x - xPos;       //get the x difference from the camera
        if(Mathf.Abs(leftDiff) > Screen.width * 0.0055f)    //if its enough to be beyond the left side of the screen
        {
            //TileType typeOfTile = TileType.none;
            if (segments[0].IsPointWithinSegmentWidth(currentSegmentXRef))  //if the current segment x ref is within the current segment then we dont need to do anything
            { 
            
            }
            else   //otherwise we must create a new segment. the old segmen isn't needed any more
            {
                Vector3 newOrigin = tileLayout[0][0].transform.position + Vector3.right * brickWidth * tileLayout[0].Count;
                Tile t = tileLayout[0][0];
                CreateNextSegment(newOrigin,false);
                currentSegmentXRef = 0;
            }                      
            for(int i =0; i < 24; i++)
            {
               
                TileType typeOfTile = segments[0].GetMapAtPoint(currentSegmentXRef, i);         //now that we know segments[0] is the proper segment get the type of tile at the current xsegment ref and the appropriate y value (i)
                Tile t = tileLayout[i][0];                                                  //makea variable for this tile
                tileLayout[i].RemoveAt(0);                                                  //remove it from the 0 positiong
                tileLayout[i].Add(t);                                                       //add it back on the top of the list
                t.SetupTile(typeOfTile);                                                    //set it up as the proper tile type
                t.transform.Translate(Vector3.right * brickWidth * tileLayout[0].Count,Space.World);    //and move it to where that tile should be in the real world
            }
            currentSegmentXRef++;
        }
    }
    float GetTimeFactor() { 
        return 1f; 
    }//in case later we want to mess with teh speed of time

    void UpdateGame(float timePassed)
    {
        if (!gamePaused)
        {
            timePassed *= GetTimeFactor();
            List<GameEvent> updateEvents = new List<GameEvent>();
            for (int i = 0; i < actors.Count; i++)
            {
                Actor a = actors[i];
                updateEvents.AddRange(a.UpdateActor(timePassed));
            }
            updateEvents.AddRange(player.UpdatePlayer(timePassed));
            ProcessGameEvents(updateEvents);
            CheckIfTilesHavePassedScreenLeft();
            UpdateCamera();
        }
        else
        {
            if(Time.time > 0.5f && Input.touchCount > 0)
            {
                StartGame();
            }
        }
        
        //UpdateParallaxEffects(timePassed);
    }
    void UpdateParallaxEffects(float timePassed)
    {
        foreach(ParallaxEffect p in backgrounds) { p.UpdateParallax(timePassed); }
    }
    void UpdateCamera()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 campPos = transform.position;
        Vector3 objectivePosition = new Vector3(playerPos.x + Screen.width * 0.0035f, campPos.y, campPos.z);
        Vector3 diff = objectivePosition - campPos;diff.z = 0f;
        transform.position = (campPos + diff * 0.25f) ;
    }

    private void FixedUpdate()
    {
        //transform.Translate(Vector3.right * Time.fixedDeltaTime * 1.75f, Space.World);
        UpdateGame(Time.fixedDeltaTime);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
}
