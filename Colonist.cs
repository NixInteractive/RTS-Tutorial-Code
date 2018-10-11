using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script contains information specific to the colonist unit. It gets attached to the Colonist Object.
public class Colonist : MonoBehaviour {

    //Retrieves the TaskList
    public TaskList task;

    //Retrieves the ResourceManager
    public ResourceManager RM;

    //Retrieves the ActionList
    private ActionList AL;

    //The resource node the colonist is currently harvesting
    GameObject targetNode;

    //The type of resource the colonist is holding.
    public NodeManager.ResourceTypes heldResourceType;

    //Is the Colonist currently harvesting resources?
    public bool isGathering = false;

    //The NavMeshAgent attached to the Colonist
    private NavMeshAgent agent;

    //How much resource the Colonist is carrying and how much they can carry.
    public int heldResource;
    public int maxHeldResource;

    //An array of possible drop off locations
    public GameObject[] drops;

    // Use this for initialization
    void Start () {

        //Starts the incremental gathering 
        StartCoroutine(GatherTick());

        //Retrieves the NavMeshAgent Component
        agent = GetComponent<NavMeshAgent>();

        //Finds the object with the ActionList script and pulls the information
        AL = FindObjectOfType<ActionList>();
    }
	
	// Update is called once per frame
	void Update () {

        //If the node is completely harvested this makes the Colonist drop off any remaining held resources
        if (targetNode == null)
        {
            if (heldResource != 0)
            {
                drops = GameObject.FindGameObjectsWithTag("Drops");
                agent.destination = GetClosestDropOff(drops).transform.position;
                drops = null;
                task = TaskList.Delivering;
            }
            else
            {
                task = TaskList.Idle;
            }
        }

        //Checks if the Colonist has to drop off the gathered resources
        if (heldResource >= maxHeldResource)
        {
            
            drops = GameObject.FindGameObjectsWithTag("Drops");
            agent.destination = GetClosestDropOff(drops).transform.position;
            drops = null;
            task = TaskList.Delivering;
            GetComponent<NavMeshObstacle>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = true;

        }

        //Calls the RightClick Method when the player right clicks while the colonis is selected.
        if (Input.GetMouseButtonDown(1) && GetComponent<ObjectInfo>().isSelected)
        {
            RightClick();
        }
    }

    //Retrieves all of the resource drop points and returns the closest one.
    GameObject GetClosestDropOff(GameObject[] dropOffs)
    {
        GameObject closestDrop = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject targetDrop in dropOffs)
        {
            Vector3 direction = targetDrop.transform.position - position;
            float distance = direction.sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDrop = targetDrop;
            }
        }

        return closestDrop;
    }

    //Performs different actions based on what the player clicks
    public void RightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {

            //Moves the Colonist if the player clicks the ground
            if (hit.collider.tag == "Ground")
            {
                AL.Move(agent, hit);
                task = TaskList.Moving;
            }

            //Tasks the Colonist to gather if the player clicks a resource node.
            else if (hit.collider.tag == "Resource")
            {
                AL.Move(agent, hit);
                targetNode = hit.collider.gameObject;
                task = TaskList.Gathering;
            }
        }
    }

    //Performs different actions based on what the Colonist hits
    public void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        //If the hit object is a resource node the Colonist starts harvesting
        if (hitObject.tag == "Resource" && task == TaskList.Gathering)
        {
            isGathering = true;
            hitObject.GetComponent<NodeManager>().gatherers++;
            heldResourceType = hitObject.GetComponent<NodeManager>().resourceType;
            GetComponent<NavMeshObstacle>().enabled = true;
            GetComponent<NavMeshAgent>().enabled = false;
        }

        //If it's a drop off point the Colonist drops off it's held resources and goes back to it's target node if applicable
        else if (hitObject.tag == "Drops" && task == TaskList.Delivering)
        {
            if (RM.ice >= RM.maxIce)
            {
                task = TaskList.Idle;
            }
            else
            {
                RM.ice += heldResource;
                heldResource = 0;
                task = TaskList.Gathering;
                agent.destination = targetNode.transform.position;
            }
        }
    }

    //Cancels the gathering action when the Colonist leaves the node. This will probably be deleted in favor of a better method
    public void OnTriggerExit(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.tag == "Resource")
        {
            hitObject.GetComponent<NodeManager>().gatherers--;
            isGathering = false;
            }
    }

    //Increments the held resource when the Colonist is gathering
    IEnumerator GatherTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (isGathering)
            {
                heldResource++;
            }
        }
    }
}
