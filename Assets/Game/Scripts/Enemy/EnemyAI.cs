using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FSM;
using Random = UnityEngine.Random;
using UnityEngine.Animations.Rigging;
using RPG.Control;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace EnemyBehavior
{
    public class EnemyAI : MonoBehaviour
    {
        [Range(0, 50)][SerializeField] float m_BoundingSphereRadius = 20;
        [Range(0, 1)][SerializeField] float m_SubSphereRadiusMul = 0.2f;

        [Header("AI MOVEMENT")]

        [Tooltip("Default Low Crawl Animation speed = 0.5662793")]
        [Range(0,3)] [SerializeField] float m_WalkSpeed = 0.5662793f;

        [Tooltip("Default Running Crawl Animation speed = 2.223197")]
        [Range(0,10)] [SerializeField] float m_SprintSpeed = 2.223197f;

        [Range(0,50)] [SerializeField] float maxChaseRange = 10f;
        [Range(0,90)] [SerializeField] float visionCone = 10f;
        [Range(0,20)] [SerializeField] float turnSpeed = 1f;
        [Range(0, 5)] [SerializeField] float chaseTargetPause = 0.5f;
        [Range(0, 5)][SerializeField] float m_AlertStateTime = 3f;

        [Header("EYE VARS")]
        [Range(0, 0.2f)][SerializeField] float m_EyeTurnTime = 0.1f;
        [Range(0, 1)][SerializeField] float m_LightMaxOutAngMul = 0.7f;
        [Range(0, 1)][SerializeField] float m_LightMinOutAngMul = 0.2f;
        [Range(0, 10)][SerializeField] float lightFocusSpeed = 3f;
        [Range(0, 30)][SerializeField] float m_EyeTwitchSpeed = 10f;
        float m_headRigWeightShifter = 0;
        
        [Header("ADD RANDOMNESS")]
        [SerializeField] Vector3 m_MaxRandomNoise = new Vector3(1f, 0f, 1f);
        [SerializeField] Vector3 m_MinRandomNoise = new Vector3(-1f, 0f, -1f);

        [Header("Refrenced Objects")]
        [SerializeField] Transform m_EyeMask;
        [SerializeField] Light m_SpotLight;
        [SerializeField] MultiAimConstraint m_HeadRig;
        WeightedTransformArray m_headRigData;
        Rigidbody[] rigidbodies;
        
        TPSController m_target;
        EnemyHealth m_enemyHealth;
        NavMeshAgent m_navAgent;
        PlayerHealth m_playerHealth;
        Animator m_animator;
        FiniteStateMachine m_fSM;
        PatrolController m_patrolControl;
        Color m_targetLineCol = Color.green;
        
        WaitForSeconds m_coroitineLerpWait = new WaitForSeconds(1/60);

        float m_outerAngLerpMul, m_currTurnVelocity, m_deltaTime;
        float targetDist = Mathf.Infinity;

        int m_animIDCrawlSpeed;

        // public bool m_fSM.isProvoked {get; private set;} = false;
        Vector3 m_randomPtRel, m_targetPos, m_tfPos;

        [Header("Debug")]
        public bool executeProvokeBehevior = true;

        void Awake()
        {
            AssignComponents();
            m_animIDCrawlSpeed = Animator.StringToHash("CrawlSpeed");
            m_headRigData = m_HeadRig.data.sourceObjects;
            rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        }

        void AssignComponents()
        {
            m_target        = FindObjectOfType<TPSController>();
            m_navAgent      = GetComponent<NavMeshAgent>();
            m_animator      = GetComponent<Animator>();
            m_playerHealth  = FindObjectOfType<PlayerHealth>();
            m_fSM           = GetComponent<FiniteStateMachine>();
            m_enemyHealth   = GetComponent<EnemyHealth>();
            m_patrolControl = GetComponent<PatrolController>();
        }

        void Start()
        {
            m_outerAngLerpMul = m_LightMinOutAngMul;
            StartCoroutine( EmitSound() );
            StartCoroutine( LerpLightShape() );
        }

        IEnumerator EmitSound()
        {
            while (true)
            {
                m_fSM.m_emitSoundAtRandomPos = true;
                yield return new WaitForSeconds( Random.Range(0.1f, 3f) );
            }
        }

        IEnumerator LerpLightShape()
        {
            float lightPopCurrV = default;
            float baceLightIntencity = m_SpotLight.intensity;
            while (true)
            {
                m_SpotLight.spotAngle = Mathf.Lerp(
                    m_SpotLight.spotAngle,
                    179 * m_outerAngLerpMul,
                    m_deltaTime * lightFocusSpeed);

                m_SpotLight.intensity = Mathf.SmoothDamp(m_SpotLight.intensity, baceLightIntencity, ref lightPopCurrV, 0.1f);
                yield return m_coroitineLerpWait;
            }
        }

        void Update()
        {
            if ( !executeProvokeBehevior ) return;
            if ( !m_fSM.m_isDead )
            {
                CacheVars();
                GenerateRandomPt();
            
                m_animator.SetFloat(m_animIDCrawlSpeed, /* m_navAgent.speed */ GetLocalVelocity().z);
                m_HeadRig.data.sourceObjects = m_headRigData;

                IdleBehavior();
                AlertBehavior();
                InspectSoundLocation();
                ProvokedBehavior();
                m_headRigData.SetWeight(0, -m_headRigWeightShifter + 1);
                m_headRigData.SetWeight(1, m_headRigWeightShifter);
            }
            else
            {
                m_SpotLight.enabled = false;
                m_navAgent.enabled = false;
                m_animator.enabled = false;
            }

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                // To account for collisions
                rigidbodies[i].isKinematic = !m_fSM.m_isDead;
            }
        }

        void CacheVars()
        {
            m_deltaTime     = Time.deltaTime;
            m_targetPos     = m_target.transform.localPosition; // local position is more optimized
            m_tfPos         = transform.position;
        }
        
        void IdleBehavior()
        {
            if (m_fSM.m_isIdle)
            {
                // m_EyeMask.rotation = PerlinNoiseRotY(transform.rotation);
                m_navAgent.speed = m_WalkSpeed;
                m_navAgent.SetDestination(m_patrolControl.nextWaypoint/*  + PerlinNoiseRotY() */);
            }
        }

        void AlertBehavior()
        {
            if (m_fSM.m_isAlerted)
            {
                // if(!m_fSM.isProvoked)
                    // m_EyeParent.LookAt(m_randomPtRel);

                Vector3 soundDir    = (m_randomPtRel - m_EyeMask.position).normalized;
                float targetAngYaw  = Mathf.Atan2(soundDir.x, soundDir.z) * Mathf.Rad2Deg;
                float angleYaw      = Mathf.SmoothDampAngle(
                    m_EyeMask.eulerAngles.y,
                    targetAngYaw,
                    ref m_currTurnVelocity,
                    m_EyeTurnTime
                );

                float targetAngPitch = Mathf.Atan2(soundDir.z, soundDir.y) * Mathf.Rad2Deg;
                float anglePitch = Mathf.SmoothDampAngle(m_EyeMask.eulerAngles.x, targetAngPitch, ref m_currTurnVelocity,m_EyeTurnTime);

                m_outerAngLerpMul    = m_LightMinOutAngMul;
            }
            else
            {
                // m_EyeParent.LookAt(m_randomPtRel);
                m_outerAngLerpMul = m_LightMaxOutAngMul;
            }
        }

        void InspectSoundLocation()
        {
            if (m_fSM.m_isInspecting && !m_fSM.m_isProvoked)
            {
                // print("Initiating Inspect State!");
                Vector3 target = Vector3.ProjectOnPlane(m_randomPtRel, Vector3.up);
                Vector3 targetRel = target - m_tfPos;
                
                m_navAgent.SetDestination(target);
                // print(navAgent.velocity.magnitude);
                if ( m_navAgent.velocity.magnitude <= 0 )
                {
                    m_fSM.Set_isAlerted(false);
                }
                // After Behavior logic execution Change States.
            }
        }

        void ProvokedBehavior()
        {
            Vector3 targetVec = m_targetPos - m_tfPos;

            Vector3 angleFrom = m_fSM.m_isAlerted ? m_EyeMask.forward : transform.forward; // to prevent FOV from twitching during Idle state
            float angle = Vector3.Angle(targetVec, angleFrom);
            // Debug.DrawLine(tfPos, new Vector3(-10, 0, 100) , Color.magenta);

            targetDist = targetVec.magnitude;
            if (m_fSM.m_isProvoked && !m_fSM.m_isDead)
            {
                m_fSM.Set_isIdle(false);
                EngageTarget();
                LookAtTarget();
                m_playerHealth.EnableHealthBar();
                // GetComponent<EnemyLight>().SetLightFlicker(m_fSM.isProvoked);

                m_headRigWeightShifter = 1;
                m_HeadRig.weight = 1;
            }
            else if (targetDist <= maxChaseRange && angle < visionCone)
            {
                m_fSM.Set_isProvoked(true);
            }
            else
            {
                m_headRigWeightShifter = 0;
            }
        }

        void EngageTarget()
        {
            if (targetDist >= m_navAgent.stoppingDistance && !m_fSM.m_isDead)
            {
                StartCoroutine(ChaseTarget());
            }

            if (targetDist <= m_navAgent.stoppingDistance)
            {
                // AttackTarget();
            }
        }

        void LookAtTarget()
        {
            Vector3 lookDirection   = (m_targetPos - m_tfPos).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z)); 
            transform.rotation = 
            Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                turnSpeed * m_deltaTime
            );
        }

        IEnumerator ChaseTarget()
        {
            m_navAgent.speed =  0;
            yield return new WaitForSeconds( Random.Range(chaseTargetPause * 0.5f, chaseTargetPause) );
            m_navAgent.speed =  m_SprintSpeed;
            // m_animator.SetBool("Attack", false);
            
            Vector3 randomPos = new Vector3(
                Random.Range(m_MinRandomNoise.x, m_MaxRandomNoise.x),
                Random.Range(m_MinRandomNoise.y, m_MaxRandomNoise.y),
                Random.Range(m_MinRandomNoise.z, m_MaxRandomNoise.z)
            );

            if (!m_fSM.m_isDead)
            {
                m_navAgent.SetDestination(/* randomPos +=  */m_targetPos);
            }
            
            // m_animator.SetTrigger("Move");

        }

        void AttackTarget()
        {
            // m_animator.SetBool("Attack", true);
        }

        public void ChaseTargetIfShot(Collider collider)  // need to find a bettr way, this'll only work with ray cast
        {
            if (collider)
            {
                m_fSM.Set_isProvoked(true);
                chaseTargetPause *= 0.5f;
            }
        }

        void GenerateRandomPt()
        {
            if (m_fSM.m_emitSoundAtRandomPos && !m_fSM.m_isProvoked)
            {
                m_fSM.m_isSoundEmitted = true;
                m_fSM.m_alertTimer = m_AlertStateTime;
                m_fSM.m_isSoundFromOldSpot = m_fSM.m_isAlerted ? true : false;

                // What Are these Fucking Names Lad!
                Vector3 oldRandomPT         = m_randomPtRel;
                Vector3 relToBound          = m_patrolControl.patrolPath.transform.position + (Random.insideUnitSphere * m_BoundingSphereRadius);
                Vector3 relToOldRandomPT    = oldRandomPT + (Random.insideUnitSphere * (m_BoundingSphereRadius * m_SubSphereRadiusMul));

                // Vector3 newRandomPt = fSM.m_isAlerted ? relToOldRandomPT : relToTF;
                Vector3 newRandomPt = relToBound;

                RaycastHit hit = default;
                Vector3 targetRel = newRandomPt - m_tfPos;
                
                Physics.Raycast( m_tfPos, targetRel, out hit, targetRel.magnitude );

                m_randomPtRel = hit.collider ? hit.point : newRandomPt; 

                m_fSM.m_emitSoundAtRandomPos = false;
                // isSoundEmitted = false;
            }
        }

        public void SpotLightPop(float damage)
        {
            m_SpotLight.intensity *= damage;
        }

        Quaternion PerlinNoiseRotY(Quaternion targetRot)
        {
            return Quaternion.Euler( 0, ( Mathf.PerlinNoise( 0, 1 + Time.timeSinceLevelLoad * m_EyeTwitchSpeed ) - 0.5f ) * Mathf.Rad2Deg, 0 ) * targetRot;
        }
        Quaternion PerlinNoiseRotZ(Quaternion targetRot)
        {
            return Quaternion.Euler( 0, 0, ( Mathf.PerlinNoise( 0, 1 + Time.timeSinceLevelLoad * m_EyeTwitchSpeed ) - 0.5f ) * Mathf.Rad2Deg ) * targetRot;
        }

        private Vector3 GetLocalVelocity() => transform.InverseTransformDirection(m_navAgent.velocity);


    #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 pos = m_tfPos;
            Vector3 dir = GetComponent<FiniteStateMachine>().m_isAlerted ? m_EyeMask.forward : transform.forward;
            Vector3 targetDir = m_target == null ? default : m_target.transform.position - pos;

            // Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = m_targetLineCol;

            Handles.DrawWireArc(pos, Vector3.up, dir,  visionCone, maxChaseRange);
            Handles.DrawWireArc(pos, Vector3.up, dir, -visionCone, maxChaseRange);

            // Handles.DrawWireDisc(pos, transform.up, maxChaseRange);

            if (Vector3.Angle(targetDir, dir) < visionCone)
            {
                m_targetLineCol = Color.cyan;
                Handles.DrawAAPolyLine(pos, pos + targetDir);
                if (targetDir.magnitude < maxChaseRange) 
                    m_targetLineCol = Color.red;
            }
            else{m_targetLineCol = Color.gray;}
        }
        void OnDrawGizmosSelected() 
        {
            // Vector3 randomPt = transform.position + (Random.insideUnitSphere * radius);
            Gizmos.DrawSphere(m_randomPtRel, 0.3f);
            Gizmos.DrawLine(transform.position, m_randomPtRel);

            if (m_patrolControl == null) return;
            // Main Sphere
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(m_patrolControl.patrolPath.transform.position, m_BoundingSphereRadius);

            // Sub Sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_randomPtRel, m_BoundingSphereRadius * m_SubSphereRadiusMul);
        }
    #endif
    }
}