using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class SFXManager : MonoBehaviour
{
    public AudioSource m_WalkSFX;
    ThirdPersonController m_tPController;
    void Awake()
    {
        m_tPController = ThirdPersonController.I;
    }

    // Update is called once per frame
    void Update()
    {
        // if (m_tPController.m_Moving)
        // {
        //     m_WalkSFX.enabled = true;
        // }
        // else
        // {
        //     m_WalkSFX.enabled = false;
        // }
    }
}
