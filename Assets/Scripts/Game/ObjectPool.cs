using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlatformType{
    Normal,Common,Grass,Winter,SpikeLeft,SpikeRight
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public int initSpawnCount = 5;
    private List<GameObject> normalPlatformList = new List<GameObject>();
    private List<GameObject> commonPlatformList = new List<GameObject>();
    private List<GameObject> grassPlatformList = new List<GameObject>();
    private List<GameObject> winterPlatformList = new List<GameObject>();
    private List<GameObject> spikePlatformLeftList = new List<GameObject>();
    private List<GameObject> spikePlatformRightList = new List<GameObject>();
    private List<GameObject> deathEffectList = new List<GameObject>();
    private List<GameObject> diamondList = new List<GameObject>();

    private ManagerVars vars;
    //private PlatformType platformType;
    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        Init();
    }
    private void Init()
    {
        for(int i = 0; i < initSpawnCount;i++)
        {
            
            InstantiateObject(vars.normalPlatformPre, ref normalPlatformList);
        }
        // 通用平台
        for (int j = 0; j < initSpawnCount; j++)
        {
            for (int i = 0; i < vars.commonPlatformGroup.Count; i++)
            {
                //Debug.Log("spawn:" + i);
                InstantiateObject(vars.commonPlatformGroup[i], ref commonPlatformList);
            }
        }

        for (int j = 0; j < initSpawnCount; j++)
        {
            for (int i = 0; i < vars.grassPlatformGroup.Count; i++)
            {
                InstantiateObject(vars.grassPlatformGroup[i], ref grassPlatformList);

            }
        }

        for (int j = 0; j < initSpawnCount; j++)
        {
            for (int i = 0; i < vars.winterPlatformGroup.Count; i++)
            {
                InstantiateObject(vars.winterPlatformGroup[i], ref winterPlatformList);
            }
        }
        
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.SpikePlatformGroup[0], ref spikePlatformLeftList);
        }
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.SpikePlatformGroup[1], ref spikePlatformRightList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.deathEffectPre, ref deathEffectList);
;       }

        for(int i = 0; i< initSpawnCount; i++)
        {
            InstantiateObject(vars.diamondPre, ref diamondList);
        }

    }

    private GameObject InstantiateObject(GameObject obj, ref List<GameObject> list)
    {
        GameObject tmp = Instantiate(obj, transform);
        tmp.SetActive(false);
        list.Add(tmp);
        return tmp;
    }

    public GameObject GetEffect()
    {
        for(int i = 0; i < deathEffectList.Count; i++)
        {
            if (deathEffectList[i].activeInHierarchy == false)
                return deathEffectList[i];
        }
        return InstantiateObject(vars.deathEffectPre, ref deathEffectList);
    }

    public GameObject GetDiamond()
    {
        for (int i = 0; i < diamondList.Count; i++)
        {
            if (diamondList[i].activeInHierarchy == false)
                return diamondList[i];
        }
        return InstantiateObject(vars.diamondPre, ref diamondList);
    }




    public GameObject GetPlatform(PlatformType platformType)
    {
        int rand;
        switch (platformType)
        {
            case PlatformType.Normal:
                for(int i = 0; i< normalPlatformList.Count; i++)
                    if (normalPlatformList[i].activeInHierarchy == false)
                        return normalPlatformList[i];
                return InstantiateObject(vars.normalPlatformPre, ref normalPlatformList);
            
            case PlatformType.Common:
                for (int i = 0; i < commonPlatformList.Count; i++)
                {
                    if (commonPlatformList[i].activeInHierarchy == false)
                        return commonPlatformList[i];
                }
                rand = Random.Range(0, vars.commonPlatformGroup.Count);
                return InstantiateObject(vars.commonPlatformGroup[rand], ref commonPlatformList);
            
            case PlatformType.Grass:
                for (int i = 0; i < grassPlatformList.Count; i++)
                {
                    if (grassPlatformList[i].activeInHierarchy == false)
                        return grassPlatformList[i];
                }
                rand = Random.Range(0, vars.grassPlatformGroup.Count);
                return InstantiateObject(vars.grassPlatformGroup[rand], ref grassPlatformList);

            case PlatformType.Winter:
                for (int i = 0; i < winterPlatformList.Count; i++)
                {
                    if (winterPlatformList[i].activeInHierarchy == false)
                        return winterPlatformList[i];
                }
                rand = Random.Range(0, vars.winterPlatformGroup.Count);
                return InstantiateObject(vars.winterPlatformGroup[rand], ref winterPlatformList);

            case PlatformType.SpikeLeft:
                for (int i = 0; i < spikePlatformLeftList.Count; i++)
                {
                    Debug.Log("spawn spike left");
                    if (spikePlatformLeftList[i].activeInHierarchy == false)
                        return spikePlatformLeftList[i];
                }
                return InstantiateObject(vars.SpikePlatformGroup[0], ref spikePlatformLeftList);

            case PlatformType.SpikeRight:
                for (int i = 0; i < spikePlatformRightList.Count; i++)
                {
                    Debug.Log("spawn spike right");
                    if (spikePlatformRightList[i].activeInHierarchy == false)
                        return spikePlatformRightList[i];
                }
                return InstantiateObject(vars.SpikePlatformGroup[1], ref spikePlatformRightList);

            default:
                return InstantiateObject(vars.normalPlatformPre, ref normalPlatformList);
        }

    }

}
