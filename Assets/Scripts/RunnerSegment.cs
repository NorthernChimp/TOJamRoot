using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerSegment 
{
    TileType[,] tileMap; // a map of every tile in the segment
    int segmentWidth;public int GetSegmentWidth() { return segmentWidth; }
    public TileType GetMapAtPoint(int x,int y) { return tileMap[x, y]; }
    Vector3 origin; public Vector3 GetOrigin() { return origin; }
    public Vector3 GetNextOrigin() { return origin + Vector3.right * (float)(segmentWidth ) * MainScript.brickWidth;
        //return origin + Vector3.right * (segmentWidth - 1)* MainScript.brickWidth;
    }
    public bool IsPointWithinSegmentWidth(int x) { return x < segmentWidth; }
    List<GameEvent> segmentEvents;
    public List<GameEvent> GetSegmentEvents() { return segmentEvents; }
    void CreateHouse(Vector2Int lowerLeftCorner, int width, int height)
    {
        for(int x = 0;x < width; x++)
        {
            for(int y =0; y < height; y++)
            {
                Vector2Int current = new Vector2Int(lowerLeftCorner.x + x, lowerLeftCorner.y + y);//get the current tile coordinates
                if(y < height - 1)
                {
                    tileMap[current.x, current.y] = TileType.house;
                }
                else
                {
                    tileMap[current.x, current.y] = TileType.roof;
                }
            }
        }
        Vector3 lowerLeftPos = origin + ((float)lowerLeftCorner.x * Vector3.right * MainScript.brickWidth) + ((float)lowerLeftCorner.y * Vector3.up * MainScript.brickWidth);
        float completeWidth = (float)width * MainScript.brickWidth;
        Vector3 middleOfRoof = lowerLeftPos + (Vector3.up * (float)(height - 1) * MainScript.brickWidth) + (Vector3.right * completeWidth * 0.5f) + (Vector3.left * 0.5f * MainScript.brickWidth);
        Vector3 topMiddleOfhouse = GetPosFromCoordinate(new Vector2Int(lowerLeftCorner.x + (int)(width * Random.value), lowerLeftCorner.y + height)) + Vector3.up * MainScript.brickWidth * 1.5f;//get the current tile coordinates
        segmentEvents.Add(GameEvent.GetCreateColliderEvent(middleOfRoof, completeWidth, 1f * MainScript.brickWidth, true));
        if (Random.value > 0.5f) { segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan", topMiddleOfhouse)); }
    }
    public RunnerSegment SetupSegment(int width,Vector3 ori, bool introSegment)//intro segment is to make sure there aren't freestanding pieces when you spawn in. otherwise like one int 15 games you get stuck when you spawn in
    {
                                                                                    //I'll try to break this down as a basic level building
        if(width < 30) { width = 30; }else if (width > 120) { width = 120; }        //this is just to make sure the segment isn't too small or large, it can be tweaked later
        origin = ori;                                                               //the origin is basically teh lower left corner of a segment and the point from which all other tiles are measure
        segmentWidth = width;                                                   
        tileMap = new TileType[width, 24];                                          //Create a 2d array to contain every tile in the level
        for(int x = 0; x< width; x++)                                               //use a for loop to iterate through that array
        {
            for(int y =0;y < 24; y++)
            {
                /*if (x == 0) { tileMap[x, y] = TileType.house; }                     //if the x value is 0 make it a house tile (tiles are just different colors right now but later will be actual textures likea  roof or house texture
                else if(x == width - 1) { tileMap[x, y] = TileType.roof; }          //if the x favlu is the last in the segment meaning its on the far right side make it a roof, both this and the last line are just to illustrate where the segment ends and a new one begings
                else
                {
                    //if (y == 0) { tileMap[x, y] = TileType.ground; }                //if the y value is zero then it is the bottom of the screen make it a ground tile
                    //else { tileMap[x, y] = TileType.skyBackground; }                //otherwise it is just a sky background tile.
                }*/
                if (y == 0) { tileMap[x, y] = TileType.ground; }                //if the y value is zero then it is the bottom of the screen make it a ground tile
                else { tileMap[x, y] = TileType.skyBackground; }                //otherwise it is just a sky background tile.
            }
        }
        //everything up to this point just kind of builds teh basics of the level and from now on we're going to change tiles or add objects to fill it in

        //this is just a bunch of random numbers
        /*int randomInt = (int)Random.Range(4f, 12f);     //this random number is the x value of the origin where we're going to make a house
        int randomInt2 = (int)Random.Range(4f, 12f);    //this is the y value of how high the house is going to be
        int randomWidth = (int)Random.Range(2f, 4f);    //this is how wide the house is
        for(int x = 0; x < randomWidth; x++)            
        {
            for (int y = 1; y < randomInt2; y++)                //we iterate through all the tiles at those areas and set them to be a house instead
            {
                tileMap[randomInt + x, y] = TileType.house;
            }
        }*/
        //this will draw a house (black tile) from the x y pos randomInt, 1 to randomInt + randomWidth,1 + randomInt2
        //I will try to use more obvious names when I'm working on the main version like houseWidth houseHeight HouseOriginX HouseOriginY

        segmentEvents = new List<GameEvent>(); //these are the game events that come with the segment. Mainly just "create actor" events like creating scenery like the garbage can or a bad guy, or a trampoline, anything really

        //I created a function for building a house in the segment
        Vector2Int houseOrigin = new Vector2Int((int)(width / 3), 1);
        int houseWidth = GetRandomIntBetween(5, 11);
        int houseHeight = GetRandomIntBetween(5, 8);
        CreateHouse(houseOrigin, houseWidth,houseHeight);
        if(houseHeight > 4)
        {
            if(Random.value < 0.25f)
            {
                Vector2Int rightAndAboveOrigin = new Vector2Int(houseOrigin.x - 8, houseOrigin.y + 4);
                int widthFree = GetRandomIntBetween(2, 5);
                if (!introSegment) { CreateFreeStandingWall(rightAndAboveOrigin, widthFree); }
                
            }
            else
            {
                CreateFreeStandingWall(new Vector2Int(0, 1), 6);
                CreateFreeStandingWall(new Vector2Int(3, 2), GetRandomIntBetween(3,5));
                if(houseHeight > 6)
                {
                    if (!introSegment){CreateFreeStandingWall(new Vector2Int(houseOrigin.x - 2, houseOrigin.y + houseHeight - 2), 2);}
                }
            }
        }
        else
        {
            CreateFreeStandingWall(new Vector2Int(0, 1), 3);
        }
        if(Random.value < 0.25f)
        {
            Vector2Int topRight = new Vector2Int(houseOrigin.x + houseWidth + 4, houseOrigin.y + houseHeight + 0);
            if (!introSegment){CreateFreeStandingWall(topRight, 2);}
            Vector3 pos = GetPosFromCoordinate(topRight) + Vector3.right * MainScript.brickWidth * 0.5f + Vector3.up * MainScript.brickWidth * 2f;
            segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan", pos));
            if(Random.value < 0.25f)
            {
                topRight = new Vector2Int(houseOrigin.x + houseWidth + 7, houseOrigin.y + houseHeight + 2);
                if (!introSegment){   CreateFreeStandingWall(topRight, 3);}
                pos = GetPosFromCoordinate(topRight) + Vector3.right * MainScript.brickWidth * 0.5f + Vector3.up * MainScript.brickWidth * 2f;
                segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan", pos));
            }
        }
        else
        {
            if(Random.value > 0.5f)
            {
                Vector2Int topRight = new Vector2Int(houseOrigin.x + houseWidth + 3, houseOrigin.y + 1);
                if (!introSegment){CreateFreeStandingWall(topRight, GetRandomIntBetween(2, 4));}
            }
        }
        if (Random.value > 0.5f) //Random.value is just a rnadom value between 0 and 1, this is basically saying if that number is higher than 0.5 (50%) do this thing
        {
            int randoInt = GetRandomIntBetween(13, 16);             //takes a random x value
            //int otherInt = randomInt + randomWidth;
            Vector3 randomPos = origin + Vector3.right * MainScript.brickWidth * randoInt + Vector3.up * MainScript.brickWidth * 2f; //uses the random x value to place the object down in the scene
            //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan", randomPos));                                                 //creates the game event to create that actor and adds it to the segment events
        }

        //CreateFreeStandingWall(new Vector2Int(GetRandomIntBetween(15, 16), GetRandomIntBetween(15, 20)), 3);
        //also I'll make some Vector3 values in here shorthand for you guys. this is a vector that is always in the lower left corner of the house for example
        int randomInt = (int)Random.Range(2f, 4f);     //this random number is the x value of the origin where we're going to make a house
        Vector3 posInFrontOfHouse = origin + Vector3.right * MainScript.brickWidth * randomInt + Vector3.up * MainScript.brickWidth * 2f;
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        if(Random.value > 0.5f)
        {
            segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan", posInFrontOfHouse + Vector3.right * houseWidth * 1f * MainScript.brickWidth));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        }
        if (Random.value > 0.65f)
        {
            houseOrigin = new Vector2Int((int)((width * 2) / 3), 1);
            houseWidth = GetRandomIntBetween(5, 8);
            houseHeight = GetRandomIntBetween(5, 6);
            CreateHouse(houseOrigin, houseWidth, houseHeight);
        }
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        //segmentEvents.Add(GameEvent.CreateActorEvent("GarbageCan",posInFrontOfHouse));  //this is the game event that will create a garbage can at that point. Note its added to segment events otherwise the even will not be executed
        return this;
    }
    Vector3 GetPosFromCoordinate(Vector2Int v){return origin + Vector3.right * MainScript.brickWidth * (float)v.x + Vector3.up * MainScript.brickWidth * (float)v.y;}
    void CreateFreeStandingWall(Vector2Int leftcoord,int width)
    {
        for(int i = 0; i < width; i++)
        {
            Vector2Int currentCoord = new Vector2Int(leftcoord.x + i, leftcoord.y);
            tileMap[currentCoord.x, currentCoord.y] = TileType.ground;
        }
        Vector3 lowerLeftPos = origin + ((float)leftcoord.x * Vector3.right * MainScript.brickWidth) + ((float)leftcoord.y * Vector3.up * MainScript.brickWidth);
        float completeWidth = (float)width * MainScript.brickWidth;
        Vector3 middleOfRoof = lowerLeftPos + (Vector3.right * completeWidth * 0.5f) + (Vector3.left * 0.5f * MainScript.brickWidth);
        segmentEvents.Add(GameEvent.GetCreateColliderEvent(middleOfRoof, completeWidth, 1f * MainScript.brickWidth, true));
    }
    int GetRandomIntBetween(int min, int max){return (int)(Random.Range((float)min, (float)(max + 1)));}
}
public enum TileType { ground,skyBackground,house,roof,none}