using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set;}

    TPSController m_tPSController;
    Reticle m_reticleUI;
    HealthBar m_healthUI;
    WaitForSeconds m_coroitineLerpWait = new WaitForSeconds(1/60);

    void Awake()
    {
        I = this;
        m_reticleUI     = GetComponentInChildren<Reticle>();
        m_healthUI      = GetComponentInChildren<HealthBar>();
        m_tPSController = FindObjectOfType<TPSController>();
    }

    void Update()
    {
       if(!m_tPSController.GetIsAiming)
        {
            AimParameters(false);
        }
        else { AimParameters(true); }
    }

    void AimParameters(bool b)
    {
        for (int i = 0; i < m_reticleUI.transform.childCount; i++)
        {
            m_reticleUI.transform.GetChild(i).gameObject.SetActive(b);
        }
        m_healthUI.transform.GetChild(0).gameObject.SetActive(b);
    }
}
