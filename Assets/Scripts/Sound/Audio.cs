using DG.Tweening;
using UnityEngine;

public class Audio : MonoBehaviour, IPoolable
{
    [SerializeField] private AudioSource audioSource;
    private Tween _delayedCallTween;
    public void Play(AudioClip audioClip, float volume, bool isLoop = false, int priority = 128)
    {
        _delayedCallTween?.Kill();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = 1f;
        audioSource.loop = isLoop;
        audioSource.priority = priority;
        audioSource.Play();
        //return sound to pool only if isn't loop
        if (!isLoop)
        {
            _delayedCallTween = DOVirtual.DelayedCall(audioClip.length, ReturnToPool)
                .SetLink(this.gameObject);
        }
    }
    public void Pause()
    {
        audioSource.Pause();
    }

    public void UnPause()
    {
        audioSource.UnPause();
    }

    public void StopAndReturn()
    {
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        if (_delayedCallTween != null)
        {
            _delayedCallTween.Kill();
            _delayedCallTween = null;
        }
        AudioPool.Instance.Return(this);
    }
    public void Reset()
    {
        _delayedCallTween?.Kill();
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.loop = false;
        audioSource.priority = 128;
    }
    
    //Checks if a specific clip is played 
    public bool IsPlaying(AudioClip clipToCheck)
    {
        return gameObject.activeInHierarchy && 
               audioSource != null && 
               audioSource.clip == clipToCheck && 
               audioSource.isPlaying;
    }
}