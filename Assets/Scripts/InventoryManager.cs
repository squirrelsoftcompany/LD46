using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    public int Food
    {
        get
        {
            return mFood;
        }
        set
        {
            mFood = value;
        }
    }
    public int Water
    {
        get
        {
            return mWater;
        }
        set
        {
            mWater = value;
        }
    }

    public static InventoryManager m_instance = null;
    [SerializeField] private int mFood = 0;
    [SerializeField] private int mWater = 0;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_instance != null)
            Destroy(gameObject);
        else
            m_instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addFood(int pFood)
    {
        mFood = mFood + pFood;
    }

    public void addWater(int pWater)
    {
        mWater = mWater + pWater;
    }

    public void subFood(int pFood)
    {
        mFood = (mFood - pFood)<0 ? 0 : (mFood - pFood);
    }

    public void subWater(int pWater)
    {
        mWater = (mWater - pWater)<0 ? 0 : (mWater - pWater);
    }
}
