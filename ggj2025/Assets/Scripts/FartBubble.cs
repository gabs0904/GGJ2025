using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartBubble : MonoBehaviour
{
    public void Explode() {
        float scale = transform.lossyScale.x;
        float damage = MapValue(scale);

        Collider[] colliders = Physics.OverlapSphere(transform.position, scale * 3);

        foreach (Collider collider in colliders) {
            if (collider.gameObject.tag == "Player") {
                collider.gameObject.GetComponent<PlayerStats>().Damage(damage);
            }
        }
    }
    float MapValue(float x) {
        return (x - 0.2f) / (2f - 0.2f) * 300f;
    }

}
