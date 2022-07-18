using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName ="CreateManagerVarsContainer")]
//[CreateToolMenu(menuName ="test_one")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> beThemeSpriteList = new List<Sprite>();
    public List<Sprite> platformThemeSpriteList = new List<Sprite>();
    public List<Sprite> shopItemList = new List<Sprite>();
    //public List<Sprite> characterSkinSpriteList = new List<Sprite>();
    public List<Sprite> characterSkinSpriteList = new List<Sprite>();

    public List<GameObject> commonPlatformGroup = new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();
    public List<GameObject> SpikePlatformGroup = new List<GameObject>();
    public List<string> skinNamelist = new List<string>();
    public List<string> skinPriceList = new List<string>();

    public GameObject characterPre;
    public GameObject normalPlatformPre;
    public GameObject deathEffectPre;
    public GameObject diamondPre;
    public GameObject shopItemPre;

    public Sprite musicOn, musicOff;


    public AudioClip buttonClip, diamondClip, fallClip, hitClip, jumpClip;

    public float nextXPos = 0.554f, nextYPos = 0.645f;
    //public Vector3 misleadPosOffset = new Vector3(1.635f, 0.646f, 0);
    public Vector3 StartSpawnPos = new Vector3(0.0f, -2.4f, 0.0f);
}
