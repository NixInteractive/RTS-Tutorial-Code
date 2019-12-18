using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script handles basic user input. It should be attached to the player object
public class InputManager : MonoBehaviour {

    [SerializeField] LayerMask mask; //The layer we want our Raycast to hit

    public bool hasPrimary; //Does the player have a primary selected object?
    public bool circlePlaced = false; //Has the selection circle been placed?

    public CanvasGroup ObjectPanel; //The unit information panel

    public GameObject primaryObject; //The primary selected game object
    public GameObject selectionCircle; //The actual selection circle
    public GameObject selectionCirclePrefab; //The prefab for the selection circle

    public ObjectInfo selectedInfo; //The primary object's information

    private GameObject[] units; //An array of units

    private Collider[] colliders; //An array of colliders that have been selected


	
	// Update is called once per frame
	void Update () {

        hasPrimary = primaryObject; //If there is a primary object, hasPrimary is true

        if (Input.GetMouseButtonDown(0)) //Is the player left clicking?
        {
            LeftClick(); //Calls the LeftClick() method
        }

        if(Input.GetMouseButton(0) && selectionCircle == null) //Is the player holding the left mouse button while there is no selection circle?
        {
            circlePlaced = true; //The selection circle has been placed
            CircleCreate(); //Calls the CircleCreate method
        }

        if (Input.GetMouseButtonUp(0)) //Did the player release the left mouse button?
        {
            MultiSelect(); //Calls the MultiSelect method
            Destroy(selectionCircle); //Destroys the selection circle
            selectionCircle = null; //Removes the reference to the selection circle
            colliders = null; //Empties the colliders array
            circlePlaced = false; //The circle is no longer placed
        }

        if(circlePlaced == true) //Has the circle been placed?
        {
            selectionCircle.transform.localScale = new Vector3(selectionCircle.transform.localScale.x + 0.75f, selectionCircle.transform.localScale.y, selectionCircle.transform.localScale.z + 0.75f); //Increase the x and z scale values of the selection circle
        }

        if (primaryObject != null) //Is there a primary object?
        {
            ObjectPanel.alpha = 1; //Sets the unit panel to be visible
            ObjectPanel.blocksRaycasts = true; //Sets the unit panel to block raycasts
            ObjectPanel.interactable = true; //Sets the unit panel to be interactable
        }
        else
        {
            ObjectPanel.alpha = 0; //Sets the unit panel to be invisible
            ObjectPanel.blocksRaycasts = false; //Sets the unit panel to not block raycasts
            ObjectPanel.interactable = false; //Sets the unit panel to be not interactable
        }
    }

    //Creates and places a selection circle where the player clicked
    public void CircleCreate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Creates a ray from the camera to where the player clicked
        RaycastHit hit; //The point the ray hits

        if (Physics.Raycast(ray, out hit, 500, mask)) //Did the ray hit anything on the Layer Mask?
        {
            selectionCircle = Instantiate(selectionCirclePrefab, hit.point, new Quaternion(0, 0, 0, 0)); //Creates and places the selection circle
        }
    }

    //Selects all units within the selection circle
    public void MultiSelect()
    {
        colliders = Physics.OverlapSphere(selectionCircle.transform.position, selectionCircle.transform.localScale.x * 0.9f); //Grabs all of the colliders the selection circle intersects

        foreach (Collider collider in colliders) //For every collider to be selected
        {
            if(collider.gameObject.tag == "Selectable") //Is the collider on a selectable object?
            {
                if(collider.GetComponent<ObjectInfo>().isUnit && collider.GetComponent<ObjectInfo>().isPlayerObject) //Is the collider on a unit owned by the player?
                {
                    if (!hasPrimary) //Checks to see if the player has no primary object selected
                    {
                        primaryObject = collider.gameObject; //Sets this object to be the primary object
                        collider.GetComponent<ObjectInfo>().isPrimary = true; //Tells the object it's the primary
                        selectedInfo = collider.GetComponent<ObjectInfo>(); //Displays this object's information
                    }

                    collider.GetComponent<ObjectInfo>().isSelected = true; //Sets the object to be selected
                }
            }
        }
    }


    //Performs actions based on what the player clicks
    public void LeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Creates a ray from the camera to where the player clicks
        RaycastHit hit; //The object the ray hits

        if(Physics.Raycast(ray, out hit, 100)) //Does the ray hit anything?
        {
            if(hit.collider.tag == "Ground") //Did the player click the ground?
            {
                selectedInfo.isSelected = false; //Deselects the selected units
                selectedInfo.isPrimary = false; //Deselects the primary unit
                primaryObject = null; //Clears the primary object

                units = GameObject.FindGameObjectsWithTag("Selectable"); //Grabs all selectable objects

                foreach (GameObject unit in units) //For each selectable object
                {
                    unit.GetComponent<ObjectInfo>().isSelected = false; //Deselects all selectable objects
                }

                selectedInfo = null; //Clears out the selected info
            }
            else if(hit.collider.tag == "Selectable") //Did the player click a selectable object?
            {

                units = GameObject.FindGameObjectsWithTag("Selectable"); //Grabs all selectable objects

                foreach (GameObject unit in units) //For each selectable object
                {
                    unit.GetComponent<ObjectInfo>().isSelected = false; //Deselects all selectable objects
                }

                if (hasPrimary) //Is there a primary object?
                {
                    selectedInfo.isSelected = false; //Deselects the selected units
                    selectedInfo.isPrimary = false; //Deselects the primary unit
                    primaryObject = null; //Clears the primary object
                }

                primaryObject = hit.collider.gameObject; //Sets the primary object

                selectedInfo = primaryObject.GetComponent<ObjectInfo>(); //Sets the selected info

                selectedInfo.isSelected = true; //Sets the selected unit to be selected
                selectedInfo.isPrimary = true; //Sets the selected unit to be the primary unit
            }
        }
    }
}
