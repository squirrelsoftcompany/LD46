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
    private CanvasGroup mFade;
    [SerializeField]
    private int mCurrentPOIIndex = 0;
    private bool mMapVisible = true;
    private bool mOnNextCity = false;

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
            mOnNextCity = true;
            //Change current POI look here if you want
        }
    }

    public void GoInPOI()
    {
        if(mOnNextCity)
        {
            //StartCoroutine(FadeOutAndLoad(2)); Don't work and no idea why...
            StartCoroutine(SoundManager.Instance.FadeOut(2));
            mOnNextCity = false;
            LevelManager.Instance.LoadLevel(mCurrentPOIIndex);
            ShowMap(false);
            //StartCoroutine(FadeIn(2));
        }
        else
        {
            ShowMap(false);
        }
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
            && InventoryManager.Instance.Water >= mPOIList[mCurrentPOIIndex+1].WaterCost)
        {
            return true;
        }
        return false;
    }

    public IEnumerator FadeOutAndLoad(float pFadeTime)
    {
        while (mFade.alpha < 1)
        {
            mFade.alpha += Time.deltaTime / pFadeTime;
            yield return null;
        }
        mFade.alpha = 1;
        LevelManager.Instance.LoadLevel(mCurrentPOIIndex);
    }

    public IEnumerator FadeIn(float pFadeTime)
    {
        while (mFade.alpha > 0)
        {
            mFade.alpha -= Time.deltaTime / pFadeTime;
            yield return null;
        }
        mFade.alpha = 0;
    }
}
