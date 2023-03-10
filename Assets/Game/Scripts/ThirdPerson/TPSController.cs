using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using System;
using My.Utils;
using UnityEngine.Rendering;

public class TPSController : MonoBehaviour
{
    public static TPSController I { get; private set;}
    
    [Header("Sensitivity")]
    [Range(0 , 1)] [SerializeField] float m_NormalSensitivity = 1f;
    [Range(0 , 1)] [SerializeField] float m_AimSensitivity = 0.5f;
    [Range(0 , 1)] [SerializeField] float m_ShootPause = 0.2f;

    [Header("Aim Camera")]
    [Range(0 , 1)] [SerializeField] float m_AimInCamBlend =  0.1f;
    [Range(0 , 1)] [SerializeField] float m_AimOutCamBlend = 0.1f;

    [Header("Neck IK")]
    [Range(-1, 0)] [SerializeField] float m_NeckIkConstraint = -0.8f;
    [Range(0, 1)] [SerializeField] float m_NeckIkConstraintMul = 0.5f;
    [Range(0 , 0.5f)] [SerializeField] float m_NeckIkWeightSmoothTime = 0.2f;
    [Range(0 , 1)] [SerializeField] float m_NeckIkWeightSpeedMul = 0.2f;

    [Header("IK Target")]
    [Range(0 , 0.3f)] [SerializeField] float m_IkTargetSmoothTime = 0.1f;
    [Range(0 , 1)] [SerializeField] float m_IkTargetMaxTurnSpeedMul = 0.2f;

    [Space(10)]
    [Range(1, 30)] [SerializeField] float m_IkTargetDist = 2;

    [SerializeField] CinemachineVirtualCamera m_CrouchVCam, m_AimVCam;
    [SerializeField] MeshRenderer m_Pistol;
    [SerializeField] MultiAimConstraint m_NeckRig, m_ChestRig;
    [SerializeField] Volume m_AimVignette, m_AimDOP;
    CinemachineBrain m_cinemachineBlend;
    StarterAssetsInputs m_inputs;
    ThirdPersonController m_tPController;
    Animator m_animator;
    IKTarget m_ikTarget;
    Camera m_mainCam;
    Transform m_tF, m_mainCamTF;
    Firearm m_firearm;
    
    float m_shootPauseThresh = Mathf.Infinity;
    float m_neckIkWeight, m_aimVignetteWeight, m_aimVignetteTime;

    int m_animIDSpeed, m_animIDAim, m_animIDShoot;
    bool m_isAiming = false;

    WaitForSeconds m_coroitineWait = new WaitForSeconds(1/60);

    void Awake()
    {
        I = this;
        AssignComponents();
        AssignAnimationIDs();

        // Starting Position
        PosAtStart posAtStart = FindObjectOfType<PosAtStart>();
        transform.position = posAtStart != null ? posAtStart.transform.position: transform.position;
    }

    void Start()
    {
        /* Caching transforms for faster execution.
        because while calling for transform unity executes some safty code everytime. */
        m_tF = transform;
        m_mainCamTF = m_mainCam.transform;

        StartCoroutine(UpadteIKTaregtPos());
        StartCoroutine(UpadteNeckRigWeight());
        StartCoroutine(UpadteAimVignetteWeight());
    }

    void AssignComponents()
    {
        m_inputs              = GetComponent<StarterAssetsInputs>();
        m_animator            = GetComponent<Animator>();
        m_ikTarget            = FindObjectOfType<IKTarget>();
        m_cinemachineBlend    = FindObjectOfType<CinemachineBrain>();
        m_mainCam             = FindObjectOfType<Camera>();
        m_tPController        = GetComponent<ThirdPersonController>();
        m_firearm             = FindObjectOfType<Firearm>();
    }
    
    void AssignAnimationIDs()
    {
        m_animIDSpeed         = Animator.StringToHash("Speed");
        m_animIDAim           = Animator.StringToHash("Aim");
        m_animIDShoot         = Animator.StringToHash("Shoot");
    }

    void Update()
    {
        SendMouseInput();
        ProcessAim();
        ProcessShoot();
        NeckIkConstraint();

        if (m_tPController.GetIsCrouch)
        {
            m_CrouchVCam.gameObject.SetActive(true);
        }
        else m_CrouchVCam.gameObject.SetActive(false);
    }

    void SendMouseInput()
    {
        m_inputs.LookInput( new Vector2( Input.GetAxis("MouseX"), Input.GetAxis("MouseY") ) );
    }

    void ProcessAim()
    {
        bool isGrounded = m_tPController.GetIsGrounded;
        if ( m_inputs.aim && isGrounded )
        {
            m_isAiming = true;
        }

        else
        {
            m_isAiming = false;
        }

        SetAimParameters();

        // Aim animation Blend
        m_animator.SetBool(m_animIDAim, m_isAiming);
        
        m_shootPauseThresh += Time.deltaTime;
    } 

    void SetAimParameters()
    {
        if (m_isAiming)
        {
            AimParameters(true, 1, m_AimInCamBlend, m_AimSensitivity);
        }
        else
        {
            AimParameters(false, 0, m_AimOutCamBlend, m_NormalSensitivity);
        }
    }

    void AimParameters(bool b, float weight, float aimCamBlend, float sensitivity)
    {
        m_cinemachineBlend.m_DefaultBlend = 
        new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, aimCamBlend);
        m_aimVignetteTime = aimCamBlend;
        m_AimVCam.gameObject.SetActive(b);
        m_Pistol.enabled = b;
        m_AimDOP.enabled = b;
        m_ChestRig.weight = weight;
        m_aimVignetteWeight = weight;
        m_tPController.SetSensitivity(sensitivity);
    }

    void ProcessShoot()
    {
        if (!m_isAiming) return;
        if (Input.GetButtonDown("Fire1") && m_shootPauseThresh > m_ShootPause) // unity new input system in working right
        {
            m_shootPauseThresh = 0;
            // m_firearm.InitiateShoot();
        }
    }
    
    void NeckIkConstraint()
    {
        float targetDirSimilarity = Vector3.Dot(m_tF.forward, m_mainCamTF.forward);
        
        if (targetDirSimilarity < m_NeckIkConstraint)
            m_neckIkWeight = 0;
        if (targetDirSimilarity > m_NeckIkConstraint * m_NeckIkConstraintMul )
            m_neckIkWeight = 1;
    }

    IEnumerator UpadteIKTaregtPos()
    {
        Vector3 currVelocity = default;
        while (true)
        {
            m_ikTarget.transform.position = Vector3.SmoothDamp(
                m_ikTarget.transform.position,
                m_mainCamTF.position + (m_mainCamTF.forward * m_IkTargetDist),
                ref currVelocity, m_IkTargetSmoothTime, 100 * m_IkTargetMaxTurnSpeedMul);
            yield return m_coroitineWait;
        }
    }

    IEnumerator UpadteNeckRigWeight()
    {
        float currVelocity = default;
        while (true)
        {
            // neckRig.weight = Mathf.SmoothStep(neckRig.weight, neckIkWeight, Time.fixedDeltaTime * 20 * neckIkWeightSpeedMul);
            m_NeckRig.weight = Mathf.SmoothDamp(
                m_NeckRig.weight,
                m_neckIkWeight,
                ref currVelocity, 
                m_NeckIkWeightSmoothTime, 
                100 * m_NeckIkWeightSpeedMul
            );
            yield return m_coroitineWait;
        }
    }

    IEnumerator UpadteAimVignetteWeight()
    {
        float currVelocity = default;
        while (true)
        {
            // m_AimPostProcess.enabled = true;
            if (m_AimVignette.weight <= 0.1f) yield return m_coroitineWait;
            m_AimVignette.weight = Mathf.SmoothDamp(m_AimVignette.weight, m_aimVignetteWeight, ref currVelocity, m_aimVignetteTime , 10, Time.deltaTime);
            yield return m_coroitineWait;
            // m_AimPostProcess.enabled = false;
        }
    }

    float EaseIn(float a, float b, float t)
    {
        return a + (b-a) * (t * t);
    }

    public bool GetIsAiming => m_isAiming;
    public void SetIsAiming(bool t) {m_isAiming = t;}
}
