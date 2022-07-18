using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject block;
    //Rigidbody2D rb;
    public float Speed = 5f;
    void Start()
    {
        block = this.gameObject;
        //rb = GetComponent<Rigidbody2D>();
        //rb.velocity = new Vector2(0, -30);
        
    }

    // Update is called once per frames
    void FixedUpdate(){
        // block.transform.Translate(new Vector2(0,-3));
    }

    void Update()
    {   
        
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            //block.transform.position = new Vector2(block.transform.position.x, block.transform.position.y - 30);
            block.transform.Translate(new Vector2(0,-1) * Speed * Time.deltaTime);
        }else{
            block.transform.Translate(new Vector2(0,-1) * Speed * 2 * Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            block.transform.position = new Vector2(block.transform.position.x - 30, block.transform.position.y);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            block.transform.position = new Vector2(block.transform.position.x + 30, block.transform.position.y);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter)){
            block.transform.position = new Vector2(block.transform.position.x, 0);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)){
            //rb.velocity = new Vector2(0, 0);
        }
    }
}
