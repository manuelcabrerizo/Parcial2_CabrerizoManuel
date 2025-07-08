using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static Action onPauseAll;
    public static Action onResumeAll;
    public static Action onPlayMusic;
    public static Action onStopMusic;
    public static Action onPauseMusic;
    public static Action<AudioClip> onPlayClip;
    public static Action<AudioClip, Vector3, float, float> onPlayClip3D;

    [SerializeField] private VolumeDataSO volumeData;
    [SerializeField] private SoundClipsSO soundClips;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    private AudioSource musicAudioSource;
    private IObjectPool<AudioSource> pool;

    private void Start()
    {
        UISettings.onMusicSliderChange += OnMusicSliderChange;
        UISettings.onSfxSliderChange += OnSfxSliderChange;
        onPauseAll += PauseAll;
        onResumeAll += ResumeAll;
        onPlayMusic += PlayMusic;
        onStopMusic += StopMusic;
        onPauseMusic += PauseMusic;
        onPlayClip += PlayClip;
        onPlayClip3D += PlayClip3D;

        mixer.SetFloat("SfxVolume", Utils.LinearToDecibel(volumeData.Sfx));
        mixer.SetFloat("MusicVolume", Utils.LinearToDecibel(volumeData.Music));
        mixer.SetFloat("MasterVolume", Utils.LinearToDecibel(volumeData.Master));

        musicAudioSource = GetComponent<AudioSource>();
        pool = new ObjectPool<AudioSource>(
            CreateAudioSource, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            collectionCheck, defaultCapacity, maxSize);

        //musicAudioSource.clip = soundClips.music;
        //musicAudioSource.loop = true;
        //musicAudioSource.Stop();
    }
    private void OnDestroy()
    {
        UISettings.onMusicSliderChange -= OnMusicSliderChange;
        UISettings.onSfxSliderChange -= OnSfxSliderChange;
        onPauseAll -= PauseAll;
        onResumeAll -= ResumeAll;
        onPlayMusic -= PlayMusic;
        onStopMusic -= StopMusic;
        onPauseMusic -= PauseMusic;
        onPlayClip -= PlayClip;
        onPlayClip3D -= PlayClip3D;

        StopAllCoroutines();
        pool.Clear();
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

    private void PlayClip(AudioClip clip)
    {
        AudioSource audioSource = pool.Get();
        audioSource.transform.position = Vector3.zero;
        audioSource.spatialBlend = 0.0f;
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(ReleaseAudioSourceIfFinish(audioSource));
    }


    private void PlayClip3D(AudioClip clip, Vector3 position, float minDist, float maxDist)
    {
        AudioSource audioSource = pool.Get();
        audioSource.transform.position = position;
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = minDist;
        audioSource.maxDistance = maxDist;
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(ReleaseAudioSourceIfFinish(audioSource));
    }

    private void PauseAll()
    {
        mixer.SetFloat("MasterVolume", -80);
    }

    private void ResumeAll()
    {
        mixer.SetFloat("MasterVolume", 0);
    }

    private IEnumerator ReleaseAudioSourceIfFinish(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        pool.Release(audioSource);
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
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
    
    private void OnSfxSliderChange(float value)
    {
        volumeData.Sfx = value;
        mixer.SetFloat("SfxVolume", Utils.LinearToDecibel(volumeData.Sfx));
    }

    private void OnMusicSliderChange(float value)
    {
        volumeData.Music = value;
        mixer.SetFloat("MusicVolume", Utils.LinearToDecibel(volumeData.Music));
    }
}
