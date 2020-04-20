using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlaylist : MonoBehaviour
{
    [SerializeField] private List<AudioClip> mMusicList;
    [SerializeField] private int mCurrentIndex = 0;
    [SerializeField] private float mPauseDuration = 0;

    private float mPauseTimer = 0;
    private float mFadeTime = 5.0f;
    private bool mFadeStart = false;

    // Start is called before the first frame update
    void Start()
    {
        mPauseTimer = mPauseDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SoundManager.Instance.Music.isPlaying && mCurrentIndex < mMusicList.Count)
        {
            mPauseTimer = mPauseTimer + Time.deltaTime;
            if(mPauseTimer> mPauseDuration)
            {
                SoundManager.Instance.PlaySingle(mMusicList[mCurrentIndex]);
                mCurrentIndex++;
                mFadeStart = false;
                mPauseTimer = 0;
            }
        }
        else if (!SoundManager.Instance.Music.isPlaying && mCurrentIndex >= mMusicList.Count)
        {
            mCurrentIndex = 0;
        }
        else
        {
            if(!mFadeStart && SoundManager.Instance.Music.time > SoundManager.Instance.Music.clip.length - mFadeTime)
            {
                StartCoroutine(SoundManager.Instance.FadeOut(mFadeTime));
                mFadeStart = true;
            }
        }
    }
}
