using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacle;
    private bool startTimer;
    private float fallTime;
    private Rigidbody2D my_body;

    /// <summary>
    /// ����һ�����ƽ̨ - composite platform
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="obstacleDir"></param>

    private void Awake()
    {
        my_body = GetComponent<Rigidbody2D>();
    }

    public void Init(Sprite sprite,float falltime, int obstacleDir)
    {
        this.fallTime = falltime;
        my_body.bodyType = RigidbodyType2D.Static;
        startTimer = true;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }

        if(obstacleDir == 0) // ���ұ�
        {
            if (obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y, 0);
            }
        }
    }
    private void Update()
    {
        if (startTimer && GameManager.Instance.IsGameStarted && GameManager.Instance.isCharacterMove)
        {
            fallTime -= Time.deltaTime;
            if(fallTime < 0)
            {
                // ����
                startTimer = true;
                if(my_body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_body.bodyType = RigidbodyType2D.Dynamic;
                    my_body.freezeRotation = true;
                    StartCoroutine(DelayHide());
                }
            }
        }

        if(gameObject.transform.position.y + 6 < Camera.main.transform.position.y)
        {
            StartCoroutine(DelayHide());
        }
    }
    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
