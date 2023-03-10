using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CompanionAI : MonoBehaviour
{
    public static CompanionAI I { get; private set;}

    NavMeshAgent navAgent;
    [Range(0, 5)][SerializeField] float frontPosDist = 1;
    TPSController partner;
    CinemachineBrain mainCamera;
    
    void Awake()
    {
        I = this;
        navAgent = GetComponent<NavMeshAgent>();
        partner = FindObjectOfType<TPSController>();
        mainCamera = FindObjectOfType<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.SetDestination(
            partner.transform.position
            + ( (partner.transform.forward + partner.transform.right + mainCamera.transform.forward) / 2)
            * frontPosDist
        );
    }
}
