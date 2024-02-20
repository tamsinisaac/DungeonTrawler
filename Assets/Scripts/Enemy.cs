using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public navMeshAgent agent;
    public Transform player;
    public layerMask whatIsGround, whatIsPlayer;

    //Patrolling 
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked; 

    //States 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake () 
    {
     player = GameObject.Find ("PlayerObj").transform;
     agent = GetComponent<NavMeshAgent>();

    }

    





}
