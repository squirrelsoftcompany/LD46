using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance
    {
        get
        {
            return mInstance;
        }
    }
    
    [SerializeField] private List<Level> mLevelList;

    private int mLevelIndex = 0;
    private bool mStartScreen = true;
    public static LevelManager mInstance = null;

    
    void Awake()
    {
        if (mInstance != null)
            Destroy(gameObject);
        else
            mInstance = this;
    }

    public void LoadLevel(int pLevelIndex)
    {
        mLevelIndex = pLevelIndex;
        // Load/reload scene
        Scene lScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(lScene.name);
    }

    void Update()
    {
        if(!SoundManager.Instance.Music)
        {
            SoundManager.Instance.Music = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        }
        if (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name == "Start")
        {
            SceneManager.LoadScene("main_scene");
        }
    }

    public void GetLevelData (out List<GridGeneration.Building> pBuildings,
        out List<GridGeneration.Building> pLakes,
        out List<GridGeneration.Ground> pGrounds,
        out List<GridGeneration.Props> pProps,
        out int pWidth, out int pHeight, out hex.HexCell pBasePrefab, out int pWolfCount, out int pWater, out int pFood)
    {
        pBuildings = mLevelList[mLevelIndex]._buildings;
        pLakes = mLevelList[mLevelIndex]._lakes;
        pGrounds = mLevelList[mLevelIndex]._grounds;
        pProps = mLevelList[mLevelIndex]._props;
        pWidth = mLevelList[mLevelIndex].width;
        pHeight = mLevelList[mLevelIndex].height;
        pBasePrefab = mLevelList[mLevelIndex].cellPrefab;
        pWolfCount = mLevelList[mLevelIndex].wolfNumber;
        pWater = mLevelList[mLevelIndex].water;
        pFood = mLevelList[mLevelIndex].food;
    }
    
    public void goToEndGame()
    {
        SceneManager.LoadScene("End");
    }

    public void goToBadEndGame()
    {
        SceneManager.LoadScene("EndBad");
    }

    public void replay()
    {
        SceneManager.LoadScene("Final_scene");
    }
}
