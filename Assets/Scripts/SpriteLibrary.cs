using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLibrary : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private string[] lookupKey;
    public static SpriteLibrary instance;
    
    private void Awake()
    {
        if(instance == null) { instance = this; }
    }
    public Sprite GetSpriteFromKey(string key) 
    { 
        for(int i = 0; i < lookupKey.Length; i++) { if(key == lookupKey[i]) { return sprites[i]; } }
        return sprites[0];
    }
    // Update is called once per frame
    
}