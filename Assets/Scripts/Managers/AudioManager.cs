using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> LevelsMusic;
    public AudioClip MenuMusic;
    public AudioClip SelectLevelMusic;
    private string currentScene;
    private static AudioManager instance = null;
    public AudioSource musicSource;
    private int baseFade = 2;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        currentScene = SceneManager.GetActiveScene().name;
        AudioEventManager.ChangeMusic += HandleChangeMusic;
        SceneManager.activeSceneChanged += HandleActiveSceneChange;
    }

    private void Start()
    {
        musicSource.volume = 0;
        musicSource.clip = MenuMusic;
        musicSource.Play();
        StartCoroutine(StartFade(this.musicSource, null, baseFade, 1));
    }

    private void HandleChangeMusic(AudioClip audioClip, string sceneName)
    {
        if (audioClip == null)
        {
            audioClip = ReturnMusicForScene(sceneName);
        }

        StartCoroutine(StartFade(this.musicSource, audioClip, baseFade, 0));
    }

    private void HandleActiveSceneChange(Scene prev, Scene loaded)
    {
        if (currentScene != loaded.name)
        {
            currentScene = loaded.name;
            HandleChangeMusic(null, currentScene);
        }
    }

    private AudioClip ReturnMusicForScene(string sceneName)
    {
        int level = 0;

        try
        {
            level = int.Parse(sceneName);
        }
        catch
        {
            switch (sceneName)
            {
                case "Menu":
                    return MenuMusic;
                case "SelectLevel":
                    return SelectLevelMusic;
                default:
                    return null;
            }
        }

        try
        {
            return LevelsMusic[level];
        }
        catch
        {
            return null;
        }
    }

    public IEnumerator StartFade(AudioSource audioSource, AudioClip toClip, float duration, float targetVolume)
    {
        if (audioSource == null)
        {
            yield break;
        }

        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (toClip != null)
        {
            audioSource.clip = toClip;
            audioSource.Play();
            StartCoroutine(StartFade(audioSource, null, 1, 1));
        }

        yield break;
    }
}
