using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] private int mFoodCost = 0;
    [SerializeField] private int mWaterCost = 0;

    public int FoodCost
    {
        get
        {
            return mFoodCost;
        }
        set
        {
            mFoodCost = value;
        }
    }

    public int WaterCost
    {
        get
        {
            return mWaterCost;
        }
        set
        {
            mWaterCost = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
