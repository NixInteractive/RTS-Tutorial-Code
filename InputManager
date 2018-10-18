using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script handles basic user input. It should be attached to the player object
public class InputManager : MonoBehaviour {

    public bool hasPrimary; //Does the player have a primary selected object?

    public CanvasGroup UnitPanel; //The unit information panel

    private Vector2 boxStart; //The mouse coordinates stored when the player clicks
    private Vector2 boxEnd; //The mouse coordinates stored when the player releases the mouse button

    public GameObject primaryObject; //The primary selected game object

    private Rect selectBox; //The selection box

    public Texture boxTex; //The selection box texture

    public ObjectInfo selectedInfo; //The primary object's information

    private GameObject[] units; //An array of units
	
	// Update is called once per frame
	void Update () {

        hasPrimary = primaryObject; //If there is a primary object, hasPrimary is true

        UnitPanel = GameObject.Find("UnitPanel").GetComponent<CanvasGroup>(); //Assigns the UnitPanel object

        if (Input.GetMouseButtonDown(0)) //Is the player left clicking?
        {
            LeftClick(); //Calls the LeftClick() method
        }

        if(Input.GetMouseButton(0) && boxStart == Vector2.zero) //Is the player holding down the left mouse button and is boxStart zero?
        {
            boxStart = Input.mousePosition; //Sets the boxStart to equal the mouse position when the player first started holding the mouse button down
        }
        else if(Input.GetMouseButton(0) && boxStart != Vector2.zero) //Is the player holding down the left mouse button and is boxStart not equal to zero
        {
            boxEnd = Input.mousePosition; //Sets the boxEnd to equal the mouse position
        }

        if (Input.GetMouseButtonUp(0)) //Did the player release the left mouse button?
        {
            units = GameObject.FindGameObjectsWithTag("Selectable"); //Stores all selectable objects into an array

            MultiSelect(); //Calls the MultiSelect() method
        }

        selectBox = new Rect(boxStart.x, Screen.height - boxStart.y, boxEnd.x - boxStart.x, -1 * ((Screen.height - boxStart.y) - (Screen.height - boxEnd.y))); //Creates the selection box values

        if (primaryObject != null) //Is there a primary object?
        {
            UnitPanel.alpha = 1; //Sets the unit panel to be visible
            UnitPanel.blocksRaycasts = true; //Sets the unit panel to block raycasts
            UnitPanel.interactable = true; //Sets the unit panel to be interactable
        }
        else
        {
            UnitPanel.alpha = 0; //Sets the unit panel to be invisible
            UnitPanel.blocksRaycasts = false; //Sets the unit panel to not block raycasts
            UnitPanel.interactable = false; //Sets the unit panel to be not interactable
        }
    }

    //Selects all units within the selection box
    public void MultiSelect()
    {
        foreach(GameObject unit in units) //For each unit in the array of selectable objects
        {
            if (unit.GetComponent<ObjectInfo>().isUnit) //Is the object in question a unit?
            {
                Vector2 unitPos = Camera.main.WorldToScreenPoint(unit.transform.position); //Translate the unit's screen position into 2D coordinates

                if (selectBox.Contains(unitPos, true)) //Is the unit inside the selection box?
                {

                    if(!hasPrimary) //Is there not a primary selected object?
                    {
                        primaryObject = unit; //This unit becomes the primary object
                        unit.GetComponent<ObjectInfo>().isPrimary = true; //Sets the unit to be the primary
                    }

                    unit.GetComponent<ObjectInfo>().isSelected = true; //Sets the unit to be selected
                }
            }
        }

        boxStart = Vector2.zero; //Sets the boxStart to be zero
        boxEnd = Vector2.zero; //Sets the boxEnd to be zero
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

    //This draws the selection box on the screen
    void OnGUI()
    {
        if(boxStart != Vector2.zero && boxEnd != Vector2.zero) //Does the selection box have any values?
        {
            GUI.DrawTexture(selectBox, boxTex); //Draws a box using the selection box values
        }
    }
}
