using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script contains the basic information for selectable objects. It gets attached to all selectable objects
public class ObjectInfo : MonoBehaviour {

    #region Node Values
    public enum ResourceTypes { Ice, Iron, Food } //Possible resource types

    public ResourceTypes resourceType; //Creates the enum for this node

    public float availableResource; //How much can be harvested from this node

    public int gatherers; //How many units gathering from this node

    #endregion

    #region Colonist Values
    public ResourceManager RM; //Retrieves the ResourceManager

    public ResourceTypes heldResourceType; //The type of resource the colonist is carrying

    public bool isGathering = false; //Is the Colonist currently harvesting resources?
    public bool isGatherer = false; //Has the Colonist been counted as a gatherer?

    public int heldResource; //The amount off resources the Colonist is holding
    public int maxHeldResource; //The maximum amount the Colonist can carry

    public GameObject[] drops; //An array of possible drop off locations

    public GameObject targetNode; //This colonist's target node for gathering

    #endregion

    #region Enums
    public enum ObjectTypes { Node, Building, Unit}; //Possible object types
    public ObjectTypes objectType; //This object's type

    public enum Ranks { Recruit } //All of the available ranks
    public Ranks rank; //Creates the rank enum for this object

    public enum TaskList { Gathering, Attacking, Moving, Building, Idle, Delivering}; //Possible tasks this object can perform
    public TaskList task; //The task this object is currently performing
    #endregion

    #region References
    private GUIManager GM; //A reference to our GUIManager object

    public GameObject selectionIndidcator; //The object that shows the unit is selected
    public GameObject iconCam; //The Camera that feeds into the unitIcon
    public GameObject target; //This object's current target

    public NavMeshAgent agent; //The NavMeshAgent attached to the Colonist
    #endregion

    #region Bools
    public bool isPrimary = false; //Is the object the primary object?
    public bool isColonist; //Is this unit a colonist?
    public bool isUnit; //Is the object a unit?
    public bool isPlayerObject; //Does the player own this object?
    public bool isAllyObject; //Does an ally own this object?
    public bool canAttack; //Can this object attack?
    #endregion

    public string objectName; //The object's name
    public string toolTipInfo; //The object's description
    public string[] actions = new string[16]; //Object's available actions

    #region Attributes
    public float health; //The object's current health
    public float maxHealth; //The object's max health
    public float energy; //The object's current energy
    public float maxEnergy; //The object's max energy
    public float patk; //The object's physical attack
    public float pdef; //The object's physical defense
    public float eatk; //The object's energy attack
    public float edef; //The object's energy defense
    public float range; //The attack range of the object

    public int kills; //The object's kill count
    #endregion

    public float distToTarget; //Distance to the target point
    public float attackSpeed; //How fast the target can attack. Higher number means slower attack

    internal bool isSelected { get; set; } //Retrieves the object's selected value

    // Use this for initialization
    void Start () {

        health = maxHealth; //Set health = maxHealth
        energy = maxEnergy; //Set energy = maxEnergy

        StartCoroutine(AttackTick()); //Starts the AttackTick() IEnumerator

        //If this object is a node or a colonist, start the OneTick() IEnumerator
        if(objectType == ObjectTypes.Node || isColonist)
        {
            StartCoroutine(OneTick());
        } 

        agent = GetComponent<NavMeshAgent>(); //Retrieves the NavMeshAgent Component

        iconCam = GetComponentInChildren<Camera>().gameObject; //Retrieves the camera for the iconCam

        GM = GameObject.Find("Player").GetComponent<GUIManager>(); //Assigns the GUIManager Reference
    }
	
	// Update is called once per frame
	void Update () {

        #region Colonist Functions

        //Is this object a colonist?
        if (isColonist)
        {
            if (task == TaskList.Gathering) //Is the Colonist currently gathering?
            {
                distToTarget = Vector3.Distance(targetNode.transform.position, transform.position); //Gets the distance to the target node

                if (distToTarget <= 3.5f) //Is the distance to the target less than 3.5?
                {
                    Gather(); //Calls the Gather() Method
                }
            }

            if (task == TaskList.Delivering) //Is the Colonist currently delivering?
            {
                if (distToTarget <= 4f) //Is the distance to the target drop less than 4?
                {
                    if (heldResourceType == ResourceTypes.Ice) //Is this Colonist holding Ice?
                    {
                        if (RM.ice >= RM.maxIce) //Is the stored amount of Ice greater than or equal to the max amount of ice?
                        {
                            task = TaskList.Idle; //Set the Colonist to be idle
                            isGatherer = false; //Set the Colonist to not be a gatherer
                        }
                        else if (RM.ice + heldResource >= RM.maxIce) //Is the stored amount of ice going to exceed the max when the Colonist delivers?
                        {
                            int resourceOverflow = (int)RM.maxIce - (int)RM.ice; //How much ice can be stored before hitting capacity

                            heldResource -= resourceOverflow; //Remove the ice that can be stored from the colonist
                            RM.ice = RM.maxIce; //Set the stored ice to equal the maximum
                            task = TaskList.Gathering; //Set the Colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the Colonist to not be a gatherer
                        }
                        else
                        {
                            RM.ice += heldResource; //Add the Colonist's ice to the stored ice
                            heldResource = 0; //Empty the Colonist's ice storage
                            task = TaskList.Gathering; //Set the Colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the Colonist to not be a gatherer
                        }
                    }
                    else if (heldResourceType == ResourceTypes.Iron)  //Is this Colonist holding Iron?
                    {
                        if (RM.iron >= RM.maxIron) //Is the stored amount of Iron greater than or equal to the max amount of Iron?
                        {
                            task = TaskList.Idle; //Set the colonist to be idle
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                        else if (RM.iron + heldResource >= RM.maxIron) //Is the stored amount of Iron going to exceed the max when the colonist delivers?
                        {
                            int resourceOverflow = (int)RM.maxIron - (int)RM.iron; //How much Iron can be stored before hitting capacity

                            heldResource -= resourceOverflow; //Remove the Iron that can be stored from the colonist
                            RM.iron = RM.maxIron; //Set the stored Iron to equal the maximum
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                        else
                        {
                            RM.iron += heldResource; //Add the colonist's Iron to the stored Iron
                            heldResource = 0; //Empty the colonist's Iron storage
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                    }
                    else if (heldResourceType == ResourceTypes.Food) //Is this Colonist holding Food?
                    {
                        if (RM.food >= RM.maxFood) //Is the stored amount of Ice greater than or equal to the max amount of Food?
                        {
                            task = TaskList.Idle; //Set the colonist to be idle
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                        else if (RM.food + heldResource >= RM.maxFood) //Is the stored amount of Food going to exceed the max when the colonist delivers?
                        {
                            int resourceOverflow = (int)RM.maxFood - (int)RM.food; //How much Food can be stored before hitting capacity

                            heldResource -= resourceOverflow; //Remove the Food that can be stored from the colonist
                            RM.food = RM.maxFood; //Set the stored Food to equal the maximum
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                        else
                        {
                            RM.food += heldResource; //Add the colonist's Food to the stored Food
                            heldResource = 0; //Empty the colonist's Food storage
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            target = targetNode; //Set the target to be the target node
                            agent.destination = target.transform.position; //Set the Colonist's destination
                            isGatherer = false; //Set the colonist to not be a gatherer
                        }
                    }
                }
            }

            if (targetNode == null && task == TaskList.Gathering) //Does the node the colonist was gathering from still exist?
            {

                if (heldResource != 0) //Is the colonist still holding resources?
                {
                    drops = GameObject.FindGameObjectsWithTag("Drops"); //Fill the array of drops with all possible drop locations
                    agent.destination = GetClosestDropOff(drops).transform.position; //Set the colonist to drop resources at the nearest drop point
                    distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
                    drops = null; //Clear the drop array
                    task = TaskList.Delivering; //Set the colonist to be delivering
                    targetNode = null; //Clear the target node reference
                }
                else
                {
                    task = TaskList.Idle; //Set the Colonist to be Idling
                    targetNode = null; //Clear the target node reference
                }
            }

            if (heldResource >= maxHeldResource) //Is the colonist carrying the max amount of resources?
            {
                targetNode.GetComponent<ObjectInfo>().gatherers--; //Remove itself from the node's gatherers
                isGathering = false; //Set the colonist to not be gathering
                drops = GameObject.FindGameObjectsWithTag("Drops"); //Fill the array of drops with all possible drop locations
                target = GetClosestDropOff(drops); //Set the colonist to drop resources at the nearest drop point
                agent.destination = target.transform.position; //Set the NavMeshAgent's destination to be the target's position
                distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
                drops = null; //Clear the drop array
                task = TaskList.Delivering; //Set the colonist to be delivering

            }
        }
        #endregion

        //If this object is not a node and has 0 or less health, destroy this object
        if(objectType != ObjectTypes.Node)
        {
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }

        //If this object is a unit and at the set destination, set to Idle 
        if (objectType == ObjectTypes.Unit)
        {
            if (transform.position == agent.destination && task == TaskList.Moving) //Is the Colonist done moving?
            {
                task = TaskList.Idle; //Set the Colonist to be idle
            }
        }

        //If this object is set to attack and has a target, check if the target is in range
        if (task == TaskList.Attacking)
        {
            if (target)
            {
                distToTarget = Vector3.Distance(target.transform.position, transform.position);

                //If the target isn't in range, set canAttack to false and move closer to the target.
                //If the target is in range, set canAttack to true
                if (distToTarget >= range)
                {
                    canAttack = false;
                    agent.destination = target.transform.position;
                }
                else if (distToTarget <= range)
                {
                    canAttack = true;
                }
            }
        }

        //If there is no target, set canAttack to false and task to idle
        if (!target)
        {
            canAttack = false;
            task = TaskList.Idle;
        }

        //If this object is a resource node that has no available resource left, destroy it
        if (objectType == ObjectTypes.Node && availableResource <= 0)
        {
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(1) && isSelected) //Is the Player right clicking while this colonist is selected?
        {
            RightClick(); //Calls the RightClick() Method
        }

        selectionIndidcator.SetActive(isSelected); //Sets the status of the selectionIndicator based on if this object is selected

        iconCam.SetActive(isPrimary); //Sets the iconCam's status based on if this object is the primary object
    }

    //Increments the available resources down
    public void ResourceGather()
    {
        if (gatherers != 0) //Are there any units gathering from this node?
        {
            availableResource -= gatherers; //Subtract the current number of units gathering from the available resources
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
                    targetNode.GetComponent<ObjectInfo>().gatherers--; //Remove the colonist from the node's gatherers
                    isGathering = false; //Stop the colonist from gathering
                    isGatherer = false; //Set the colonist to not be a gatherer
                }

                Move(agent, hit); //Calls the Move() Method from ActionList
                task = TaskList.Moving; //Sets the colonist to be moving
                GetComponent<NavMeshObstacle>().enabled = false; //Disables the NavMeshObstacle component
                GetComponent<NavMeshAgent>().enabled = true; //Enables the NavMeshAgent component
            }
            else if (hit.collider.tag == "Resource") //Did the player click on a resource node?
            {
                Move(agent, hit); //Calls the Move() Method from ActionList
                targetNode = hit.collider.gameObject; //Sets the targetNode to be the node that was clicked
                target = hit.collider.gameObject; //Sets the targetNode to be the node that was clicked
                task = TaskList.Gathering; //Sets the colonist to be gathering
            }
            else if (hit.collider.tag == "Drops") //Did the player click a drop point?
            {
                targetNode.GetComponent<ObjectInfo>().gatherers--; //Remove the colonist from the node's gatherers
                isGathering = false; //Stop the colonist from gathering
                drops = GameObject.FindGameObjectsWithTag("Drops"); //Fills the drop array with all possible drop points
                agent.destination = GetClosestDropOff(drops).transform.position; //Sets the colonist's destination to the closest drop point
                distToTarget = Vector3.Distance(GetClosestDropOff(drops).transform.position, transform.position); //The distance to the target drop point
                drops = null; //Clears the drop array
                task = TaskList.Delivering; //Sets the colonist to be delivering
                GetComponent<NavMeshObstacle>().enabled = false; //Disable the NavMeshObstacle component
                GetComponent<NavMeshAgent>().enabled = true; //Enables the NavMeshAgent component
            }
            else if(hit.collider.tag == "Selectable") //Did the player click a selectable object?
            {
                ObjectInfo hitObject = hit.collider.GetComponent<ObjectInfo>(); //Retrieves the clicked object's info

                //If the clicked object isn't a player object or ally object, target it and set task to attacking
                if(hitObject.isPlayerObject == false && hitObject.isAllyObject == false)
                {
                    target = hit.collider.gameObject;
                    task = TaskList.Attacking;
                }
            }
        }
    }

    //This method takes a NavMeshAgent and a RaycastHit and tells the agent to move to the hit location.
    public void Move(NavMeshAgent agent, RaycastHit hit)
    {
        agent.destination = hit.point;
    }

    //Sets the colonist to be gathering
    public void Gather()
    {
        isGathering = true; //Allows the colonist to gather

        if (!isGatherer) //Is the colonist a gatherer?
        {
            targetNode.GetComponent<ObjectInfo>().gatherers++; //Add the colonist to the node's gatherers
            isGatherer = true; //Sets the colonist to be a gatherer
        }

        heldResourceType = target.GetComponent<ObjectInfo>().resourceType; //Sets the resource that the colonist is holding to the same as the node
    }

    //If the player hovers over this object witht he mouse, display this object's description in the tooltip
    private void OnMouseOver()
    {
        GM.tooltipInfo = toolTipInfo;
    }

    //When the player stops hovering over this object, clear the tooltip
    private void OnMouseExit()
    {
        GM.tooltipInfo = "Hover the Mouse over an Object to see a ToolTip";
    }

    //Called when object is enabled
    private void OnEnable()
    {
        InputManager.selectedObjects.Add(this);
    }

    //Called when object is disabled
    private void OnDisable()
    {
        InputManager.selectedObjects.Remove(this);
    }

    //Handles the incrementation process
    IEnumerator OneTick()
    {
        while (true) //Is this CoRoutine running?
        {
            yield return new WaitForSeconds(1); //Wait for 1 second

            //Is this object a resource node?
            if(objectType == ObjectTypes.Node)
            {
                ResourceGather(); //Calls the ResourceGather() method
            }

            //Is this object a colonist?
            if (isColonist)
            {
                if (isGathering) //Is the colonist gathering?
                {
                    heldResource++; //Adds 1 to the colonist's held resources
                }
            }
        }
    }

    //Handles the attack process
    IEnumerator AttackTick()
    {
        while (true) //Is this CoRoutine running?
        {
            yield return new WaitForSeconds(attackSpeed); //Wait for seconds equal to this object's attack speed

            //If this object can attack, obtain the target's info and subtract both physical and energy damage from their health
            if (canAttack)
            {
                ObjectInfo targetInfo = target.GetComponent<ObjectInfo>();

                targetInfo.health -= Mathf.Round(patk * (1 - (targetInfo.pdef * 0.05f)));
                targetInfo.health -= Mathf.Round(eatk * (1 - (targetInfo.edef * 0.05f)));

                Debug.Log(targetInfo.health);
            }
        }
    }
}
