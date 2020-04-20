using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Journey : MonoBehaviour
{
    [SerializeField]
    private List<PointOfInterest> mPOIList;
    [SerializeField]
    private GameObject mWorldCamel;
    [SerializeField]
    private int mCurrentPOIIndex = 0;
    private bool mMapVisible = true;

    //UI
    [SerializeField]
    private Text mTextFood;
    [SerializeField]
    private Text mTextWater;

    //Inspector variables for test
    [SerializeField]
    private bool mShow = false;

    //TODO : Verifier si l'inventaire est suffisant pour pouvoir aller au point suivant

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GoToNextPOI()
    {
        if(CanGoNext())
        {
            mCurrentPOIIndex++;
            InventoryManager.Instance.subFood(mPOIList[mCurrentPOIIndex].FoodCost);
            InventoryManager.Instance.subWater(mPOIList[mCurrentPOIIndex].WaterCost);
            mWorldCamel.GetComponent<Animator>().SetInteger("position", mCurrentPOIIndex);
            //Change current POI look here if you want
        }
    }

    public void GoInPOI()
    {
        return;
        //Load and show map
    }

    public void ShowMap(bool pShow) {
        mShow = pShow;
        transform.GetComponent<Image>().enabled = pShow;
        foreach (Image child in GetComponentsInChildren<Image>())
        {
            child.enabled = pShow;
        }
        foreach (Text child in GetComponentsInChildren<Text>())
        {
            child.enabled = pShow;
        }
        mMapVisible = pShow;
    }

    public PointOfInterest GetCurrentPOI()
    {
        return mPOIList[mCurrentPOIIndex];
    }

    public void Update()
    {
        if (mMapVisible != mShow)
            ShowMap(mShow);

        mTextFood.text = InventoryManager.Instance.Food.ToString();
        mTextWater.text = InventoryManager.Instance.Water.ToString();
    }

    private bool CanGoNext()
    {
        //If last POI
        if (mCurrentPOIIndex+1 >= mPOIList.Count)
            return false;
        
        if ( InventoryManager.Instance.Food >= mPOIList[mCurrentPOIIndex+1].FoodCost
            && InventoryManager.Instance.Food >= mPOIList[mCurrentPOIIndex+1].WaterCost)
        {
            return true;
        }
        return false;
    }
}
