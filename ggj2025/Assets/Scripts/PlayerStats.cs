using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 1000;
    public float health = 1000;

    public Image healthbar;

    private void Start() {
        healthbar = GameObject.Find("Healthbar").GetComponent<Image>();
    }

    public void Damage(float amount) {
        health -= amount;

        healthbar.fillAmount = health / maxHealth;

        if (health <= 0) {
            Destroy(gameObject);
            print("Player died");
        }
    }
}
