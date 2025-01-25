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
    public int maxCharge = 1000;

    private float activeSoundDuration = 0f; // Tracks how long loudness is above threshold
    private float lastLoudnessTime = 0f;    // Tracks the last time loudness exceeded threshold
    private int previousCharge = 0;         // Tracks the previous charge state

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
    Debug.Log($"Loudness: {loudness}");

    // Store the previous charge for comparison
    int currentCharge = charge;

    // Check loudness and calculate charge
    if (loudness > threshold)
    {
        activeSoundDuration += Time.deltaTime;
        lastLoudnessTime = Time.time;

        // Update the charge based on loudness and time
        charge = Mathf.Min(charge + Mathf.RoundToInt(loudness * sensitivity * 100 * Time.deltaTime), maxCharge);
        Debug.Log($"Charge: {charge}/{maxCharge}");

        // Trigger explosion when charge reaches maxCharge
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


    void HandleStateChange()
    {
        Debug.Log($"Charge: {charge}, Previous Charge: {previousCharge}");

        // Check if charge dropped to 0 from a higher value
        if (charge == 0 && previousCharge > 0)
        {
            Debug.Log("OnChargeDepleted called!");
            OnChargeDepleted();
        }

        // Update previous charge state
        previousCharge = charge;
    }

    void OnChargeDepleted()
    {
        Debug.Log("Charge depleted. Handling the event...");
        // Add any custom logic for when the charge is depleted
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

            Debug.Log($"Normalized Charge: {normalizedCharge}");

            // Scale start size and start speed based on charge
            main.startSize = Mathf.Lerp(0.5f, 3.0f, normalizedCharge); // From 0.5 to 3.0
            main.startSpeed = Mathf.Lerp(1.0f, 10.0f, normalizedCharge); // From 1.0 to 10.0
            emission.rateOverTime = Mathf.Lerp(10, 100, normalizedCharge); // From 10 particles/sec to 100

            Debug.Log($"Particle Size: {main.startSize.constant}, Speed: {main.startSpeed.constant}, Emission Rate: {emission.rateOverTime.constant}");
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
