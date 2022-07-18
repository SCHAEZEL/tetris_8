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
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision){
        // entity.transform.Translate(new Vector2(0,0));
        Debug.Log("111111");
    }

    void OnCollisionEnter2D(Collision2D collision){
        // entity.transform.Translate(new Vector2(0,0));
        if(collision.gameObject.CompareTag("FreezBlock")){
            Debug.Log("111111");
            if(!check){
                //findAllBlock(this.gameObject);
            }
            //Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("ground")){
            Debug.Log("22222");
            findAllBlock(this.gameObject);
        }
    }

    void findAllBlock(GameObject b){
        freezBlock();
        if(b != null && !check){
            
            if(up != null){
                Block upB = up.GetComponent<Block>();
                upB.check = true;
                upB.findAllBlock(up);
                Debug.Log("UP");
            }
            if(down != null){
                Block downB = down.GetComponent<Block>();
                downB.check = true;
                downB.findAllBlock(down);
                Debug.Log("down");
            }
            if(left != null){
                Block leftB = left.GetComponent<Block>();
                leftB.check = true;
                leftB.findAllBlock(left);
                Debug.Log("left");
            }
            if(right != null){
                Block rightB = right.GetComponent<Block>();
                rightB.check = true;
                rightB.findAllBlock(right);
                Debug.Log("right");
            }
        }
        
    }

    void freezBlock(){
        rb.bodyType = RigidbodyType2D.Static;
        this.gameObject.tag = "FreezBlock";
    }
}
