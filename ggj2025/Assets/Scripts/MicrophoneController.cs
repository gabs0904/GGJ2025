using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneController : MonoBehaviour
{
    public float threshold = 0.01f; // Minimum loudness to trigger scaling
    public float sensitivity = 5f; // Controls how quickly the object grows
    public Vector3 baseScale = Vector3.one; // Initial size of the object

    private AudioSource audioSource;
    public ParticleSystem explosionEffect;
    private bool hasExploded = false;


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
            while (Microphone.GetPosition(null) <= 0) { } // Wait for mic to initialize
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        // Get the current loudness of the microphone input
        float loudness = GetLoudness();
        Debug.Log("Loudness: " + loudness);

        // If loudness drops below threshold, trigger explosion
        if (loudness < 0.001f && !hasExploded) // You can adjust this value for better triggering
        {
            Debug.LogWarning($"Explosion effect happens at loudnes {loudness}");
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
    }

    // Function to calculate the loudness of the microphone input
    float GetLoudness()
    {
        float[] data = new float[256]; // Array to hold audio samples
        audioSource.GetOutputData(data, 0); // Fill array with raw audio data

        float sum = 0f;
        foreach (float sample in data) {
            sum += sample * sample; // Square each sample
        }
        float rms = Mathf.Sqrt(sum / data.Length); // Take the square root of the average
        float db = 20f * Mathf.Log10(rms);
        return db;
    }
}
