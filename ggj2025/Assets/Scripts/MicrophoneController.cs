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
            // audioSource.mute = true; // Mute the mic so we don't hear it
            // Wait for the microphone to start
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

        // Trigger explosion when loudness exceeds 0.08
        if (loudness > 0.18f && !hasExploded)
        {
            Debug.Log($"Explosion effect happens at loudness {loudness}");
            TriggerExplosion();
        }

        // Check if loudness exceeds the threshold (to scale object)
        if (loudness > threshold)
        {
            // Scale object based on loudness
            float scaleFactor = 1 + (loudness * sensitivity);
            transform.localScale = baseScale * scaleFactor;
        }
        else
        {
            // Reset to base scale when loudness is below threshold
            transform.localScale = baseScale;
        }
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
        Debug.Log("has exploded");

        Destroy(bomb);
    }

    // Function to calculate the loudness of the microphone input
    float GetLoudness()
    {
        float[] data = new float[256]; // Array to hold audio samples

        // Wait for the microphone to start recording and fill data.
        // It's important to wait until there's actual data from the microphone.
        if (Microphone.GetPosition(null) > 0)
        {
            audioSource.GetOutputData(data, 0); // Fill array with raw audio data
        }
        else
        {
            return 0f; // Return 0 if microphone hasn't started recording yet.
        }

        float sum = 0f;

        // Start from index 1 to skip the first sample if needed
        for (int i = 1; i < data.Length; i++)
        {
            sum += Mathf.Abs(data[i]); // Sum the absolute values of the samples starting from the second element
        }

        return sum / (data.Length - 1); // Return average loudness
    }
}
