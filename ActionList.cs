using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionList : MonoBehaviour
{
    //Currently Selected Units
    public List<ObjectInfo> selectedUnits = InputManager.selectedObjects;

    //Actions & stored action arrays
    public string[] actions = new string[16];
    public string[] storedActions;

    //Colonist Only, building actions
    public string[] standardBuildings = new string[16];
    public string[] advancedBuildings = new string[16];

    public bool moveButtonActive = false; //Is the move action active?
    public bool buildModeActive = false; //Is the build mode active?
    public bool ghostInstantiated = false; //Is the ghost building instantiated?

    public GameObject buildingPrefab; //Building to be built
    public GameObject ghostBuilding; //Ghost version of the building to be built

    public GameObject[] availableBuildings; //Buildings available to be built


    private void Update()
    {
        //Handles the move button action
        if (Input.GetMouseButtonDown(0))
        {
            if (moveButtonActive)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.tag == "Ground")
                    {
                        Move(hit.point);
                        moveButtonActive = false;
                    }
                }
            }
        }

        //Handles build mode
        if (buildModeActive)
        {
            if (!ghostInstantiated)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.tag == "Ground")
                    {
                        ghostBuilding = Instantiate(buildingPrefab);
                        ghostBuilding.transform.position = hit.point;
                        ghostInstantiated = true;
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        ghostBuilding.transform.position = hit.point;
                    }
                    else
                    {
                        Destroy(ghostBuilding);
                        ghostInstantiated = false;
                    }
                }
            }
        }
    }

    //Actions available for the selected unit
    #region Actions
    public void ActionOne()
    {
        Invoke(actions[0], 0f);
    }

    public void ActionTwo()
    {
        Invoke(actions[1], 0f);
    }

    public void ActionThree()
    {
        Invoke(actions[2], 0f);
    }

    public void ActionFour()
    {
        Invoke(actions[3], 0f);
    }

    public void ActionFive()
    {
        Invoke(actions[4], 0f);
    }

    public void ActionSix()
    {
        Invoke(actions[5], 0f);
    }

    public void ActionSeven()
    {
        Invoke(actions[6], 0f);
    }

    public void ActionEight()
    {
        Invoke(actions[7], 0f);
    }

    public void ActionNine()
    {
        Invoke(actions[8], 0f);
    }

    public void ActionTen()
    {
        Invoke(actions[9], 0f);
    }

    public void ActionEleven()
    {
        Invoke(actions[10], 0f);
    }

    public void ActionTwelve()
    {
        Invoke(actions[11], 0f);
    }

    public void ActionThirteen()
    {
        Invoke(actions[12], 0f);
    }

    public void ActionFourteen()
    {
        Invoke(actions[13], 0f);
    }

    public void ActionFifteen()
    {
        Invoke(actions[14], 0f);
    }

    public void ActionSixteen()
    {
        Invoke(actions[15], 0f);
    }

    public void MoveButton()
    {
        moveButtonActive = true;
    }
    #endregion

    //Move Action
    private void Move(Vector3 movePoint)
    {
        foreach (ObjectInfo selectedUnit in selectedUnits)
        {
            if (selectedUnit.isSelected)
            {
                selectedUnit.agent.destination = movePoint;
            }
        }

        moveButtonActive = false;
    }

    //Basic building toggle
    private void ShowBasicBuildings()
    {
        storedActions = actions;
        actions = standardBuildings;
    }

    //Advanced building toggle
    private void ShowAdvancedBuildings()
    {
        storedActions = actions;
        actions = advancedBuildings;
    }

    //Default action toggle
    private void CancelShowBuildings()
    {
        actions = storedActions;
    }

    //Build HQ action
    private void BuildHQ()
    {
        buildModeActive = true;
        buildingPrefab = availableBuildings[0];
    }
}
