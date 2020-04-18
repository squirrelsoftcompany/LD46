using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float mCoolDownInput = 0.3f;

    private Animator mCameraAnim;
    private float mTimerCoolDownInput;
    // Start is called before the first frame update
    void Start()
    {
        mCameraAnim = GetComponent<Animator>();
        mTimerCoolDownInput = mCoolDownInput;
    }

    // Update is called once per frame
    void Update()
    {
        mTimerCoolDownInput -= Time.deltaTime;
        if (mTimerCoolDownInput < 0)
        {
            if (Input.GetButtonDown("CamLeft"))
            {
                int lValue = mCameraAnim.GetInteger("NumeroCam");
                lValue = (lValue + 1) % 6;
                mCameraAnim.SetInteger("NumeroCam", (mCameraAnim.GetInteger("NumeroCam") + 1) % 6);
                mTimerCoolDownInput = mCoolDownInput;
            }
            else if (Input.GetButtonDown("CamRight"))
            {
                int lValue = mCameraAnim.GetInteger("NumeroCam");
                lValue = (lValue - 1) < 0 ? 5 : lValue - 1;
                mCameraAnim.SetInteger("NumeroCam", lValue);
                mTimerCoolDownInput = mCoolDownInput;
            }
        }
    }
}
