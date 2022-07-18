using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Campere : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;
    private bool isMoveLeft = false;
    private bool isJumping = false;
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //����һ������¼�
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //����λ�÷���һ�����ߣ�����Ƿ�����UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }
    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1, Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);

        if (IsPointerOverGameObject(Input.mousePosition)) return;

        if (!GameManager.Instance.IsGameStarted || GameManager.Instance.IsGameOver
            || GameManager.Instance.IsGamePause)
            return;

        if (Input.GetMouseButtonDown(0) && !isJumping && nextPlatformLeft != Vector3.zero)
        {
            EventCenter.Broadcast(EventDefine.DecidePath);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            //������������Ļ
            if (mousePos.x <= Screen.width / 2)
            {
                isMoveLeft = true;
            }
            //������ұ���Ļ
            else if (mousePos.x > Screen.width / 2)
            {
                isMoveLeft = false;
            }
            Jump();
        }

        //��Ϸ������
        if (my_Body.velocity.y < 0 && !IsRayPlatform() && !GameManager.Instance.IsGameOver)
        {
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
        }
        if (isJumping && IsRayObstacle() && !GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            GameObject deathEffect = ObjectPool.Instance.GetEffect();
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;
            Destroy(gameObject);

        }
        //if (transform.position.y - Camera.main.transform.position.y < -5 && GameManager.Instance.IsGameOver == false)
        //{
        //    GameManager.Instance.IsGameOver = true;
        //    StartCoroutine(DealyShowGameOverPanel());
        //}
    }
    //IEnumerator DealyShowGameOverPanel()
    //{
    //    yield return new WaitForSeconds(1f);
    //    //���ý������
    //}
    private GameObject lastHitGo = null;
    /// <summary>
    /// �Ƿ��⵽ƽ̨
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {

        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);

        if (hit.collider != null)
        {
            Debug.Log("hit position:" + hit.transform.position);
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
        }
        Debug.Log("hit position:NULL");
        return false;
    }
    /// <summary>
    /// �Ƿ��⵽�ϰ���
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);

        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }

        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }
    private void Jump()
    {
        if (isJumping)
        {
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
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatformPos = collision.gameObject.transform.position;
            nextPlatformLeft = new Vector3(currentPlatformPos.x -
                vars.nextXPos, currentPlatformPos.y + vars.nextYPos, 0);
            nextPlatformRight = new Vector3(currentPlatformPos.x +
                vars.nextXPos, currentPlatformPos.y + vars.nextYPos, 0);
        }
    }
}