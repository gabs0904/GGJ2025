using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class InitiatePlayer : NetworkBehaviour
{
    public GameObject camera;

    void Start() {
        if (!IsOwner) return;


        CinemachineFreeLook cmCam = Instantiate(camera).GetComponent<CinemachineFreeLook>();

        cmCam.Follow = transform;
        cmCam.LookAt = transform;

        CameraControl camController = cmCam.GetComponent<CameraControl>();

        camController.player = transform;
        camController.rb = transform.GetComponent<Rigidbody>();
        camController.orientation = transform.Find("Orientation");
        camController.playerModel = transform.Find("Model");

    }
}
