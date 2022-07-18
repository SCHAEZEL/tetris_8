using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlatformGroupType
{
    Grass,Winter
}
public enum DirectionType
{
    Left,Right
}

public enum ComposiePlatformType
{
    Common,Grass,Winter
}
public class PlatformSpawner : MonoBehaviour
{
    public Vector3 startSpawnPos; // Ԥ���ƽ̨��ʼƽ̨λ��
    private int spawnPlatformCount; // ����ƽ̨����
    private int misleadPlatformCount; // ���ɵ���ƽ̨����

    private int mileStone = 10;
    private float fallTime;
    private float minFallTime;
    private float coefficient;

    private ManagerVars vars;
    private Vector3 mainPathPlatformPos; // currentƽ̨����λ��
    private Vector3 misleadPathPlatformPos;
    private bool isLeftSpawn = false; // �Ƿ���������
    private Sprite selectPlaformThemeSprite; // ѡ�е�ƽ̨
    private PlatformGroupType groupType; // ���ƽ̨������

    private void Awake()
    {
        coefficient = 0.9f;
        fallTime = 2;
        minFallTime = 0.5f;

        EventCenter.AddListener(EventDefine.DecidePath,DecidePath);

        vars = ManagerVars.GetManagerVars(); // ����Դ���ȡ��Դ����
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }
    private void Start()
    {
        
        RandomPlatformTheme();
        startSpawnPos = vars.StartSpawnPos;
        mainPathPlatformPos = startSpawnPos; 
        
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }

        GameObject next = Instantiate(vars.characterPre);
        next.transform.position = new Vector3(0,- 1.8f, 0);
    }
    /// <summary>
    /// ���ƽ̨����
    /// </summary>
    /// 

    private void Update()
    {
        if(!GameManager.Instance.IsGameOver && GameManager.Instance.IsGameStarted && !GameManager.Instance.IsGamePause)
        {
            UpdateFallTime();
        }
    }

    private void UpdateFallTime()
    {
        if(GameManager.Instance.getScore() > mileStone)
        {
            mileStone *= 2;
            fallTime *= coefficient;
            if (fallTime < minFallTime)
                fallTime = minFallTime;
        }
    }

    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlaformThemeSprite = vars.platformThemeSpriteList[ran];

        if(ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }

    /// <summary>
    /// ȷ��·��
    /// </summary>
    private void DecidePath()
    {
        int rand = Random.Range(0, 10);
        if (rand == 6)
        {
            GameObject diamond = ObjectPool.Instance.GetDiamond();
            diamond.SetActive(true);
            diamond.transform.position = new Vector3(mainPathPlatformPos.x,
                mainPathPlatformPos.y + 0.5f, 0);
        }
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            isLeftSpawn = !isLeftSpawn; // ת��
            spawnPlatformCount = Random.Range(1, 5);
            SpawnPlatform();
        }
    }
    /// <summary>
    /// ����ƽ̨
    /// </summary>
    private void SpawnPlatform()
    {
        // ���ɵ���ƽ̨
        int randObstacleDir = Random.Range(0, 2); // ����ƽ̨�ϰ���λ�ã� 0��1��

        if(misleadPlatformCount-- >= 1)
        {
            Debug.Log("mislead count = " + misleadPlatformCount);
            misleadPathPlatformPos = SpawnSinglePlatform(misleadPathPlatformPos, isLeftSpawn);
            Debug.Log("create a spike");
        }

        if (spawnPlatformCount >= 1) // ����1ʱ��Ҫ������һ��ƽ̨
        {
            SpawnNormalPlatform();
            
        }
        // �������ƽ̨���պ��ڹս�λ��
        else if(spawnPlatformCount == 0) 
        {
            int ran = Random.Range(0, 3);
            // ����ͨ�����ƽ̨
            if(ran == 0)
            {
                SpawnCompositePlatform(ComposiePlatformType.Common, randObstacleDir);
            }
            // �����������ƽ̨
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnCompositePlatform(ComposiePlatformType.Grass, randObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnCompositePlatform(ComposiePlatformType.Winter, randObstacleDir);
                        break;
                    default:
                        break;
                }
            }
            // ���ɶ������ƽ̨
            else // ran == 2
            {
                int Left;
                if (isLeftSpawn)
                    Left = 1; // ·�����������������
                else
                    Left = 0; // ·���ұ�������������

                misleadPathPlatformPos = SpawnSpikePlatformGroup(Left); // ��ö���λ��
                misleadPlatformCount = Random.Range(1, 5); // �������ɶ��ٸ���ƽ̨
                 
                // ����ƽ̨�����ɣ�������������ƽ̨
            }
        }

        if (isLeftSpawn) // ��������ƽ̨
        {
            mainPathPlatformPos = new Vector3(mainPathPlatformPos.x - vars.nextXPos,
                mainPathPlatformPos.y + vars.nextYPos);
        }
        else // ��������ƽ̨
        {
            mainPathPlatformPos = new Vector3(mainPathPlatformPos.x + vars.nextXPos,
                mainPathPlatformPos.y + vars.nextYPos);
        }
    }

    public Vector3 SpawnSinglePlatform(Vector3 original, bool left)
    {
        
        GameObject next = ObjectPool.Instance.GetPlatform(PlatformType.Normal);
        if (left)
        {
            next.transform.position = new Vector3(original.x - vars.nextXPos,
                original.y + vars.nextYPos, 0);
        }
        else
        {
            next.transform.position = new Vector3(original.x + vars.nextXPos,
                original.y + vars.nextYPos, 0);
        }
        Debug.Log("enter spawn single platform");
        next.SetActive(true);
        next.GetComponent<PlatformScript>().Init(selectPlaformThemeSprite, fallTime, 0);
        return next.transform.position;
        //return original;
    }

    private void SpawnNormalPlatform()
    {
        //int ran = Random.Range(0, 2);
        GameObject next = ObjectPool.Instance.GetPlatform(PlatformType.Normal);
        next.SetActive(true);
        //GameObject next = Instantiate(vars.normalPlatformPre, transform); // ��ʵ����һ��GameObject
        next.transform.position = mainPathPlatformPos;
        next.GetComponent<PlatformScript>().Init(selectPlaformThemeSprite, fallTime, 0);
    }

    private Vector3 SpawnSpikePlatformGroup(int Left)
    {
        GameObject next;
        Vector3 spikePos;
        //Vector3 cur;
        Debug.Log("try to spawn spike");

        if (Left == 0) // 1��ʾmain path���󣬶�����������
        {
            next = ObjectPool.Instance.GetPlatform(PlatformType.SpikeLeft);
        }
        else
        {
            next = ObjectPool.Instance.GetPlatform(PlatformType.SpikeRight);
        }
        next.SetActive(true);
        next.transform.position = mainPathPlatformPos;
        //next.SetActive(true);
        spikePos = Left == 1 ? new Vector3(next.transform.position.x + 1.09f, next.transform.position.y, 0)
            : new Vector3(next.transform.position.x - 1.09f, next.transform.position.y, 0);

        next.GetComponent<PlatformScript>().Init(selectPlaformThemeSprite, fallTime, Left);
        //return next.transform.position;
        return spikePos;
    }

    private void SpawnCompositePlatform(ComposiePlatformType composiePlatformType, int randObstacleDir)
    {
        GameObject next;
        //int rand;
        switch (composiePlatformType)
        {
            case ComposiePlatformType.Common:
                //rand = Random.Range(0, vars.commonPlatformGroup.Count);
                next = ObjectPool.Instance.GetPlatform(PlatformType.Common);
                break;
            case ComposiePlatformType.Grass:
                //rand = Random.Range(0, vars.grassPlatformGroup.Count);
                next = ObjectPool.Instance.GetPlatform(PlatformType.Grass);
                break;
            case ComposiePlatformType.Winter:
                //rand = Random.Range(0, vars.winterPlatformGroup.Count);
                next = ObjectPool.Instance.GetPlatform(PlatformType.Winter);
                break;
            default:
                return;
        }
        next.SetActive(true);
        next.transform.position = mainPathPlatformPos;
        next.GetComponent<PlatformScript>().Init(selectPlaformThemeSprite,fallTime, randObstacleDir);
    }
}