using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using EnemyBehavior;
using FSM;

namespace RPG.Control
{
    public class PatrolController : MonoBehaviour
    {
        public static PatrolController I { get; private set;}
        // [Range(1, 20)][SerializeField] float m_PatrolTwitchSpeed = 10f;
        [SerializeField] float attackRange = 1f;
        [SerializeField] float smoothTime = 0.3f;
        [SerializeField] float waypointTolerance = 3f;
        [SerializeField] float t_WaypointDwell = 2f;
        [SerializeField] int currentWaypointIndex = 0;
        
        float t_SinceAttack = 0f;
        float currentSmoothVelocity;
        float t_SinceLastSawPlayer = Mathf.Infinity;
        float t_SinceWaypointStop = Mathf.Infinity;
        float m_deltaTime;

        // GameObject target;
        TPSController target;
        EnemyHealth EnemyHP;
        FiniteStateMachine m_fSM;
        EnemyAI m_enemyAI;
        NavMeshAgent navAgent;
        Vector3 initialPos;
        public Vector3 nextWaypoint;
        [HideInInspector] public EnemyPatrolPath patrolPath;

        void Awake()
        {
            I = this;
            m_enemyAI   = GetComponent<EnemyAI>();
            navAgent    = GetComponent<NavMeshAgent>();
            m_fSM       = GetComponent<FiniteStateMachine>();
            // target      = GameObject.FindGameObjectWithTag("Player");
            target      = FindObjectOfType<TPSController>();
            EnemyHP     = GetComponent<EnemyHealth>();
        }

        void Start()
        {
            initialPos = transform.position;
        }

        void Update()
        {
            if(m_fSM.m_isDead) return;
            m_deltaTime = Time.deltaTime;
            // navAgent.enabled = !EnemyHP.m_isDead;

            t_SinceAttack += m_deltaTime;

            if (m_fSM.m_isIdle)
            {
                LoopPatrolPath();
            }

            t_SinceLastSawPlayer += m_deltaTime;
            t_SinceWaypointStop += m_deltaTime;
            
        }

        void LoopPatrolPath()
        {
            nextWaypoint = initialPos;
            if (patrolPath != null && GetTargetDist() > attackRange)
            {
                if(AtWaypoint() && t_SinceWaypointStop > t_WaypointDwell)
                {
                    t_SinceWaypointStop = 0f;
                    CycleWaypoint();
                }
                nextWaypoint = GetCurrectWaypoint();
            }

            Vector3 rayDir = (nextWaypoint - transform.position);
            var rayToPT = Physics.Raycast(transform.position, rayDir.normalized, out RaycastHit hit, rayDir.magnitude);
            Vector3 nextExplorePT = rayToPT ? hit.point : nextWaypoint;
        }

        private bool AtWaypoint() => Vector3.Distance(transform.position, GetCurrectWaypoint()) < waypointTolerance;

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
        private Vector3 GetCurrectWaypoint() => patrolPath.GetWaypoint(currentWaypointIndex);


        private float GetTargetAngle() => Mathf.Atan2(target.transform.position.x, target.transform.position.z) * Mathf.Rad2Deg;

        private float GetAngle() => Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            GetTargetAngle(),
            ref currentSmoothVelocity,
            smoothTime
        );
        private Vector3 GetLocalVelocity() => transform.InverseTransformDirection(navAgent.velocity);

        private float GetTargetDist() => Vector3.Distance(transform.position, target.transform.position);
        private float GetTargetDot() => Vector3.Dot(transform.forward, target.transform.forward);

        /* void OnDrawGizmosSelected()
        {
            TPShooterController target = FindObjectOfType<TPShooterController>();
            if (Vector3.Distance(transform.position, target.transform.position) < chaseRange)
            Gizmos.color = Color.red;
            else
            Gizmos.color = Color.cyan;
    
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }

        void OnDrawGizmos()
        {
            TPShooterController target = FindObjectOfType<TPShooterController>();
            if (Vector3.Distance(transform.position, target.transform.position) < chaseRange)
            Gizmos.color = Color.red;
            else
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);
        } */
    }
}
