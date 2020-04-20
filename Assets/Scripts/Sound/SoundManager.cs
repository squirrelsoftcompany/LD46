using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    public AudioSource Music
    {
        get
        {
            return mMusic;
        }
        set
        {
            mMusic = value;
        }
    }

    public static SoundManager m_instance = null;
    [SerializeField] private AudioSource mMusic;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
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

    public void PlayLoopSingle(AudioClip clip)
    {
        mMusic.clip = clip;
        mMusic.loop = true;
        mMusic.Play();
    }

    public void PlaySingle(AudioClip clip)
    {
        mMusic.clip = clip;
        mMusic.loop = false;
        mMusic.Play();
    }

    public IEnumerator FadeOut(float pFadeTime)
    {
        float startVolume = mMusic.volume;

        while (mMusic.volume > 0)
        {
            mMusic.volume -= startVolume * Time.deltaTime / pFadeTime;
            yield return null;
        }
        mMusic.Stop();
        mMusic.volume = startVolume;
    }

    public IEnumerator FadeIn(float pFadeTime)
    {
        mMusic.volume = 0;
        mMusic.Play();
        while (mMusic.volume < 1)
        {
            mMusic.volume += Time.deltaTime / pFadeTime;
            yield return null;
        }
    }

}
