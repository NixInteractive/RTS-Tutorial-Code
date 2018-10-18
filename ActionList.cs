using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script will contain all of the basic actions that can be performed in the game.
//This gets attached to your player object.
public class ActionList : MonoBehaviour {

    //This is the basic movement action.
	public void Move(NavMeshAgent agent, RaycastHit hit)
    {
        agent.destination = hit.point; //Sets the agent's destination to the clicked point
    }

}
