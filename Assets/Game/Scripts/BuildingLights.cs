using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLights : MonoBehaviour
{
    [SerializeField] Light[] m_Lights;
    [SerializeField] AnimationCurve m_TestCurv;
    [Range(0,40000)][SerializeField] float m_LightIntencity = 40000;
    [Range(0,2)][SerializeField] float m_Speed = 10;

    WaitForSeconds waitForSeconds = new WaitForSeconds(1/30);
    void Start()
    {
        StartCoroutine(LightMorseCode());
    }

    IEnumerator LightMorseCode()
    {
        while (true)
        {
            for (int i = 0; i < m_Lights.Length; i++)
            {
                m_Lights[i].intensity = m_TestCurv.Evaluate(Time.timeSinceLevelLoad * m_Speed) * m_LightIntencity;
            }
            yield return waitForSeconds;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
