using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMove : MonoBehaviour
{
    private GameObject god;
    // Start is called before the first frame update
    void Start()
    {
        god = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            god.transform.position = new Vector2(god.transform.position.x - 30, god.transform.position.y);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            god.transform.position = new Vector2(god.transform.position.x + 30, god.transform.position.y);
        }
    }
}
