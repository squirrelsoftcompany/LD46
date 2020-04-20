using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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

    [SerializeField] private int mLevelTotalNumber;

    private int mLevelIndex = 0;
    public static LevelManager mInstance = null;
    private List<Level> mLevelList;
    
    void Awake()
    {
        if (mInstance != null)
            Destroy(gameObject);
        else
            mInstance = this;

        for (int i = 0;i< mLevelTotalNumber;++i)
        {
            mLevelList[i] = ScriptableObject.CreateInstance<Level>();
            AssetDatabase.CreateAsset(mLevelList[i], "Assets/ScriptableObjects/Levels/Level"+i+".asset");
        }
    }

    public void LoadLevel(int pLevelIndex)
    {
        mLevelIndex = pLevelIndex;
        // Load/reload scene
        Scene lScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(lScene.name);
    }

    public void GetLevelData (out List<GridGeneration.Building> pBuildings,
        out List<GridGeneration.Building> pLakes,
        out List<GridGeneration.Ground> pGrounds,
        out List<GridGeneration.Props> pProps,
        out int pWidth, out int pHeight, out hex.HexCell pBasePrefab)
    {
        pBuildings = mLevelList[mLevelIndex]._buildings;
        pLakes = mLevelList[mLevelIndex]._lakes;
        pGrounds = mLevelList[mLevelIndex]._grounds;
        pProps = mLevelList[mLevelIndex]._props;
        pWidth = mLevelList[mLevelIndex].width;
        pHeight = mLevelList[mLevelIndex].height;
        pBasePrefab = mLevelList[mLevelIndex].cellPrefab;
    }

    public void goToEndGame()
    {
        SceneManager.LoadScene("EndGameScene");
    }

    public void replay()
    {
        SceneManager.LoadScene("Final_scene");
    }
}
