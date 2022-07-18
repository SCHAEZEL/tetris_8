using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerMove cc;
    public float speed = 5f;
    private float energy = 0;

    bool jump;
    float move;
    // Start is called before the first frame update
    void Start()
    {
       cc = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        move *=speed;
        bool jump = Input.GetButton("Jump");
    }
    
    void FixedUpdate(){
        cc.Move(move, jump);
    }
}
