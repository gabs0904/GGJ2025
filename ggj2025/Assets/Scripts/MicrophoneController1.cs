using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MicrophoneController1 : MonoBehaviour
{
    #region objects

    public ParticleSystem explosionEffect;
    public GameObject bomb;
    private AudioSource audioSource;

    public TMP_Text loudnessText;
    public TMP_Text durationText;

    #endregion

    #region parameters

    public int maxCharge = 1000;

    public float threshold = 0.005f; // Minimum loudness to trigger scaling
    public float sensitivity = 5f; // Controls how quickly the object grows

    #endregion

    #region private

    private bool hasExploded = false;
    private bool isMicInitialized = false;
    private int charge = 0;

    private float activeSoundDuration = 0f; // Tracks how long loudness is above threshold
    private float lastLoudnessTime = 0f;    // Tracks the last time loudness exceeded threshold
    private int previousCharge = 0;         // Tracks the previous charge state

    #endregion

    private Vector3 baseScale = Vector3.one; // Initial size of the object

    private float fartDuration;

    void Start()
    {

        // Set up the AudioSource to use the microphone
        audioSource = gameObject.AddComponent<AudioSource>();
        if (Microphone.devices.Length > 0)
        {
            string microphoneName = Microphone.devices[0]; // Use the first detected microphone
            Debug.Log("Using microphone: " + microphoneName);
            // Assign the microphone as the AudioClip
            audioSource.clip = Microphone.Start(null, true, 1, 44100); // Default mic
            audioSource.loop = true;
            StartCoroutine(WaitForMicToInitialize());
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    IEnumerator WaitForMicToInitialize()
    {
        while (!(Microphone.GetPosition(null) > 0))
        {
            yield return null; // Wait until the mic is ready
        }

        audioSource.Play();
        isMicInitialized = true; // Mic is now initialized
        Debug.Log("Microphone initialized.");
    }

/*
  void Update()
{
    if (!isMicInitialized) return;

    // Get the current loudness of the microphone input
    float loudness = GetLoudness();
    Debug.Log($"Loudness: {loudness}");

    // Store the previous charge for comparison
    //int currentCharge = charge;

    // Check loudness and calculate charge
    if (loudness > threshold)
    {
        fartDuration += Time.deltaTime;

        // Update the charge based on loudness and time
        //charge = Mathf.Min(charge + Mathf.RoundToInt(loudness * sensitivity * 100 * Time.deltaTime), maxCharge);
        //Debug.Log($"Charge: {charge}/{maxCharge}");

        // // Trigger explosion when charge reaches maxCharge
        // if (charge >= maxCharge && !hasExploded)
        // {
        //     TriggerExplosion();
        // }

        // Scale object based on loudness
        //float scaleFactor = 1 + (loudness * sensitivity);
        //transform.localScale = baseScale * scaleFactor;
    }
    else
    {
        fartDuration = 0;
        transform.localScale = baseScale;

        // Gradually reduce charge if there's no loudness
        if (Time.time - lastLoudnessTime > 0.5f && charge > 0)
        {
            charge = Mathf.Max(charge - Mathf.RoundToInt(sensitivity * 5 * Time.deltaTime), 0);
            activeSoundDuration = 0f; // Reset active sound duration
        }
    }

    // Check if charge dropped to zero after being higher
    if (charge == 0 && currentCharge > 0)
    {
        Debug.Log("Charge depleted, triggering explosion.");
        TriggerExplosion();
    }

    // Update particle intensity based on charge
    UpdateParticleIntensity();

    // Update previous charge at the end of the frame
    previousCharge = currentCharge;

    // Debugging: Track activeSoundDuration and charge
    Debug.Log($"ActiveSoundDuration: {activeSoundDuration}, Previous Charge: {previousCharge}, Current Charge: {charge}");
}
*/
    void Update() {
        if (!isMicInitialized) return;

        durationText.text = fartDuration + "s";

        float loudness = GetLoudnessInDecibels();

        if (loudness >= threshold) {
            fartDuration += Time.deltaTime;
        } else {
            EndFart(fartDuration);

            fartDuration = 0;
        }
    }

    void EndFart(float power) {
        // ove ke spawnirat bomb 
    }

    // void TriggerExplosion()
    // {
    //     // Play the explosion particle effect
    //     if (explosionEffect != null)
    //     {
    //         explosionEffect.Play();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Explosion effect is not assigned.");
    //     }

    //     hasExploded = true; // Prevent triggering it again until reset
    //     Debug.Log("Explosion triggered!");

    //     Destroy(bomb);
    // }

    void UpdateParticleIntensity() {
        if (explosionEffect != null) {
            // Adjust particle properties based on the charge level
            ParticleSystem.MainModule main = explosionEffect.main;
            ParticleSystem.EmissionModule emission = explosionEffect.emission;

            float normalizedCharge = (float)charge / maxCharge; // Normalize charge between 0 and 1

            Debug.Log($"Normalized Charge: {normalizedCharge}");

            // Scale start size and start speed based on charge
            main.startSize = Mathf.Lerp(0.5f, 3.0f, normalizedCharge); // From 0.5 to 3.0
            main.startSpeed = Mathf.Lerp(1.0f, 10.0f, normalizedCharge); // From 1.0 to 10.0
            emission.rateOverTime = Mathf.Lerp(10, 100, normalizedCharge); // From 10 particles/sec to 100

            Debug.Log($"Particle Size: {main.startSize.constant}, Speed: {main.startSpeed.constant}, Emission Rate: {emission.rateOverTime.constant}");
        }
    }

    float GetLoudnessInDecibels()
    {
        float[] data = new float[16]; // Array to hold audio samples

        if (Microphone.GetPosition(null) > 0) {
            audioSource.GetOutputData(data, 0); // Fill array with raw audio data
        } else {
            return -80f; // Return a very low dB value if the microphone isn't active
        }

        float rms = 0f; // Root Mean Square value
        for (int i = 0; i < data.Length; i++) {
            rms += data[i] * data[i]; // Sum of squares of the samples
        }

        rms = Mathf.Sqrt(rms / data.Length); // Square root of the average

        // Convert RMS to decibels
        float db = 20f * Mathf.Log10(rms / 0.1f); // Reference value is set to 0.1f (adjust if necessary)

        loudnessText.text = Mathf.Clamp(db, -80f, 0f) + "db";

        return Mathf.Clamp(db, -80f, 0f); // Clamp between -80 dB (silence) and 0 dB (maximum loudness)
    }
}
