using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponZoom : MonoBehaviour
{
    [SerializeField] Camera fpsCamera;
    [SerializeField] float zoomedIn = 30f;
    [SerializeField] float zoomedOut = 60f;
    FirstPersonController fpsController;
    // bool canZoom = false;


    void Start()
    {
        fpsController = FindObjectOfType<FirstPersonController>();
    }

    void Update()
    {
        
    }
    
    public void SetCanZoom(bool isAiming)
    {
        if(isAiming)
        {
            SetCamerFOV();
            print("Zoomin In");
            // TODO Make it work
            // if(Input.GetAxis("Mouse ScrollWheel") > 0)
            // {
            //     SetCamerFOV();
            //     print("Zooming In!");
            // }
            // if(Input.GetAxis("Mouse ScrollWheel") < 0)
            // {
            //     ResetCamerFOV();
            //     print("Zooming Out!");
            // }
        }
        else
        {
            ResetCamerFOV();
        }
        // if(isAiming)
        // {
        //     // if(Input.GetAxis("Mouse ScrollWheel") > 0)
        //     // {
        //     //     SetCamerFOV();
        //     // }
        // }
        // // && Input.GetMouseButton(1))
        
    }
    private void SetCamerFOV()
    {
        fpsCamera.fieldOfView = zoomedIn;  //Mathf.Lerp(zoomedOut, zoomedIn, 1f);
        // fpsController.m_MouseLook.XSensitivity = 1.5f;
    }
    private void ResetCamerFOV()
    {
        fpsCamera.fieldOfView = zoomedOut;
        // fpsController.m_MouseLook.XSensitivity = 2f;
    }

    
}
