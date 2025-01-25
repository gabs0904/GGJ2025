using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneController : MonoBehaviour
{
    public float threshold = 0.005f; // Minimum loudness to trigger scaling
    public float sensitivity = 5f; // Controls how quickly the object grows
    public Vector3 baseScale = Vector3.one; // Initial size of the object

    private AudioSource audioSource;
    public ParticleSystem explosionEffect;
    private bool hasExploded = false;
    private bool isMicInitialized = false;
    public GameObject bomb;
    public int charge = 0;
    public int maxCharge = 100;

    private float activeSoundDuration = 0f; // Tracks how long loudness is above threshold
    private float lastLoudnessTime = 0f;    // Tracks the last time loudness exceeded threshold

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

    void Update()
    {
        if (!isMicInitialized) return;

        // Get the current loudness of the microphone input
        float loudness = GetLoudness();
        Debug.Log("Loudness: " + loudness);

        // Check if loudness exceeds the threshold
        if (loudness > threshold)
        {
            activeSoundDuration += Time.deltaTime; // Increase duration while above threshold
            lastLoudnessTime = Time.time;         // Update last loudness time

            // Increment charge based on duration above threshold
            charge = Mathf.Min((int)(activeSoundDuration * 10), maxCharge); // Scale charge by time
            Debug.Log($"Charge: {charge}/{maxCharge}");

            // Trigger explosion if charge reaches maxCharge
            if (charge >= maxCharge && !hasExploded)
            {
                TriggerExplosion();
            }

            // Scale object based on loudness
            float scaleFactor = 1 + (loudness * sensitivity);
            transform.localScale = baseScale * scaleFactor;
        }
        else
        {
            // Reset object scale when loudness is below threshold
            transform.localScale = baseScale;

            // Reset duration if too much time has passed since last loudness
            if (Time.time - lastLoudnessTime > 0.5f) // E.g., reset after 0.5 seconds of silence
            {
                activeSoundDuration = 0f;
            }
        }

        // Update particle intensity based on charge
        UpdateParticleIntensity();
    }

    void TriggerExplosion()
    {
        // Play the explosion particle effect
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }
        else
        {
            Debug.LogWarning("Explosion effect is not assigned.");
        }

        hasExploded = true; // Prevent triggering it again until reset
        Debug.Log("Explosion triggered!");

        Destroy(bomb);
    }

    void UpdateParticleIntensity()
    {
        if (explosionEffect != null)
        {
            // Adjust particle properties based on the charge level
            ParticleSystem.MainModule main = explosionEffect.main;
            ParticleSystem.EmissionModule emission = explosionEffect.emission;

            float normalizedCharge = (float)charge / maxCharge; // Normalize charge between 0 and 1

            // Scale start size and start speed based on charge
            main.startSize = Mathf.Lerp(0.5f, 3.0f, normalizedCharge); // From 0.5 to 3.0
            main.startSpeed = Mathf.Lerp(1.0f, 10.0f, normalizedCharge); // From 1.0 to 10.0
            emission.rateOverTime = Mathf.Lerp(10, 100, normalizedCharge); // From 10 particles/sec to 100
        }
    }

    // Function to calculate the loudness of the microphone input
    float GetLoudness()
    {
        float[] data = new float[256]; // Array to hold audio samples

        if (Microphone.GetPosition(null) > 0)
        {
            audioSource.GetOutputData(data, 0); // Fill array with raw audio data
        }
        else
        {
            return 0f; // Return 0 if microphone hasn't started recording yet.
        }

        float sum = 0f;

        for (int i = 0; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]); // Sum the absolute values of the samples
        }

        return sum / data.Length; // Return average loudness
    }
}
