using System.Collections;
using System.Collections.Generic;
using EnemyBehavior;
using FSM;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float playerHPDamage = 40f;
    PlayerHealth playerHealth;
    EnemyAI enemyAI;
    FiniteStateMachine m_fSM;

    int nextUpdate = 1;
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        enemyAI = GetComponent<EnemyAI>();
        m_fSM           = GetComponent<FiniteStateMachine>();
    }

    void Update()
    {
        // print(Mathf.RoundToInt(Time.time));
        RaycastHit hit;
        // RaycastHit hit;
        if(m_fSM.m_isProvoked)
        {
            Physics.Raycast(transform.position, transform.forward, out hit, 13f);
            // Gizmos.DrawWireSphere(transform.position, 13);
            // Gizmos.color = Color.red;
            Debug.DrawLine(transform.position, transform.position + transform.forward * 13, Color.yellow);
            
            if(hit.collider != null && Time.time >= nextUpdate)
            {
                // StartCoroutine(DecreasePlayerHealth());
                AttackHitEvent(); 
                nextUpdate = Mathf.RoundToInt(Time.time) + 1;
            }
        }
    }
    
    IEnumerator DecreasePlayerHealth()
    {
        yield return new WaitForSeconds(1);
        
    }
    public void AttackHitEvent()
    {
        if(playerHealth == null) {return;}
        playerHealth.DecreasePlayerHP(playerHPDamage);
    }
}
