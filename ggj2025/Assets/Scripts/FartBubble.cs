using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartBubble : MonoBehaviour
{
    public void Explode() {
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (Collider collider in colliders) {

        }
    }
}
