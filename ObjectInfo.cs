using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script contains the basic information for selectable objects. It gets attached to all selectable objects
public class ObjectInfo : MonoBehaviour {

    public GameObject selectionIndidcator; //The object that shows the unit is selected
    public GameObject iconCam; //The Camera that feeds into the unitIcon
    public GameObject target;

    public bool isPrimary = false; //Is the object the primary object?
    public bool isSelected = false; //Is the object selected?
    public bool isUnit; //Is the object a unit?
    public bool isPlayerObject; //Does the player own this object?
    public bool isAllyObject; //Does an ally own this object?

    public string objectName; //The object's name

    public float health; //The object's current health
    public float maxHealth; //The object's max health
    public float energy; //The object's current energy
    public float maxEnergy; //The object's max energy
    public float patk; //The object's physical attack
    public float pdef; //The object's physical defense
    public float eatk; //The object's energy attack
    public float edef; //The object's energy defense
    public int kills; //The object's kill count

    public enum Ranks {Recruit} //All of the available ranks

    public Ranks rank; //Creates the rank enum for this object

    // Use this for initialization
    void Start () {

        iconCam = GetComponentInChildren<Camera>().gameObject; //Retrieves the camera for the iconCam
    }
	
	// Update is called once per frame
	void Update () {

        selectionIndidcator.SetActive(isSelected); //Sets the status of the selectionIndicator based on if this object is selected

        if(health <= 0) //Is the object's health less than or equal to zero?
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Selectable"); //Retrieves all selectable objects

            foreach(GameObject unit in units) //For each selectable object
            {
                if (unit.GetComponent<ObjectInfo>().isUnit) //Is the object a unit?
                {
                    if(unit.GetComponent<ObjectInfo>().target == this.gameObject) //Is the unit targeting this object?
                    {
                        unit.GetComponent<ObjectInfo>().target = null; //Removes the unit's reference to this object as the target
                    }
                }
            }
            Destroy(gameObject); //Destroy this object
        }

        iconCam.SetActive(isPrimary); //Sets the iconCam's status based on if this object is the primary object
    }
}
