using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Alteruna;

public class InitiatePlayer : MonoBehaviour
{
    public GameObject camera;

    void Start() {
        
        Alteruna.Avatar _avatar = GetComponent<Alteruna.Avatar>();

        if (!_avatar.IsMe) return;


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
