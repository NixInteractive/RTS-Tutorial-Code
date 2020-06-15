using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//This Script handles basic user input. It should be attached to the player object
public class InputManager : MonoBehaviour
{
    public ActionList AL; //ActionList Reference

    public static List<ObjectInfo> selectedObjects = new List<ObjectInfo>(); //Currently selected objects

    public Canvas canvas; //canvas reference

    public Image selectionBox; //Selection box

    [SerializeField] LayerMask mask; //The layer we want our Raycast to hit

    public bool hasPrimary; //Does the player have a primary selected object?
    public bool canClickGround = true;

    public CanvasGroup ObjectPanel; //The unit information panel

    public GameObject primaryObject; //The primary selected game object

    public ObjectInfo selectedInfo; //The primary object's information

    private GameObject[] units; //An array of units

    private Vector3 startPos; //Box start position

    private BoxCollider worldCollider; //Collider for the selection box

    private RectTransform RT; //Transform for the box selector

    private bool isSelecting; //Is the player selecting?

    //Called before start
    void Awake()
    {
        //Retrieves the canvas reference
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        //Creates the selection box
        if (selectionBox != null)
        {
            RT = selectionBox.GetComponent<RectTransform>();
            RT.pivot = Vector2.one * .5f;
            RT.anchorMin = Vector2.one * .5f;
            RT.anchorMax = Vector2.one * .5f;
            selectionBox.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        AL = GetComponent<ActionList>(); //Retrieves the ActionList reference
    }

    // Update is called once per frame
    void Update()
    {
        hasPrimary = primaryObject; //If there is a primary object, hasPrimary is true

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

        //Did the player left click while being able to click the ground and while the move button is not active?
        if (Input.GetMouseButtonDown(0) && canClickGround && !AL.moveButtonActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Did the raycast hit something?
            if (Physics.Raycast(ray, out hit))
            {
                ObjectInfo OI = hit.collider.GetComponent<ObjectInfo>();

                //Is there anything currently selected?
                if (OI != null)
                {
                    //Is the player holding left shift?
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        UpdateSelection(OI, !OI.isSelected); //Add the clicked object to selected units
                    }
                    else
                    {
                        //Replace selected units with the clicked object
                        ClearSelected();
                        UpdateSelection(OI, true);
                    }
                }
            }

            startPos = Input.mousePosition; //Set the start position
            isSelecting = true; //The player is now selecting
        }

        //Did the player release the left mouse button?
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
        }

        //Toggle the selection box based on if the player is selecting
        selectionBox.gameObject.SetActive(isSelecting);

        //Is the player selecting?
        if (isSelecting)
        {
            //Define new bounds based on current selection box
            Bounds bounds = new Bounds();
            bounds.center = Vector3.Lerp(startPos, Input.mousePosition, 0.5f);
            bounds.size = new Vector3(Mathf.Abs(startPos.x - Input.mousePosition.x), Mathf.Abs(startPos.y - Input.mousePosition.y), 0);

            //Adjust the selection bos image to match the bounds
            RT.position = bounds.center;
            RT.sizeDelta = canvas.transform.InverseTransformVector(bounds.size);

            //For each selectable object
            foreach (ObjectInfo selectable in selectedObjects)
            {
                //If it's a unit owned by the player
                if (selectable.isUnit && selectable.isPlayerObject)
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(selectable.transform.position); //Get the unit's position on the screen
                    screenPos.z = 0; //Set the screen position z to 0
                    UpdateSelection(selectable, (bounds.Contains(screenPos))); //Update selection based on if the selected object is inside selection box bounds
                }
            }
        }
    }

    //Updates the current selection
    void UpdateSelection(ObjectInfo selectedObject, bool value)
    {
        //Does the selected object's selected value equal the passed in value?
        if (selectedObject.isSelected != value)
        {
            //Is the value false?
            if (value == false)
            {
                //Does the player have a primary selected object?
                if (hasPrimary)
                {
                    //removes this object from the primary if applicable
                    selectedObject.isPrimary = value;
                    primaryObject = null;
                    hasPrimary = value;
                    AL.actions = null;
                }
            }
            else
            {
                //Does the player not have a primary?
                if (!hasPrimary)
                {
                    //Assigns this object to be the primary object
                    selectedObject.isPrimary = value;
                    primaryObject = selectedObject.gameObject;
                    hasPrimary = value;
                    selectedInfo = primaryObject.GetComponent<ObjectInfo>(); //Sets the selected info
                    AL.actions = selectedInfo.actions;
                }
            }
            selectedObject.isSelected = value; //Selects or deselects the object based on passed value
        }
    }

    //Populates the list with selected objects
    List<ObjectInfo> GetSelected()
    {
        return new List<ObjectInfo>(selectedObjects.Where(x => x.isSelected));
    }

    //Clears the selected objects
    void ClearSelected()
    {
        selectedObjects.ForEach(x => x.isSelected = false);
        selectedObjects.ForEach(x => x.isPrimary = false);
    }
}
