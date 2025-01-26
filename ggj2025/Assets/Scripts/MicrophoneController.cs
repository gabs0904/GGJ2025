using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MicrophoneController : MonoBehaviour {

    #region objects

    public ParticleSystem explosionEffect;
    //public GameObject bomb;

    public GameObject fartBubblePrefab;

    //public TMP_Text loudnessText;
    //public TMP_Text durationText;

    #endregion

    #region parameters

    public float threshold = -20;    // Minimum loudness to trigger scaling

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
    private float currentSize = 0;
    private float scalingRate;

    #endregion

    float temp = 0;

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

        scalingRate = (maxFartSize - startFartSize) / maxFartDuration;
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

        //durationText.text = fartDuration + "s";
        /*
        float loudness = GetLoudnessInDecibels();

        if (loudness <= threshold && fartDuration < maxFartDuration) {
            fartDuration += Time.deltaTime;
            ScaleFart();
        } else if (fartBubble != null) {

            IEnumerator endfart = EndFart(fartDuration);
            StartCoroutine(endfart);

            fartDuration = 0;
        }
        */
        temp += Time.deltaTime;

        if (temp > 5 && temp < 15) {
            fartDuration += Time.deltaTime;
            ScaleFart();
        } else if (fartBubble != null) {

            IEnumerator endfart = EndFart(fartDuration);
            StartCoroutine(endfart);

            fartDuration = 0;

            temp = 0;
        } else if (fartBubble == null) {
            // Add this extra check to ensure the fart bubble is instantiated if it gets destroyed
            ScaleFart();
        }
    }

    void ScaleFart() {
        if (fartBubble == null) {
            print("Spawned fart bubble");
            fartBubble = Instantiate(fartBubblePrefab) as GameObject;
            print(fartBubble);
            fartBubble.transform.position = transform.position;
            currentSize = startFartSize;
        }

        currentSize += scaleRate * Time.deltaTime;
        fartBubble.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    IEnumerator EndFart(float duration) {
        print("EndFart method started");
        yield return new WaitForSeconds(duration);

        if (fartBubble != null) {
            fartBubble.GetComponent<FartBubble>().Explode();
            fartBubble = null; // Only reset fartBubble here
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

        //loudnessText.text = Mathf.Clamp(db, -80f, 0f) + "db";

        return Mathf.Clamp(db, -80f, 0f); // Clamp between -80 dB (silence) and 0 dB (maximum loudness)
    }
}
