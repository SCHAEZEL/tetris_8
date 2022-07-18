using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;
    private bool isMoveLeft = false; // 是否点击了左侧屏幕
    private bool isJumping = false;
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_body;
    private SpriteRenderer spriteRenderer;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_body = GetComponent<Rigidbody2D>();
        m_AudioSource = GetComponent<AudioSource>();
        GameManager.Instance.isCharacterMove = false;
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.AddListener<bool>(EventDefine.SetAudio, SetAudioOn);
    }

    private void ChangeSkin(int selectIndex)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[selectIndex];
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.RemoveListener<bool>(EventDefine.SetAudio, SetAudioOn);

    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击的UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }

    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 0.4f, Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);

        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    Debug.Log("UI clicked.");
        //    return;
        //}
        if (IsPointerOverGameObject(Input.mousePosition)) return;

        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver
            || GameManager.Instance.IsGamePause)
            return;

        if (Input.GetMouseButtonDown(0) && isJumping == false && nextPlatformLeft != Vector3.zero)
        //            && EventSystem.current.IsPointerOverGameObject() == false
        {

            if (!GameManager.Instance.isCharacterMove)
            {
                GameManager.Instance.isCharacterMove = true;
            }
            m_AudioSource.PlayOneShot(vars.jumpClip);
            //Debug.Log("Button Left Clicked.");
            EventCenter.Broadcast(EventDefine.DecidePath);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x <= Screen.width / 2)
            {
                // 点击左边
                isMoveLeft = true;
            }
            else if (mousePos.x > Screen.width / 2) // 可以不用写的，不过还是增加可读性好
            {
                // 点击右边
                isMoveLeft = false;
            }
            Jump();
            // 代表y轴速度为负方向
        }

        // 第一种死法
        if (my_body.velocity.y < 0 && !IsDetectedPlatform() && 
            !GameManager.Instance.IsGameOver)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            //Debug.Log("velocity : " + my_body.velocity.y);
            //Debug.Log("Detected ? : " + IsDetectedPlatform());
            print("游戏结束1");
            //spriteRenderer.sortingLayerName = "Platform";
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
        }

        // 第二种死法
        if (isJumping && IsDetectedObstacle() && GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.hitClip);

            print("游戏结束2");
            GameObject deathEffect = ObjectPool.Instance.GetEffect();
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            //Destroy(gameObject);
            spriteRenderer.enabled = false;
        }
        // 第三种死法
        if (gameObject.transform.position.y + 6 < Camera.main.transform.position.y)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);

            print("游戏结束3");
            GameManager.Instance.IsGameOver = true;
        }
        if (GameManager.Instance.IsGameOver)
        {
            print("Game Over!");
            StartCoroutine(DelayShowGameOverPanel());
            //EventCenter.Broadcast(EventDefine.AwakeGameOverPanel);
        }

    }
    IEnumerator DelayShowGameOverPanel()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("AwakeGameOverPanel");
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
        
    }




    private GameObject lastHitPlatform = null;

    private bool IsDetectedPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 0.7f, platformLayer);
        //if (hit.collider != null) // 这一帧有没有碰撞
        //    if (hit.collider.tag == "Platform") // 如果这一帧有碰撞，那碰撞的是否为Platform
        //    {
        //        return true;
        //    }

        //if (hit.collider != null)
        //{
        //    Debug.Log("hit position:" + hit.transform.position);
        //    if (hit.collider.tag == "Platform")
        //    {
        //        if (lastHitPlatform != hit.collider.gameObject)
        //        {
        //            if (lastHitPlatform == null)
        //            {
        //                lastHitPlatform = hit.collider.gameObject;
        //                return true;
        //            }
        //            EventCenter.Broadcast(EventDefine.AddScore);
        //            lastHitPlatform = hit.collider.gameObject;
        //        }
        //        return true;
        //    }
        //}
        if (hit.collider != null)
        {
            //Debug.Log("hit position:" + hit.transform.position);
            if (hit.collider.tag == "Platform")
            {
                if (lastHitPlatform != hit.collider.gameObject)
                {
                    if (lastHitPlatform == null)
                    {
                        lastHitPlatform = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitPlatform = hit.collider.gameObject;
                }
                return true;
            }
        }

        //Debug.Log("hit position:null");
        return false;
    }

    private bool IsDetectedObstacle()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);

        if (hitLeft.collider != null)
        {
            if (hitLeft.collider.tag == "Obstacle")
                return true;
        }
        if (hitRight.collider != null)
        {
            if (hitRight.collider.tag == "Obstacle")
                return true;
        }
        return false;
    }
    private void Jump()
    {
        //isJumping = true;
        if (isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
        }
        else
        {
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
            transform.localScale = Vector3.one;
        }
    }

    private void SetAudioOn(bool value)
    {
        m_AudioSource.mute = !value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ///
        if (!collision.CompareTag("Platform"))
        {
            Debug.Log("没撞到平台，返回...");
            return;
        }

        Debug.Log("OnTriggerEnter2D");
        isJumping = false;
        Vector3 currentPlatformPos = collision.gameObject.transform.position;
        nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos,
            currentPlatformPos.y + vars.nextYPos, 0);
        nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos,
            currentPlatformPos.y + vars.nextYPos, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(("Pickup")))
        {
            return;
        }
        Debug.Log("吃到钻石了...");
        m_AudioSource.PlayOneShot(vars.diamondClip);
        collision.gameObject.SetActive(false);
        EventCenter.Broadcast(EventDefine.AddDiamond);
    }


}
