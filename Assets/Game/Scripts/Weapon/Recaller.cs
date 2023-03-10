using System.Collections;
using System.Collections.Generic;
using FSM;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using GD.MinMaxSlider;

public class Recaller : MonoBehaviour
{
    const float PI = Mathf.PI;
    const float TAU = Mathff.TAU;
    [SerializeField] Transform m_Origin, m_TrailObj, m_PlayerChestTF;
    [SerializeField] AnimationCurve m_recallCurve;
    [Range(0, 1)] [SerializeField]  float m_Damage;
    [Range(0, 10)] [SerializeField]  float m_RecallSpeedMul;

    [MinMaxSlider(0, 100)]
    [SerializeField] Vector2 m_DistThreshold;

    [Tooltip("in Radians")]
    [Range(0, 1)] [SerializeField] float m_InitialAngleMul = 0.125f;
    [Range(0, 1)] [SerializeField] float m_InitialVelocityMul, m_HeightMul, m_RacallCurve, m_CamShakeAmp, m_CamShakeFreq, m_CamShakeT;

    Vector3 m_posAtCall, m_impulse;
    Transform m_recallTF, i_TrailObj;
    Collider m_recallCollider, m_trailCollider;
    Rigidbody m_recallRB, m_trailRB;

    Recallable m_recallable;
    TrailRenderer m_trailRend;
    TPSController m_tPSController;
    IKTarget m_iKTarget;
    CustomCinemachineCam m_customCineCam;

    WaitForSeconds m_RecallUpdateCycles = new WaitForSeconds(1);
    WaitForSeconds m_trailImpulseWait = new WaitForSeconds(.1f);

    public float m_time, m_maxRange, m_maxHeight, m_distanceToffset;
    bool EmitTrail, m_isRecalling = false;
    bool m_isPointLight = false;

    Vector3 velocity;

    void Awake()
    {
        m_tPSController     = GetComponentInParent<TPSController>();
        m_recallable        = FindObjectOfType<Recallable>();
        m_recallRB          = m_recallable.GetComponent<Rigidbody>();
        m_recallCollider    = m_recallable.GetComponent<SphereCollider>();
        m_recallTF          = m_recallable.transform;
        m_trailRB           = m_TrailObj.GetComponent<Rigidbody>();
        m_trailCollider     = m_TrailObj.GetComponent<Collider>();
        m_trailRend         = m_TrailObj.GetComponent<TrailRenderer>();
        m_iKTarget          = FindObjectOfType<IKTarget>();
        m_customCineCam     = FindObjectOfType<CustomCinemachineCam>();
    }

    void Start()
    {
        StartCoroutine(UpdateRecallable());
    }

    IEnumerator UpdateRecallable()
    {
        while(true)
        {
            EmitTrail = true;
            yield return m_RecallUpdateCycles;
        }
        
    }

    void Update()
    {
        Shader.SetGlobalFloat("_Tention", m_RacallCurve);
        CalculateVars();

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_time = 0;
            m_posAtCall = m_recallTF.localPosition;
        }


        if ( m_tPSController.GetIsAiming )
        {
            m_isRecalling = Input.GetKey(KeyCode.Mouse0);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                m_isPointLight = false;
                float sqrMag = (m_recallTF.localPosition - m_Origin.position).sqrMagnitude;
                m_distanceToffset = m_DistThreshold.x / Mathf.Max( 10, Mathf.Min( sqrMag, m_DistThreshold.y ) );
                m_posAtCall = m_recallTF.localPosition;
                m_time = 0;
            }
            if (m_isRecalling)
            {
                m_RacallCurve = m_recallCurve.Evaluate( (m_time * m_RecallSpeedMul) * m_distanceToffset);
                TurnPhysicsOff(true);
                Recall();
            }
            else
            {
                TurnPhysicsOff(false);
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                // m_customCineCam.CamShake(m_CamShakeAmp, m_CamShakeFreq, m_CamShakeT);
                CustomCinemachineCam.I.CamShake(m_CamShakeAmp, m_CamShakeFreq, m_CamShakeT);
                m_recallCollider.isTrigger = false;
                m_recallRB.AddRelativeForce(m_impulse * m_RacallCurve);
                m_RacallCurve = 0;
            }
            // Trail();
        }
        else
        {
            m_isRecalling = false;
            m_recallCollider.isTrigger = false;
            m_recallRB.isKinematic = false;
            m_RacallCurve = 0;
        }
        
        if (m_isPointLight)
        {
            TurnPhysicsOff(true);
            Vector3 offset = (m_PlayerChestTF.forward * m_recallable.GetPointLightOffset.z) + 
                ( m_PlayerChestTF.up    * m_recallable.GetPointLightOffset.y ) + 
                ( m_PlayerChestTF.right * m_recallable.GetPointLightOffset.x );

            Vector3 targetPos = m_PlayerChestTF.position + offset;
            m_recallTF.localPosition = Vector3.Lerp(m_recallTF.localPosition, targetPos, Time.fixedDeltaTime*2);
        }
    }

    void CalculateVars()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay( screenCenterPoint );

        bool rayHit = Physics.SphereCast(ray, m_recallTF.localScale.x, out RaycastHit hit, 999f);
        m_Origin.rotation = Quaternion.LookRotation(hit.point - m_Origin.position);

        float i_Velocity_sqr = ((100 * 100) * m_InitialVelocityMul) * m_RacallCurve;
        float sinAngDeg = Mathf.Sin(m_InitialAngleMul * PI);

        m_maxRange  = Mathf.Abs(i_Velocity_sqr * Mathf.Sin((m_InitialAngleMul * PI) * 2));
        m_maxHeight = Mathf.Abs( (m_Origin.position.y + i_Velocity_sqr) * sinAngDeg * sinAngDeg ) *  m_HeightMul * 0.1f;
        m_impulse   = new Vector3(0, m_maxHeight, 5000 * m_InitialVelocityMul);
        m_time      += Time.fixedDeltaTime;

        if (Input.GetKeyDown(KeyCode.Tab) && !m_tPSController.GetIsAiming )
        {
            m_posAtCall = m_recallTF.localPosition;
            m_isPointLight = m_isPointLight ? false : true;
        }
    }

    void Recall()
    {
        m_recallTF.localPosition = Vector3.Lerp(
            m_posAtCall,
            m_Origin.position + (m_Origin.forward * (-m_RacallCurve + 1)),
            m_RacallCurve
        );

        // Point Constraint
        // Vector3 C_pt = m_recallTF.localPosition - (m_Origin.position + (m_Origin.forward * (-m_RacallCurve + 1)));
        // float beta = 0.1f;
        // velocity += -(beta/Time.deltaTime) * C_pt;
        // velocity *= .7f;
        // m_recallTF.localPosition += velocity * Time.deltaTime;


        m_recallTF.rotation = m_Origin.rotation;
    }

    void Trail()
    {
        if ( Input.GetKeyDown(KeyCode.Q) ) EmitTrail = true;
        if ( !Input.GetKey(KeyCode.Q) ) return;
        if ( EmitTrail )
        {
            // i_TrailObj = Instantiate(m_TrailObj, m_Origin.position, m_Origin.rotation);

            m_trailRend.Clear();
            TurnPhysicsOff(true);
            m_TrailObj.transform.localPosition = m_Origin.position;
            m_TrailObj.transform.rotation = m_Origin.rotation;
            m_trailRend.enabled = false;
            StartCoroutine( ApplyImpulse() );
            EmitTrail = false;
            // Destroy(i_TrailObj.gameObject, 1);
        }
    }

    IEnumerator ApplyImpulse()
    {
        yield return m_trailImpulseWait;
        TurnPhysicsOff(false);
        m_trailRend.enabled = true;
        Vector3 impulse = m_isRecalling ? m_impulse * m_RacallCurve : m_impulse;
        m_trailRB.AddRelativeForce(impulse);
    }

    void TurnPhysicsOff(bool b)
    {
        m_recallRB.isKinematic = b;
        m_recallCollider.isTrigger = b;
        m_trailCollider.isTrigger = b;
        m_trailRB.isKinematic = b;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {/* 
        Vector3 pos = m_Ball.transform.localPosition;
        Handles.DrawAAPolyLine(m_Origin.position, pos);
        Vector3 vector3 = new Vector3(pos.x, m_Origin.position.y, pos.z);
        Handles.DrawAAPolyLine(m_Origin.position, vector3);
        Handles.DrawAAPolyLine(pos, vector3);

        Handles.color = Color.blue;
        // Handles.DrawAAPolyLine(m_PistolTip.position, m_PistolTip.position + new Vector3(0, m_maxHeight, m_maxRange).normalized);

        for (int i = 0; i < 50; i++)
        {
            float t = i/50;
        }
     */
        if(m_recallable != null && m_tPSController.GetIsAiming)
        {
            Physics.SphereCast(m_Origin.position+m_Origin.forward, m_recallTF.localScale.x, m_Origin.forward, out RaycastHit hit0, 100, 3);

            if (hit0.collider != null)
            {
                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(m_Origin.position+m_Origin.forward, hit0.point);
                Gizmos.DrawWireSphere(hit0.point, m_recallTF.localScale.x);

                Vector3 reflected = Vector3.Reflect((hit0.point- m_Origin.position).normalized, hit0.normal);
                Physics.SphereCast(hit0.point, m_recallTF.localScale.x, reflected, out RaycastHit hit1, 100, 3);

                Handles.color = Color.yellow;
                Handles.DrawAAPolyLine(hit0.point, hit1.point);
                Vector3 reflected2 = Vector3.Reflect((hit1.point - hit0.point), hit1.normal);
                if (hit1.collider != null)
                    Handles.color = Color.cyan;
                    Handles.DrawAAPolyLine(hit1.point, reflected2 + hit1.point);
            }
        }
    }
#endif
}
