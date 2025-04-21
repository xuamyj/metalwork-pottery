using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WorldStatus
{
    Main,
    Gather,
    Craft,
    Sell
}

public enum ScreenStatus
{
    None,
    Quests,
    Inventory,
    Showcase,
    Journal,
    Settings,
}

public class StatusController : MonoBehaviour
{
    public WorldStatus currWorldStatus;
    public ScreenStatus currScreenStatus;

    public bool navDebugPrintOn;

    /* InputAction */
    public InputAction tapAction;

    /* DRAGGABLE */
    public Camera mainCamera;
    public List<GameObject> screens;

    public Dictionary<WorldStatus, int> worldStatusToCameraX;
    public Dictionary<ScreenStatus, GameObject> screenStatusToActualScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello there!");

        // InputAction */
        tapAction.Enable();
        tapAction.performed += OnTapPerformed;

        // WorldStatus dictionary
        worldStatusToCameraX = new Dictionary<WorldStatus, int>();
        worldStatusToCameraX[WorldStatus.Main] = 0;
        worldStatusToCameraX[WorldStatus.Gather] = -10;
        worldStatusToCameraX[WorldStatus.Craft] = 10;
        worldStatusToCameraX[WorldStatus.Sell] = 20;

        // ScreenStatus dictionary
        screenStatusToActualScreen = new Dictionary<ScreenStatus, GameObject>();
        screenStatusToActualScreen[ScreenStatus.Quests] = screens[1];
        screenStatusToActualScreen[ScreenStatus.Inventory] = screens[4];
        screenStatusToActualScreen[ScreenStatus.Showcase] = screens[2];
        screenStatusToActualScreen[ScreenStatus.Journal] = screens[0];
        screenStatusToActualScreen[ScreenStatus.Settings] = screens[3];
    }

    public void NavDebugPrint(string str)
    {
        if (navDebugPrintOn)
        {
            Debug.Log(str);
        }
    }

    public void GoToWorld(WorldStatus chosenWorld)
    {
        CloseAllScreens();

        // Set status
        currWorldStatus = chosenWorld;

        // Move camera
        mainCamera.transform.position = new Vector3(worldStatusToCameraX[chosenWorld], 0, -10);
    }

    public void GoToMainTest()
    {
        GoToWorld(WorldStatus.Main);
    }

    // --------

    public void CloseAllScreens()
    {
        NavDebugPrint("CloseAllScreens: here");

        // Set status
        currScreenStatus = ScreenStatus.None;

        // Disable screens
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

    public bool OpenScreen(ScreenStatus chosenScreen)
    {
        // Check / set status
        if (currScreenStatus != ScreenStatus.None)
        {
            return false;
        }
        currScreenStatus = chosenScreen;

        // Enable screen
        screenStatusToActualScreen[chosenScreen].SetActive(true);

        return true;
    }

    public void OpenScreenSettingsTest()
    {
        OpenScreen(ScreenStatus.Settings);
    }
    public void OpenScreenQuestsTest()
    {
        OpenScreen(ScreenStatus.Quests);
    }
    public void OpenScreenInventoryTest()
    {
        OpenScreen(ScreenStatus.Inventory);
    }

    // --------

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        // Get the tap position
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Vector3 tapPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        // Cast a ray from the camera through the tap position
        Collider2D hit = Physics2D.OverlapPoint(tapPosition);
        // Check if the ray hits something
        if (hit)
        {
            // Try to get a NavId component from the hit object
            WorldNavId worldNavIdObj = hit.GetComponent<WorldNavId>();
            ScreenNavId screenNavIdObj = hit.GetComponent<ScreenNavId>();
            if (worldNavIdObj != null)
            {
                GoToWorld(worldNavIdObj.worldNavId);
                NavDebugPrint("OnTapPerformed: go to world " + worldNavIdObj.worldNavId);
            }
            else if (screenNavIdObj != null)
            {
                OpenScreen(screenNavIdObj.screenNavId);
                NavDebugPrint("OnTapPerformed: open screen " + screenNavIdObj.screenNavId);
            }
            else
            {
                NavDebugPrint("OnTapPerformed: neither WorldNavId nor ScreenNavId");
            }
        }
        else
        {
            NavDebugPrint("OnTapPerformed: no hit");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
