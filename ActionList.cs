using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script is attached to the Player object and will eventually have most of the basic functions required by most units such as movement and basic attacks.
public class ActionList : MonoBehaviour {

    //This method takes a NavMeshAgent and a RaycastHit and tells the agent to move to the hit location.
	public void Move(NavMeshAgent agent, RaycastHit hit)
    {
        agent.destination = hit.point;
    }

}
