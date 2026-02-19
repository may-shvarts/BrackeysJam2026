using System.Collections;
using UnityEngine;

/// <summary>
/// Interactable wheel: when the player activates it (via Enter),
/// it disables the gas hazard and fades out the gas VFX quickly.
/// </summary>
public class WheelGasSwitch : MonoBehaviour
{
    [Header("Gas References")]
    public ParticleSystem gasVfx;
    public Collider2D gasTrigger;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.25f; 
    [SerializeField] private bool clearParticlesAtEnd = true;

    private bool used = false;
    private Coroutine fadeRoutine;

    public void Interact()
    {
        if (used) return;
        used = true;

        EventManagement.OnGasWheelActivated?.Invoke();

        // Disable hazard immediately (so player stops taking damage right away)
        if (gasTrigger != null)
            gasTrigger.enabled = false;

        // Fade visuals quickly
        if (gasVfx != null)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOutGas());
        }
    }

    private IEnumerator FadeOutGas()
    {
        var emission = gasVfx.emission;
        float startRate = emission.rateOverTimeMultiplier;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            emission.rateOverTimeMultiplier = Mathf.Lerp(startRate, 0f, k);
            yield return null;
        }

        emission.rateOverTimeMultiplier = 0f;

        if (clearParticlesAtEnd)
            gasVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        else
            gasVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}