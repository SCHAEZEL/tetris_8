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
    public Vector3 startSpawnPos; // 预设的平台初始平台位置
    private int spawnPlatformCount; // 生成平台数量
    private int misleadPlatformCount; // 生成的误导平台数量

    private int mileStone = 10;
    private float fallTime;
    private float minFallTime;
    private float coefficient;

    private ManagerVars vars;
    private Vector3 mainPathPlatformPos; // current平台生成位置
    private Vector3 misleadPathPlatformPos;
    private bool isLeftSpawn = false; // 是否左向生成
    private Sprite selectPlaformThemeSprite; // 选中的平台
    private PlatformGroupType groupType; // 组合平台的类型

    private void Awake()
    {
        coefficient = 0.9f;
        fallTime = 2;
        minFallTime = 0.5f;

        EventCenter.AddListener(EventDefine.DecidePath,DecidePath);

        vars = ManagerVars.GetManagerVars(); // 从资源类获取资源容器
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
    /// 随机平台主题
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
    /// 确定路径
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
            isLeftSpawn = !isLeftSpawn; // 转向
            spawnPlatformCount = Random.Range(1, 5);
            SpawnPlatform();
        }
    }
    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        // 生成单个平台
        int randObstacleDir = Random.Range(0, 2); // 复合平台障碍的位置， 0左1右

        if(misleadPlatformCount-- >= 1)
        {
            Debug.Log("mislead count = " + misleadPlatformCount);
            misleadPathPlatformPos = SpawnSinglePlatform(misleadPathPlatformPos, isLeftSpawn);
            Debug.Log("create a spike");
        }

        if (spawnPlatformCount >= 1) // 等于1时还要再生成一个平台
        {
            SpawnNormalPlatform();
            
        }
        // 生成组合平台，刚好在拐角位置
        else if(spawnPlatformCount == 0) 
        {
            int ran = Random.Range(0, 3);
            // 生成通用组合平台
            if(ran == 0)
            {
                SpawnCompositePlatform(ComposiePlatformType.Common, randObstacleDir);
            }
            // 生成主题组合平台
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
            // 生成钉子组合平台
            else // ran == 2
            {
                int Left;
                if (isLeftSpawn)
                    Left = 1; // 路往左边走生成右向钉子
                else
                    Left = 0; // 路往右边走生成左向钉子

                misleadPathPlatformPos = SpawnSpikePlatformGroup(Left); // 获得钉子位置
                misleadPlatformCount = Random.Range(1, 5); // 决定生成多少个误导平台
                 
                // 钉子平台已生成，接下来生成误导平台
            }
        }

        if (isLeftSpawn) // 向左生成平台
        {
            mainPathPlatformPos = new Vector3(mainPathPlatformPos.x - vars.nextXPos,
                mainPathPlatformPos.y + vars.nextYPos);
        }
        else // 向右生成平台
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
        //GameObject next = Instantiate(vars.normalPlatformPre, transform); // 新实例化一个GameObject
        next.transform.position = mainPathPlatformPos;
        next.GetComponent<PlatformScript>().Init(selectPlaformThemeSprite, fallTime, 0);
    }

    private Vector3 SpawnSpikePlatformGroup(int Left)
    {
        GameObject next;
        Vector3 spikePos;
        //Vector3 cur;
        Debug.Log("try to spawn spike");

        if (Left == 0) // 1表示main path向左，钉子向右生成
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