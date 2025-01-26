using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInputController : MonoBehaviour
{
    public float sensitivity = 2000f;    // Adjust to scale the loudness effect
    public float threshold = 0.0004f;    // Minimum loudness to trigger scaling

    private AudioSource audioSource;
    private string selectedMic;

    void Start()
    {
        // Attach the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Dynamically get the first available microphone
        if (Microphone.devices.Length > 0)
        {
            selectedMic = Microphone.devices[0]; // Get the first microphone
            Debug.Log("Using Microphone: " + selectedMic);

            // Start recording with the selected microphone
            audioSource.clip = Microphone.Start(selectedMic, true, 10, 44100);
            audioSource.mute = true; // Mute playback to avoid feedback


            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphones detected on this device.");
        }
    }

    void Update()
    {
        // Check if audioSource is receiving input
        if (audioSource.clip != null)
        {
            float loudness = GetLoudness();
            Debug.Log($"Loudness: {loudness}");
        }
        else
        {
            Debug.LogError("No audio clip is assigned to the AudioSource.");
        }
    }

    float GetLoudness()
    {
        float[] data = new float[256]; // Array for audio samples
        audioSource.GetSpectrumData(data, 0, FFTWindow.Rectangular);

        float loudness = 0f;
        foreach (float sample in data)
        {
            loudness += Mathf.Abs(sample);
        }

        // Amplify the loudness
        loudness = loudness / data.Length * 10000f; // Multiply by a constant to amplify
        return loudness;
    }

}
