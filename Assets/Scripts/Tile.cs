using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    TileType typeOfTile = TileType.none; public TileType GetTypeOfTile() { return typeOfTile; }
    SpriteRenderer render;
    void Start()
    {
        
    }
    public void SetupTile(TileType ty)
    {
        typeOfTile = ty;
        render = GetComponent<SpriteRenderer>();
        Sprite s = SpriteLibrary.instance.GetSpriteFromKey("FLOOR_TILE-32");
        render.enabled = true;
        switch (ty)
        {
            case TileType.ground: render.sortingOrder = (0); break;//ground is default
            case TileType.skyBackground:render.enabled = false ;break;
            case TileType.house:s = SpriteLibrary.instance.GetSpriteFromKey("house"); render.sortingOrder = (0); break;
            case TileType.roof:s = SpriteLibrary.instance.GetSpriteFromKey("roof"); render.sortingOrder = (0); break;
        }
        render.sprite = s;
        //if (ty == TileType.ground) { SetColor(Color.red); } else if (ty == TileType.skyBackground) { SetColor(Color.blue); } else if (ty == TileType.roof) { SetColor(Color.cyan); } else { SetColor(Color.black); }
    }
    public void SetColor(Color c){render.color = c;}
    // Update is called once per frame
    void Update()
    {
        
    }
}
