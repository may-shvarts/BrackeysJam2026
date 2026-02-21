using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField] public AudioSource musicSource; //Current background music
    private float _originalMusicVolume;
    
    //One shot music play
    public Audio PlaySound(AudioClip audioClip, float volume = 1f, bool isLoop = false)
    {
        var sound = AudioPool.Instance.Get();
        sound.Play(audioClip, volume, isLoop);
        return sound;
    }

    // For Background Music
    public void PlayMusic(AudioClip audioClip, float volume = 1f, int priority = 0)
    {
        musicSource.clip = audioClip;
        musicSource.volume = volume;
        _originalMusicVolume = volume;
        musicSource.loop = true;
        musicSource.priority = priority;
        musicSource.Play();
    }

    public void FadeMusicVolumeDown(float targetVolumeRatio = 0.3f, float duration = 0.5f)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.DOKill(); // עוצר פיידים קודמים אם היו
            float targetVolume = _originalMusicVolume * targetVolumeRatio;
            musicSource.DOFade(targetVolume, duration);
        }
    }

    // פונקציה חדשה להחזרת המוזיקה לעוצמה המקורית
    public void FadeMusicVolumeUp(float duration = 0.5f)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.DOKill(); // עוצר פיידים קודמים אם היו
            musicSource.DOFade(_originalMusicVolume, duration);
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }
}
