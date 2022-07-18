using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int targetFrameRate = -1;
    public static GameManager Instance;
    private GameData data;
    private ManagerVars vars;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnlocked;
    private int diamondCount;// 总钻石数

    public bool IsGameStarted { get; set; } // 游戏是否开始
    public bool IsGameOver { get; set; } // 游戏是否结束

    public bool IsGamePause { set; get; } // 游戏是否暂停
    public bool isCharacterMove { set; get; } // 角色是否开始移动
    public int gameScore; // 每局游戏成绩
    public int gameDiamond; // 每局游戏获得的钻石
    public int BestScore; // 游戏最高分
    private void Awake()
    {
        //ResetData();
        Application.targetFrameRate = targetFrameRate;
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.AddDiamond, AddGameDiamond);
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener<int>(EventDefine.ItemPurchase, ItemPurchase);
        if (GameData.IsReplayGame)
        {
            IsGameStarted = true;
        }
        //ResetData();
        InitGameData();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddGameDiamond);
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        EventCenter.RemoveListener<int>(EventDefine.ItemPurchase, ItemPurchase);
    }


    private void AddGameScore()
    {
        if (IsGameOver || IsGamePause || !IsGameStarted) return;
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreText, gameScore);
    }

    public int getScore()
    {
        return gameScore;
    }

    public void SaveScore()
    {
        Debug.Log(bestScoreArr);
        if(gameScore < bestScoreArr[2])
        {
            return;
        }
        else
        {
            for(int i = 0; i < bestScoreArr.Count(); i++)
            {
                if(gameScore > bestScoreArr[i])
                {
                    for(int j = bestScoreArr.Count()-1; j > 0; j--)
                    {
                        print("j=" + j);
                        bestScoreArr[j] = bestScoreArr[j - 1];
                    }
                    bestScoreArr[i] = gameScore;
                    break;
                }
            }
        }
        Debug.Log(bestScoreArr);
        Save();
    }



    public void AddGameDiamond()
    {
        gameDiamond++;
        print("gameDiamond=" + gameDiamond);
        EventCenter.Broadcast<int>(EventDefine.UpdateDiamondText, gameDiamond);
        //Save();
    }

    public bool GetSkinUnlocked(int index)
    {
        return skinUnlocked[index];
    }

    public void UpdateAllDiamond(int price)
    {
        diamondCount += price;
        print("UpdateAllDiamond at " + price);
        Save();
    }

    //public int GetBestScore()
    //{
    //    return bestScoreArr[0];
    //}

    public void SetSkinUnlocked(int index)
    {
        skinUnlocked[index] = true;
    }
    public int GetAllDiamond()
    {
        return diamondCount;
    }


    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }
    private void ItemPurchase(int index)
    {
        /// 钻石够,则：
        /// 1.使用广播从GameManager里扣除钻石,
        /// 2.使用广播从GameManager里将解锁数组相对应index设置成true;
        diamondCount -= int.Parse(vars.skinPriceList[index]);
        skinUnlocked[index] = true;
        Save();
    }

    public void SetMusic(bool value)
    {
        isMusicOn = value;
        Save();
    }

    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }



    private void InitGameData()
    {
        Read();

        if(data == null)
        {
            // 没有数据那就属于是第一次游戏，则
            print("data 为空.");
            isFirstGame = true;

        }
        else
        {
            // 不是第一次游戏，游戏数据存在
            print("data 不为空.");
            isFirstGame = data.GetIsFirstGame();
        }
        if (isFirstGame)
        {
            print("isFirstGame");
            // 第一次游戏，没有数据，那么生成一下数据
            data = new GameData();
            ResetData();       
        }
        else
        {
            isFirstGame = data.GetIsFirstGame();
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            skinUnlocked = data.GetSkinUnlock();
            diamondCount = data.GetDiamondCount();
        }
}
    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data"))
            {
                data.SetBestScoreArr(bestScoreArr);
                data.SetDiamondCount(diamondCount);
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetSelectSkin(selectSkin);
                data.SetSkinUnlock(skinUnlocked);
                bf.Serialize(fs, data);
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))
            {

                data = (GameData)bf.Deserialize(fs);

            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnlocked = new bool[vars.skinNamelist.Count];
        skinUnlocked[0] = true;
        diamondCount = 0;
        Save();
    }
}
