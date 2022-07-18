using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private Vector2 velocity;
    private void Update()
    {
        if (target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            offset = target.position - transform.position;
        }
    }
    private void LateUpdate()
    {
        if (target != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, 
                target.position.x - offset.x, ref velocity.x, 0.05f);
            float posY = Mathf.SmoothDamp(transform.position.y,
                target.position.y - offset.y, ref velocity.y, 0.05f);
            
            if(transform.position.y < posY) // transform是相机位置
                // 只有当前Player的Y轴坐标大于相机才能移动
                transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}
