using System.Collections;
using System.Collections.Generic;
using EnemyBehavior;
using FSM;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float enemyHealth = 10;
    [SerializeField] Transform m_DebugHPBar;
    float m_maxHP;
    Animator m_animator;
    FiniteStateMachine m_fSM;
    Reticle m_reticle;
    EnemyAI m_enemyAI;

    void Start()
    {
        m_animator  = GetComponent<Animator>();
        m_fSM       = GetComponent<FiniteStateMachine>();
        m_reticle   = FindObjectOfType<Reticle>();
        m_enemyAI   = GetComponent<EnemyAI>();
        m_maxHP     = enemyHealth;
    }

    void Update()
    {
        float hpPercent = enemyHealth/m_maxHP;
        m_DebugHPBar.GetComponentInChildren<Renderer>().material.SetFloat("_EnemyHP", hpPercent);

        if(m_fSM.m_isDead)
        {
            Invoke("Explosion", 0.2f);
        }
    }

    void Explosion()
    {
        // print("Adding Explotion Force!");
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 1, 6);
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            nearbyColliders[i].GetComponent<Rigidbody>().AddExplosionForce(
                5000, Vector3.ProjectOnPlane(transform.position, Vector3.up), 1 );
        }
    }

    public void TakeDamage(float damage, Collider collider)
    {
        // float applyDamage = hit.transform.name == "Head Collider" ? damage * 2 : damage; // OPTIMIZE
        float applyDamage = 0;
        m_enemyAI.ChaseTargetIfShot(collider);
        m_enemyAI.SpotLightPop(applyDamage);

        if (collider.name == "mixamorig:Head")
        {
            applyDamage = damage * 2;
            m_reticle.ReticlePop();
            m_reticle.SetRaticleHitColor();
        }
        else
        {
            applyDamage = damage;
        }

        enemyHealth -= enemyHealth > 0 ? applyDamage : 0;
        enemyHealth = Mathf.Max(0, enemyHealth);
        // print(transform.name + ": Taking Damage " + enemyHealth);

        if (enemyHealth <= 0f)
        {
            m_fSM.Set_isDead(true);
            m_animator.SetBool("Dead", true);
            
            // Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 1, 6);
            // for (int i = 0; i < nearbyColliders.Length; i++)
            // {
            //     nearbyColliders[i].GetComponent<Rigidbody>().AddExplosionForce(
            //         100, Vector3.ProjectOnPlane(transform.position, Vector3.up), 1);
            // }
        }
        else
        {
            m_fSM.Set_isDead(false);
        }
    }

    public float GetEnemyHealth() {return enemyHealth;}
}
