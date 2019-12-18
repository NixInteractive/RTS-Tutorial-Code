using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script controls the in-game Selection HUD and all  object-related items displayed to the player on screen. This gets attached to the player object
public class GUIManager : MonoBehaviour {

    public ObjectInfo primary; //The object information to be displayed

    public Animator minimapAnim; //The Animator component attached to the minimap
    public Animator toolTipAnim; //The Animator component attached to the Tool Tip

    public Slider HB; //The slider object that acts as a health bar
    public Slider EB; //The slider object that acts as an energy bar

    public Text nameDisp; //The unit name text object
    public Text healthDisp; //The unit health text object
    public Text energyDisp; //The unit energy text object
    public Text patkDisp; //The unit physical attack text object
    public Text pdefDisp; //The unit physical defense text object
    public Text eatkDisp; //The unit energy attack text object
    public Text edefDisp; //The unit energy defense text object
    public Text rankDisp; //The unit rank text object
    public Text killDisp; //The unit kill count text object
    public Text toolTipText; //The Tool Tip text object

    private bool isMiniMapVisible = true; //Is the minimap visible?
    private bool isToolTipVisible = false; //Is the Tool Tip visible?

    public string tooltipInfo; //The text to be displayed in the Tool Tip

    // Use this for initialization
    private void Start()
    {
        tooltipInfo = "Hover the Mouse over an Object to see a ToolTip"; //Sets the default Tool Tip text
    }

    // Update is called once per frame
    void Update()
    {
        primary = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().selectedInfo; //Assigns the primary object

        toolTipText.text = tooltipInfo; //Sets the displayed text to be equal to the desired text

        //Checks to see if the Tool Tip text is equal to the default text and sets the visibility accordingly
        if (tooltipInfo == "Hover the Mouse over an Object to see a ToolTip")
        {
            isToolTipVisible = false;
        }
        else
        {
            isToolTipVisible = true;
        }

        //Toggles the minimap visibility when the player presses the "M" key 
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isMiniMapVisible)
            {
                isMiniMapVisible = false;
            }
            else
            {
                isMiniMapVisible = true;
            }
        }

        //Sets the minimap Animator's "isVisible" bool based on the minimap's visibility
        if (isMiniMapVisible)
        {
            minimapAnim.SetBool("isVisible", true);
        }
        else
        {
            minimapAnim.SetBool("isVisible", false);
        }

        //Sets the Tool Tip Animator's "isVisible" bool based on the Tool Tip's visibility
        if (!isToolTipVisible)
        {
            toolTipAnim.SetBool("isVisible", false);
        }
        else
        {
            toolTipAnim.SetBool("isVisible", true);
        }

        //Checks to see if there is an object selected
        if (primary)
        {
            if (primary.maxEnergy <= 0) //Does the selected object have energy?
            {
                EB.gameObject.SetActive(false); //Deactivates the Energy Bar
            }

            HB.maxValue = primary.maxHealth; //Sets the max value of the health bar to be equal to the selected unit's max health
            HB.value = primary.health; //Sets the value of the health bar to be equal to the selected object's current health

            EB.maxValue = primary.maxEnergy; //Sets the max value of the energy bar to be equal to the selected unit's max energy
            EB.value = primary.energy; //Sets the value of the energy bar to be equal to the selected object's current energy

            nameDisp.text = primary.objectName; //Displays the unit's name
            healthDisp.text = "HP: " + primary.health; //Displays the unit's health
            energyDisp.text = "EP: " + primary.energy; //Displays the unit's energy
            patkDisp.text = "PATK: " + primary.patk; //Displays the unit's physical attack
            pdefDisp.text = "PDEF: " + primary.pdef; //Displays the unit's physical defense
            eatkDisp.text = "EATK: " + primary.eatk; //Displays the unit's energy attack
            edefDisp.text = "EDEF: " + primary.edef; //Displays the unit's energy defense
            rankDisp.text = "" + primary.rank; //Displays the unit's rank
            killDisp.text = "Kills: " + primary.kills; //Displays the unit's kill count
        }
    }
}

