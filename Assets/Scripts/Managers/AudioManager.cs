using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public enum ClipType
{ 
    SFX, UI
}

public class AudioManager : MonoBehaviour
{
    public static Action onPlayMusic;
    public static Action onStopMusic;
    public static Action onPauseMusic;
    public static Action<AudioClip, ClipType> onPlayClip;
    public static Action<AudioClip, Vector3, float, float> onPlayClip3D;

    [SerializeField] private VolumeDataSO volumeData;
    [SerializeField] private SoundClipsSO soundClips;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource audioSourceSfxPrefab;
    [SerializeField] private AudioSource audioSourceUIPrefab;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    private AudioSource musicAudioSource;
    private IObjectPool<AudioSource> sfxPool;
    private IObjectPool<AudioSource> uiPool;

    private void Awake()
    {
        UISettings.onMusicSliderChange += OnMusicSliderChange;
        UISettings.onSfxSliderChange += OnSfxSliderChange;
        UISettings.onUISliderChange += OnUISliderChange;
        onPlayMusic += PlayMusic;
        onStopMusic += StopMusic;
        onPauseMusic += PauseMusic;
        onPlayClip += PlayClip;
        onPlayClip3D += PlayClip3D;

        musicAudioSource = GetComponent<AudioSource>();
        sfxPool = new ObjectPool<AudioSource>(
            CreateSfxAudioSource, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            collectionCheck, defaultCapacity, maxSize);

        uiPool =  new ObjectPool<AudioSource>(
            CreateUIAudioSource, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            collectionCheck, defaultCapacity, maxSize);

        musicAudioSource.clip = soundClips.music;
        musicAudioSource.loop = true;
        musicAudioSource.Stop();
    }

    private void Start()
    {
        mixer.SetFloat("UIVolume", Utils.LinearToDecibel(volumeData.UI));
        mixer.SetFloat("SfxVolume", Utils.LinearToDecibel(volumeData.Sfx));
        mixer.SetFloat("MusicVolume", Utils.LinearToDecibel(volumeData.Music));
        mixer.SetFloat("MasterVolume", Utils.LinearToDecibel(volumeData.Master));
    }

    private void OnDestroy()
    {
        UISettings.onMusicSliderChange -= OnMusicSliderChange;
        UISettings.onSfxSliderChange -= OnSfxSliderChange;
        UISettings.onUISliderChange -= OnUISliderChange;
        onPlayMusic -= PlayMusic;
        onStopMusic -= StopMusic;
        onPauseMusic -= PauseMusic;
        onPlayClip -= PlayClip;
        onPlayClip3D -= PlayClip3D;

        StopAllCoroutines();
        sfxPool.Clear();
        uiPool.Clear();
    }

    private void PlayMusic()
    {
        musicAudioSource.Play();
    }

    private void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    private void StopMusic()
    {
        musicAudioSource.Stop();
    }

    private void PlayClip(AudioClip clip, ClipType type)
    {
        AudioSource audioSource = null;
        switch(type)
        {
            case ClipType.SFX:
                { 
                    audioSource = sfxPool.Get();
                    audioSource.transform.position = Vector3.zero;
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip;
                    audioSource.Play();
                    StartCoroutine(ReleaseSfxAudioSourceIfFinish(audioSource));
                }
                break;
            case ClipType.UI:
                {
                    audioSource = uiPool.Get();
                    audioSource.transform.position = Vector3.zero;
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip;
                    audioSource.Play();
                    StartCoroutine(ReleaseUIAudioSourceIfFinish(audioSource));
                }
                break;
        }
    }


    private void PlayClip3D(AudioClip clip, Vector3 position, float minDist, float maxDist)
    {
        AudioSource audioSource = sfxPool.Get();
        audioSource.transform.position = position;
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = minDist;
        audioSource.maxDistance = maxDist;
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(ReleaseSfxAudioSourceIfFinish(audioSource));
    }

    private IEnumerator ReleaseSfxAudioSourceIfFinish(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        sfxPool.Release(audioSource);
    }

    private IEnumerator ReleaseUIAudioSourceIfFinish(AudioSource audioSource)
    {
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        uiPool.Release(audioSource);
    }

    private AudioSource CreateSfxAudioSource()
    {
        AudioSource audioSource = Instantiate(audioSourceSfxPrefab, transform);
        return audioSource;
    }

    private AudioSource CreateUIAudioSource()
    {
        AudioSource audioSource = Instantiate(audioSourceUIPrefab, transform);
        return audioSource;
    }

    private void OnReleaseToPool(AudioSource pooledObject)
    {
        pooledObject.enabled = false;
        pooledObject.gameObject.SetActive(false);
    }

    private void OnGetFromPool(AudioSource pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
        pooledObject.enabled = true;
        pooledObject.Stop();
    }

    private void OnDestroyPooledObject(AudioSource pooledObject)
    {
        Destroy(pooledObject);
    }
    private void OnMusicSliderChange(float value)
    {
        volumeData.Music = value;
        mixer.SetFloat("MusicVolume", Utils.LinearToDecibel(volumeData.Music));
    }

    private void OnSfxSliderChange(float value)
    {
        volumeData.Sfx = value;
        mixer.SetFloat("SfxVolume", Utils.LinearToDecibel(volumeData.Sfx));
    }

    private void OnUISliderChange(float value)
    { 
        volumeData.UI = value;
        mixer.SetFloat("UIVolume", Utils.LinearToDecibel(volumeData.UI));
    }
}
