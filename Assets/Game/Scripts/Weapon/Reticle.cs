using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Reticle : MonoBehaviour
{
    public static Reticle I { get; private set;}

    [SerializeField] CanvasRenderer m_reticle, m_AmmoGauge; 

    [Range(0, 1)] [SerializeField] float m_baceReticleUIScaleMul, m_hitRaticleColWait, raticleAlpha;

    float m_AmmoGuageScaleMul, m_ReticleScaleMul = 0.2f;

    Vector3 m_ammoGuageScale = new Vector3(7,7,7);
    Vector3 m_reticleScale = new Vector3(5f,5f,5f);
    StarterAssetsInputs starterAssetsInputs;
    ThirdPersonController thirdPersonController;
    WaitForSeconds m_coroitineLerpWait = new WaitForSeconds(1/60);
    Firearm m_Firearm;
    Color m_RaticleColor, m_AmmoGuageColor = Color.white;

    float t;

    bool IsAming;

    void Awake()
    {
        I = this;
        thirdPersonController   = ThirdPersonController.I;
        starterAssetsInputs     = FindObjectOfType<StarterAssetsInputs>();
        m_Firearm               = FindObjectOfType<Firearm>();
    }
    
    void Start()
    {
        StartCoroutine(LerpGuageScale());

    }
    IEnumerator LerpGuageScale()
    {
        float reticleCurrV = default;
        float ammoGuageCurrV = default;
        while (true)
        {
            m_AmmoGuageScaleMul = Mathf.SmoothDamp(
                m_AmmoGuageScaleMul, m_baceReticleUIScaleMul, ref reticleCurrV, 0.2f
            );
            m_ReticleScaleMul = Mathf.SmoothDamp(
                m_ReticleScaleMul, m_baceReticleUIScaleMul, ref ammoGuageCurrV, 0.2f 
            );
            yield return m_coroitineLerpWait;
        }
    }


    void Update()
    {
        SetGlobalShaderVars();

        m_AmmoGauge.transform.localScale = m_ammoGuageScale * m_AmmoGuageScaleMul;
        m_reticle.transform.localScale = m_reticleScale * m_ReticleScaleMul;
    }

    void SetGlobalShaderVars()
    {
        Shader.SetGlobalFloat("_reticleR", m_RaticleColor.r);
        Shader.SetGlobalFloat("_reticleG", m_RaticleColor.g);
        Shader.SetGlobalFloat("_reticleB", m_RaticleColor.b);
        Shader.SetGlobalFloat("_reticleA", m_RaticleColor.a);

        Shader.SetGlobalFloat("_ammoGuageR", m_AmmoGuageColor.r);
        Shader.SetGlobalFloat("_ammoGuageG", m_AmmoGuageColor.g);
        Shader.SetGlobalFloat("_ammoGuageB", m_AmmoGuageColor.b);
        Shader.SetGlobalFloat("_ammoGuageA", m_AmmoGuageColor.a);
    }

    public void SetRaticleHitColor()
    {
        m_RaticleColor = Color.red;
        Invoke("SetReticlColor", m_hitRaticleColWait);
    }

    public void SetReticlAlpha()
    {
        m_RaticleColor = new Vector4(m_RaticleColor.r, m_RaticleColor.g, m_RaticleColor.b, raticleAlpha);
        m_AmmoGuageColor = new Vector4(m_AmmoGuageColor.r, m_AmmoGuageColor.g, m_AmmoGuageColor.b, raticleAlpha);
        m_reticle.SetAlpha(raticleAlpha);
    }
    public void SetReticlColor()
    {
        m_RaticleColor = Color.white;
        m_AmmoGuageColor = Color.white;
    }

    public void ReticlePop() { m_ReticleScaleMul = 0.5f; }
    public void AmmoGuagePop() { m_AmmoGuageScaleMul = 0.3f;}
}
