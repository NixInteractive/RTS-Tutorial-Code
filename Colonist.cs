using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script contains information specific to the colonist unit. It gets attached to the Colonist Object.
public class Colonist : MonoBehaviour {

    public TaskList task; //Retrieves the TaskList

    public ResourceManager RM; //Retrieves the ResourceManager

    private ActionList AL; //Retrieves the ActionList

    public GameObject targetNode; //The resource node the colonist is currently harvesting

    public NodeManager.ResourceTypes heldResourceType; //The type of resource the colonist is holding.

    public bool isGathering = false; //Is the Colonist currently harvesting resources?
    public bool isGatherer = false; //Has the colonist been counted as a gatherer?

    private NavMeshAgent agent; //The NavMeshAgent attached to the Colonist

    public int heldResource; //The amount off resources the colonist is holding
    public int maxHeldResource; //The maximum amount the colonist can carry

    public GameObject[] drops; //An array of possible drop off locations

    public float distToTarget; //Distance to the target point

    // Use this for initialization
    void Start () {

        StartCoroutine(GatherTick()); //Starts the incremental gathering 

        agent = GetComponent<NavMeshAgent>(); //Retrieves the NavMeshAgent Component

        AL = FindObjectOfType<ActionList>(); //Finds the object with the ActionList script and pulls the information
    }
	
	// Update is called once per frame
	void Update () {

        if(transform.position == agent.destination && task == TaskList.Moving) //Is the colonist done moving?
        {
            task = TaskList.Idle; //Set the colonist to be idle
        }

        if (task == TaskList.Gathering) //Is the colonist currently gathering?
        {
            distToTarget = Vector3.Distance(targetNode.transform.position, transform.position); //Gets the distance to the target node

            if (distToTarget <= 3.5f) //Is the distance to the target less than 3.5?
            {
                Gather(); //Calls the Gather() Method
            }
        }

        if (task == TaskList.Delivering) //Is the colonist currently delivering?
        {
            if (distToTarget <= 3.5f) //Is the distance to the target drop less than 3.5?
            {
                if(RM.ice >= RM.maxIce) //Is the stored amount of Ice greater than or equal to the max amount of ice?
                {
                    task = TaskList.Idle; //Set the colonist to be idle
                    isGatherer = false; //Set the colonist to not be a gatherer
                }
                else if(RM.ice + heldResource >= RM.maxIce) //Is the stored amount of ice going to exceed the max when the colonist delivers?
                {
                    int resourceOverflow = (int)RM.maxIce - (int)RM.ice; //How much ice can be stored before hitting capacity

                    heldResource -= resourceOverflow; //Remove the ice that can be stored from the colonist
                    RM.ice = RM.maxIce; //Set the stored ice to equal the maximum
                    task = TaskList.Gathering; //Set the colonist to go back to gathering
                    agent.destination = targetNode.transform.position; //Set the colonist's destination
                    isGatherer = false; //Set the colonist to not be a gatherer
                }
                else
                {
                    RM.ice += heldResource; //Add the colonist's ice to the stored ice
                    heldResource = 0; //Empty the colonist's ice storage
                    task = TaskList.Gathering; //Set the colonist to go back to gathering
                    agent.destination = targetNode.transform.position; //Set the colonist's destination
                    isGatherer = false; //Set the colonist to not be a gatherer
                }
            }
        }

        if (targetNode == null) //Does the node the colonist was gathering from still exist?
        {

            if (heldResource != 0) //Is the colonist still holding resources?
            {
                drops = GameObject.FindGameObjectsWithTag("Drops"); //Fill the array of drops with all possible drop locations
                agent.destination = GetClosestDropOff(drops).transform.position; //Set the colonist to drop resources at the nearest drop point
                distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
                drops = null; //Clear the drop array
                task = TaskList.Delivering; //Set the colonist to be delivering
            }
            else
            {
                task = TaskList.Idle; //Set the Colonist to be Idling
            }
        }

        if (heldResource >= maxHeldResource) //Is the colonist carrying the max amount of resources?
        {
            targetNode.GetComponent<NodeManager>().gatherers--; //Remove itself from the node's gatherers
            isGathering = false; //Set the colonist to not be gathering
            drops = GameObject.FindGameObjectsWithTag("Drops"); //Fill the array of drops with all possible drop locations
            agent.destination = GetClosestDropOff(drops).transform.position; //Set the colonist to drop resources at the nearest drop point
            distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
            drops = null; //Clear the drop array
            task = TaskList.Delivering; //Set the colonist to be delivering
            GetComponent<NavMeshObstacle>().enabled = false; //Disable the NavMeshObstacle component
            GetComponent<NavMeshAgent>().enabled = true; //Enable the NavMeshAgent component

        }

        if (Input.GetMouseButtonDown(1) && GetComponent<ObjectInfo>().isSelected) //Is the Player right clicking while this colonist is selected?
        {
            RightClick(); //Calls the RightClick() Method
        }
    }

    //Retrieves all of the resource drop points and returns the closest one.
    GameObject GetClosestDropOff(GameObject[] dropOffs)
    {
        GameObject closestDrop = null; //The closest drop point

        float closestDistance = Mathf.Infinity; //The closest drop distance

        Vector3 position = transform.position;//This colonist's position

        foreach (GameObject targetDrop in dropOffs) //For every drop point
        {
            Vector3 direction = targetDrop.transform.position - position; //The direction from the colonist to the drop point

            float distance = direction.sqrMagnitude; //the distance from the colonist to the drop point

            if (distance < closestDistance) //Is the drop in question closer than the closest drop?
            {
                closestDistance = distance; //Set the closest distance to be equal to the drop's distance
                closestDrop = targetDrop; //Set the closest drop to be equal to the drop in question
            }
        }

        return closestDrop; //returns the drop that is closest
    }

    //Performs different actions based on what the player clicks
    public void RightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Creates a ray from the camera to where the player clicks
        RaycastHit hit; //The object the ray hits

        if (Physics.Raycast(ray, out hit, 100)) //Did the ray hit anything?
        {
            if (hit.collider.tag == "Ground") //Did the player click the ground?
            {
                if (isGathering) //Is the colonist gathering?
                {
                    targetNode.GetComponent<NodeManager>().gatherers--; //Remove the colonist from the node's gatherers
                    isGathering = false; //Stop the colonist from gathering
                    isGatherer = false; //Set the colonist to not be a gatherer
                }

                AL.Move(agent, hit); //Calls the Move() Method from ActionList
                task = TaskList.Moving; //Sets the colonist to be moving
                GetComponent<NavMeshObstacle>().enabled = false; //Disables the NavMeshObstacle component
                GetComponent<NavMeshAgent>().enabled = true; //Enables the NavMeshAgent component
            }
            else if (hit.collider.tag == "Resource") //Did the player click on a resource node?
            {
                AL.Move(agent, hit); //Calls the Move() Method from ActionList
                targetNode = hit.collider.gameObject; //Sets the targetNode to be the node that was clicked
                task = TaskList.Gathering; //Sets the colonist to be gathering
            }
            else if (hit.collider.tag == "Drops") //Did the player click a drop point?
            {
                targetNode.GetComponent<NodeManager>().gatherers--; //Remove the colonist from the node's gatherers
                isGathering = false; //Stop the colonist from gathering
                drops = GameObject.FindGameObjectsWithTag("Drops"); //Fills the drop array with all possible drop points
                agent.destination = GetClosestDropOff(drops).transform.position; //Sets the colonist's destination to the closest drop point
                distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
                drops = null; //Clears the drop array
                task = TaskList.Delivering; //Sets the colonist to be delivering
                GetComponent<NavMeshObstacle>().enabled = false; //Disable the NavMeshObstacle component
                GetComponent<NavMeshAgent>().enabled = true; //Enables the NavMeshAgent component
            }
        }
    }

    //Sets the colonist to be gathering
    public void Gather()
    {
        isGathering = true; //Allows the colonist to gather

        if (!isGatherer) //Is the colonist a gatherer?
        {
            targetNode.GetComponent<NodeManager>().gatherers++; //Add the colonist to the node's gatherers
            isGatherer = true; //Sets the colonist to be a gatherer
        }

        heldResourceType = targetNode.GetComponent<NodeManager>().resourceType; //Sets the resource that the colonist is holding to the same as the node
        GetComponent<NavMeshObstacle>().enabled = true; //Enables the NavMeshObstacle component
        GetComponent<NavMeshAgent>().enabled = false; //Disable the NavMeshAgent component
    }

    //Increments the held resource when the Colonist is gathering
    IEnumerator GatherTick()
    {
        while (true) //Is this CoRoutine running?
        {
            yield return new WaitForSeconds(1); //Wait for 1 second

            if (isGathering) //Is the colonist gathering?
            {
                heldResource++; //Adds 1 to the colonist's held resources
            }
        }
    }
}

