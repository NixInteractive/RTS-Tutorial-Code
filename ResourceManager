using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script controls the Resource HUD and the overall resources the player has. This script should be attached to the Player object
public class ResourceManager : MonoBehaviour {

    public float ice; //The player's current stored ice
    public float maxIce; //The max amount of ice that can be stored
    public float iron; //The player's current stored iron
    public float maxIron; //The max amount of iron that can be stored
    public float power; //The player's current stored power
    public float maxPower; //The max amount of power that can be stored
    public float food; //The player's current stored food
    public float maxFood; //The max amount of food that can be stored
    public float oxygen; //The player's current stored oxygen
    public float maxOxygen; //The max amount of oxygen that can be stored
    public float population; //The player's current population
    public float maxPopulation; //The max amount of population that can be sustained

    public Text iceDisp; //The Text object that displays the ice values
    public Text ironDisp; //The Text object that displays the iron values
    public Text powerDisp; //The Text object that displays the power values
    public Text foodDisp; //The Text object that displays the food values
    public Text oxygenDisp; //The Text object that displays the oxygen values
    public Text populationDisp; //The Text object that displays the population values

	// Update is called once per frame
	void Update () {

        iceDisp.text = "" + ice + "/" + maxIce; //Displays the current ice out of the max ice
        ironDisp.text = "" + iron + "/" + maxIron; //Displays the current iron out of the max iron
        powerDisp.text = "" + power + "/" + maxPower; //Displays the current power out of the max power
        foodDisp.text = "" + food + "/" + maxFood; //Displays the current food out of the max food
        oxygenDisp.text = "" + oxygen + "/" + maxOxygen; //Displays the current oxygen out of the max oxygen
        populationDisp.text = "" + population + "/" + maxPopulation; //Displays the current population out of the max population
    }
}
