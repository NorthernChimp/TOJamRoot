using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    // Start is called before the first frame update
    BoxCollider2D coll;
    void Start()
    {
        
    }
    public void SetupCollider(float width,float height)
    {
        if (coll == null) coll = GetComponent<BoxCollider2D>();
        coll.size = new Vector2(width, height);
    }
    public bool RemoveCollider() { return MainScript.IsPointLeftOfCamera(transform.position + Vector3.right * coll.size.x * 0.5f); }
    // Update is called once per frame
    void Update()
    {
        
    }
}
