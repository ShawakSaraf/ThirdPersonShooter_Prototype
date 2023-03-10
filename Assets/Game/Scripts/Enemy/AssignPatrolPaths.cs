using System.Collections;
using System.Collections.Generic;
using EnemyBehavior;
using RPG.Control;
using UnityEngine;

public class AssignPatrolPaths : MonoBehaviour
{
    [SerializeField] Transform m_PathPositions;
    public Transform[] m_Enemies;
    public Transform[] m_Paths;
    // [SerializeField] List<Transform> m_Soldiers;
    void Awake()
    {
        m_Enemies   = new Transform[transform.childCount];
        m_Paths     = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            // Add all enemy and paths into respective array
            m_Enemies[i]    = transform.GetChild(i);

            // Cycle throught paths
            m_Paths[i]      = m_PathPositions.GetChild(i % m_PathPositions.childCount);

            // Set enemy's transforms to path strating position
            m_Enemies[i].transform.position = m_Paths[i].GetChild(0).transform.position;

            // Assing requires Variables
            PatrolController patrolController   = m_Enemies[i].GetComponent<PatrolController>();
            EnemyPatrolPath enemyPatrolPath     = m_Paths[i].transform.GetComponent<EnemyPatrolPath>();

            patrolController.patrolPath = enemyPatrolPath;
        }
    }
}
