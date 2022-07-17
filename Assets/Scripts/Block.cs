using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject entity;

    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    private bool check = false;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision){
        // entity.transform.Translate(new Vector2(0,0));
        //Debug.Log("111111");
    }

    void OnCollisionEnter2D(Collision2D collision){
        // entity.transform.Translate(new Vector2(0,0));
        if(collision.gameObject.CompareTag("block")){
            Debug.Log("Destory");
            Destroy(this);
        }
        
    }
}
