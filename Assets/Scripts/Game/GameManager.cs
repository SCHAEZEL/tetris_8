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
    private int diamondCount;// ����ʯ��

    public bool IsGameStarted { get; set; } // ��Ϸ�Ƿ�ʼ
    public bool IsGameOver { get; set; } // ��Ϸ�Ƿ����

    public bool IsGamePause { set; get; } // ��Ϸ�Ƿ���ͣ
    public bool isCharacterMove { set; get; } // ��ɫ�Ƿ�ʼ�ƶ�
    public int gameScore; // ÿ����Ϸ�ɼ�
    public int gameDiamond; // ÿ����Ϸ��õ���ʯ
    public int BestScore; // ��Ϸ��߷�
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
        /// ��ʯ��,��
        /// 1.ʹ�ù㲥��GameManager��۳���ʯ,
        /// 2.ʹ�ù㲥��GameManager�ｫ�����������Ӧindex���ó�true;
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
            // û�������Ǿ������ǵ�һ����Ϸ����
            print("data Ϊ��.");
            isFirstGame = true;

        }
        else
        {
            // ���ǵ�һ����Ϸ����Ϸ���ݴ���
            print("data ��Ϊ��.");
            isFirstGame = data.GetIsFirstGame();
        }
        if (isFirstGame)
        {
            print("isFirstGame");
            // ��һ����Ϸ��û�����ݣ���ô����һ������
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
