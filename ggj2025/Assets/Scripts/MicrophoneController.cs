using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MicrophoneController : MonoBehaviour {

    #region objects

    public ParticleSystem explosionEffect;
    public GameObject bomb;

    public GameObject fartBubblePrefab;

    public TMP_Text loudnessText;
    public TMP_Text durationText;

    #endregion

    #region parameters

    public int maxCharge = 1000;

    public float threshold = 0.005f;    // Minimum loudness to trigger scaling
    public float sensitivity = 5f;      // Controls how quickly the object grows

    public float maxFartDuration = 10f;
    public float maxFartDamage = 300f;

    public float startFartSize = 0.2f;
    public float maxFartSize = 2f;

    #endregion

    #region private

    private AudioSource audioSource;
    private bool isMicInitialized = false;

    private float fartDuration;
    private GameObject fartBubble = null;

    private float scaleRate; 

    #endregion


    void Start() {
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

        scaleRate = (maxFartSize - startFartSize) / maxFartDuration;
    }

    IEnumerator WaitForMicToInitialize() {
        while (!(Microphone.GetPosition(null) > 0))
        {
            yield return null; // Wait until the mic is ready
        }

        audioSource.Play();
        isMicInitialized = true; // Mic is now initialized
        Debug.Log("Microphone initialized.");
    }

    void Update() {
        if (!isMicInitialized) return;

        durationText.text = fartDuration + "s";

        float loudness = GetLoudnessInDecibels();

        if (loudness >= threshold) {
            fartDuration += Time.deltaTime;
            ScaleFart();
        } else {
            EndFart(fartDuration);

            fartDuration = 0;
        }
    }

    void ScaleFart() {
        if (fartBubble == null) {
            fartBubble = Instantiate(fartBubblePrefab);
            fartBubble.transform.position = transform.position;
            float currentSize = 0;
        }

        currentSize += scaleRate * Time.deltaTime;
        fartBubble.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    void EndFart(float duration) {
        Invoke(fartBubble.GetComponent<FartBubble>().Explode(), duration);

        fartBubble = null;
    }

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

    float GetLoudnessInDecibels() {
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
