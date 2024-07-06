using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private int minFov = 3, maxFov = 30;
    [SerializeField] private float zoomSpeed = 10f;

    private CinemachineFreeLook freeLookCam;

    // Start is called before the first frame update
    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();

    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
                
        if (scroll != 0.0f) {
            //freeLookCam.m_Lens.FieldOfView -= scroll * zoomSpeed;

            //freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(freeLookCam.m_Lens.FieldOfView, minFov, maxFov);

            freeLookCam.m_Orbits[0].m_Radius -= scroll * zoomSpeed;
            //freeLookCam.m_Orbits[0].m_Height += scroll * zoomSpeed;

            freeLookCam.m_Orbits[1].m_Radius -= scroll * zoomSpeed;
            //freeLookCam.m_Orbits[1].m_Height -= scroll * zoomSpeed;

            freeLookCam.m_Orbits[2].m_Radius -= scroll * zoomSpeed;
            //freeLookCam.m_Orbits[2].m_Height -= scroll * zoomSpeed;

            freeLookCam.m_Orbits[0].m_Radius = Mathf.Clamp(freeLookCam.m_Orbits[0].m_Radius, minFov, maxFov);
            //freeLookCam.m_Orbits[0].m_Height = Mathf.Clamp(freeLookCam.m_Orbits[0].m_Radius, minFov, maxFov);

            freeLookCam.m_Orbits[1].m_Radius = Mathf.Clamp(freeLookCam.m_Orbits[1].m_Radius, minFov, maxFov);
            //freeLookCam.m_Orbits[1].m_Height = Mathf.Clamp(freeLookCam.m_Orbits[0].m_Radius, minFov, maxFov);

            freeLookCam.m_Orbits[2].m_Radius = Mathf.Clamp(freeLookCam.m_Orbits[2].m_Radius, minFov, maxFov);
            //freeLookCam.m_Orbits[2].m_Height = Mathf.Clamp(freeLookCam.m_Orbits[0].m_Radius, minFov, maxFov);
        }

    }
}
