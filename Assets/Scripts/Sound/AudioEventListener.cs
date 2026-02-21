using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioEventListener : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundClip;
    [SerializeField] private AudioClip wheelTurnClip;
    [SerializeField] private AudioClip gasSoundClip;
    [SerializeField] private int gasFloor = 3;
    [SerializeField] private AudioClip elevatorClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip openScreenClip;
    [SerializeField] private AudioClip candleCollectedClip;
    [SerializeField] private AudioClip candleLightClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip woodFallingClip;
    [SerializeField] private AudioClip doorOpeningClip;
    [SerializeField] private AudioClip onWinClip;
    [SerializeField] private float backgroundMusicVolume = 0.3f;
    [SerializeField] private AudioClip pauseMenuClip;
    [SerializeField] private float playFallingWoodDelay = 1f;
    [SerializeField] private AudioClip elevatorDoorClip;
    [SerializeField] [Range(0f, 1f)] private float elevatorDoorVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float elevatorMoveVolume = 1f;
    private Audio _currentElevatorDoorSound;
    private Audio _currentElevatorSound;
    private Audio _currentPauseMusic;
    private Audio _currentGasSound;
    private Audio _currentWinMusic;
    //private Tween _respawnTween; //track the respawn sound
    private void OnEnable()
    {
        //Gas & Wheel InterActives:
        EventManagement.OnGasWheelActivated += PlayWheelSound;
        EventManagement.OnElevatorArrived += PlayGasSound;
        //ElevatorMovement:
        EventManagement.OnElevatorPrepareToMove += PlayElevatorSound;
        EventManagement.OnElevatorArrived += HandleElevatorArrived;
        EventManagement.OnTakingDamage += PlayHitSound;
        //Candle interactive:
        EventManagement.OnItemCollected += PlayCandleCollectedSound;
        EventManagement.OnLightInteracted += PlayCandleLightSound;
        //Open, Game over & background screens:
        EventManagement.OnPlayerDied += PlayGameOverSound;
        EventManagement.OnMainMenuActive += PlayOpenScreenMusic;
        EventManagement.OnGameplayStarted += PlayBackgroundSound;
        //Pausing resuming:
        EventManagement.OnGamePaused += PauseBackgroundMusic;
        EventManagement.OnGameResumed += ResumeBackgroundMusic;

        EventManagement.OnFirstCollectedItem += PlayWoodFallingClip;
        EventManagement.OnLastCollectedItem += PlayDoorOpenClip;

        EventManagement.OnWin += PlayOnWinClip;

        EventManagement.OnElevatorEnter += PlayElevatorDoorSound;
    }

    private void OnDisable()
    {
        //Gas & Wheel InterActives:
        EventManagement.OnGasWheelActivated -= PlayWheelSound;
        EventManagement.OnElevatorArrived -= PlayGasSound;
        //ElevatorMovement:
        EventManagement.OnElevatorPrepareToMove -= PlayElevatorSound;
        EventManagement.OnElevatorArrived -= HandleElevatorArrived;
        EventManagement.OnTakingDamage -= PlayHitSound;
        //Candle interactive:
        EventManagement.OnItemCollected -= PlayCandleCollectedSound;
        EventManagement.OnLightInteracted -= PlayCandleLightSound;
        //Open, Game over & background screens:
        EventManagement.OnPlayerDied -= PlayGameOverSound;
        EventManagement.OnMainMenuActive -= PlayOpenScreenMusic;
        EventManagement.OnGameplayStarted -= PlayBackgroundSound;
        //Pausing resuming:
        EventManagement.OnGamePaused -= PauseBackgroundMusic;
        EventManagement.OnGameResumed -= ResumeBackgroundMusic;
        
        EventManagement.OnFirstCollectedItem -= PlayWoodFallingClip;
        EventManagement.OnLastCollectedItem -= PlayDoorOpenClip;
        
        EventManagement.OnWin -= PlayOnWinClip;
        
        EventManagement.OnElevatorEnter -= PlayElevatorDoorSound;
    }

    private void StopWinMusicIfNeeded()
    {
        if (_currentWinMusic != null)
        {
            _currentWinMusic.StopAndReturn();
            _currentWinMusic = null;
        }
    }
    private void PlayOnWinClip()
    {
        // 1. עוצרים את מוזיקת הרקע לחלוטין
        SoundManager.Instance.StopMusic(); 
        
        // 2. עוצרים סאונדים אחרים שאולי פועלים ברקע כדי שיהיה שקט למוזיקת הניצחון
        StopGasSoundIfNeeded();
        StopElevatorSoundIfNeeded();
        StopPauseMusicIfNeeded();

        // 3. מנגנים את מוזיקת הניצחון בלופ ושומרים את הרפרנס
        if (onWinClip != null)
        {
            _currentWinMusic = SoundManager.Instance.PlaySound(onWinClip, backgroundMusicVolume, true);
        }
    }
    private void PlayElevatorDoorSound(int floor)
    {
        // קודם עוצרים את הסאונד אם הוא עדיין מנגן (מהפעם הקודמת)
        StopElevatorDoorSoundIfNeeded();
        
        // מנגנים מחדש ושומרים את הרפרנס. זה לא לופ (false)
        if (elevatorDoorClip != null)
        {
            _currentElevatorDoorSound = SoundManager.Instance.PlaySound(elevatorDoorClip, elevatorDoorVolume, false);
        }
    }
    private void StopElevatorDoorSoundIfNeeded()
    {
        if (_currentElevatorDoorSound != null)
        {
            _currentElevatorDoorSound.StopAndReturn();
            _currentElevatorDoorSound = null;
        }
    }
    private void PlayDoorOpenClip()
    {
        DOVirtual.DelayedCall(playFallingWoodDelay, () =>
        {
            SoundManager.Instance.PlaySound(doorOpeningClip);
        });
    }
    private void PlayWoodFallingClip()
    {
        DOVirtual.DelayedCall(playFallingWoodDelay, () =>
        {
            SoundManager.Instance.PlaySound(woodFallingClip);
        });
    }
    private void PlayCandleLightSound()
    {
        SoundManager.Instance.PlaySound(candleLightClip);
    }
    private void PlayCandleCollectedSound()
    {
        SoundManager.Instance.PlaySound(candleCollectedClip);
    }
    private void PlayHitSound()
    {
        SoundManager.Instance.PlaySound(hitClip);
    }
    private void PlayElevatorSound(int floor)
    {
        if (floor == gasFloor)
        {
            StopGasSoundIfNeeded();
        }
        // מחלישים את מוזיקת הרקע לעשירית מהעוצמה שלה במשך חצי שנייה
        //SoundManager.Instance.FadeMusicVolumeDown(0.1f, 0.5f); 
        // ------------------
        
        StopElevatorSoundIfNeeded();
        _currentElevatorSound = SoundManager.Instance.PlaySound(elevatorClip, 1f, true);
    }
    
    private void HandleElevatorArrived(int floor)
    {
        // 1. עוצרים את סאונד המעלית שנוסעת
        StopElevatorSoundIfNeeded();
        
        // --- השינוי כאן ---
        // 2. מחזירים את מוזיקת הרקע לעוצמה המקורית במשך חצי שנייה
        SoundManager.Instance.FadeMusicVolumeUp(0.5f);
        // ------------------
        
        // 3. מנגנים את סאונד פתיחת דלתות המעלית!
        PlayElevatorDoorSound(floor);
        
        // 4. בודקים אם הגענו לקומה 3 ומפעילים את הגז במידת הצורך
        PlayGasSound(floor);
    }
    private void StopElevatorSoundIfNeeded()
    {
        if (_currentElevatorSound != null)
        {
            _currentElevatorSound.StopAndReturn();
            _currentElevatorSound = null;
        }
    }
    private void PlayWheelSound()
    {
        SoundManager.Instance.PlaySound(wheelTurnClip);
    }

    private void PlayGasSound(int i)
    {
        if (i == gasFloor)
        {
            // אם הסאונד עדיין לא מתנגן, נפעיל אותו בלופ
            if (_currentGasSound == null)
            {
                _currentGasSound = SoundManager.Instance.PlaySound(gasSoundClip, 1f, true);
            }
        }
    }
    private void StopGasSoundIfNeeded()
    {
        if (_currentGasSound != null)
        {
            _currentGasSound.StopAndReturn();
            _currentGasSound = null;
        }
    }
    
    private void PlayBackgroundSound()
    {
        StopPauseMusicIfNeeded();
        StopGasSoundIfNeeded();
        StopElevatorSoundIfNeeded(); // הוספנו ליתר ביטחון
        StopWinMusicIfNeeded();      // <-- עוצר את מוזיקת הניצחון בריסטרט!
        StopElevatorDoorSoundIfNeeded(); // <-- הוספנו
        SoundManager.Instance.musicSource.DOKill(); // <-- עוצר פיידים רצים
        SoundManager.Instance.PlayMusic(backgroundClip, backgroundMusicVolume);
    }

    private void PlayOpenScreenMusic()
    {
        StopPauseMusicIfNeeded();
        StopGasSoundIfNeeded();
        StopElevatorSoundIfNeeded(); // הוספנו ליתר ביטחון
        StopWinMusicIfNeeded();      // <-- עוצר את מוזיקת הניצחון בחזרה לתפריט!
        StopElevatorDoorSoundIfNeeded(); // <-- הוספנו
        SoundManager.Instance.musicSource.DOKill(); // <-- עוצר פיידים רצים
        SoundManager.Instance.PlayMusic(openScreenClip, backgroundMusicVolume);
    }
    private void PlayGameOverSound()
    {
        StopGasSoundIfNeeded();
        SoundManager.Instance.StopMusic(); // Stop completely on Game Over
        SoundManager.Instance.PlaySound(gameOverClip);
    }
    
    private void PauseBackgroundMusic()
    {
        SoundManager.Instance.PauseMusic(); // עוצר את מוזיקת המשחק
        
        if (pauseMenuClip != null)
        {
            // מנגנים את מוזיקת הפוז בלופ (true) ושומרים את הרפרנס אליה
            _currentPauseMusic = SoundManager.Instance.PlaySound(pauseMenuClip, backgroundMusicVolume, true);
        }
    }

    private void ResumeBackgroundMusic()
    {
        StopPauseMusicIfNeeded();
        SoundManager.Instance.ResumeMusic();
    } 
    private void StopPauseMusicIfNeeded()
    {
        if (_currentPauseMusic != null)
        {
            _currentPauseMusic.StopAndReturn(); 
            _currentPauseMusic = null;
        }
    }
}
