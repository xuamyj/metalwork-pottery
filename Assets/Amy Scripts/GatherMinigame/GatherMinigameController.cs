using UnityEngine;

public enum GatherStatus
{
    NotPlay,
    PlayActive,
    PlayPaused,
}

public class GatherMinigameController : MonoBehaviour
{
    GatherStatus currGatherStatus;

    /* DRAGGABLE */
    ScoreController scoreController;
    GameObject startButton; // plays the gather minigame
    GameObject finishButton; // returns to the WorldStatus.Main screen
    GameObject expGenButton;
    GameObject clayGenButton;
    GameObject goldGenButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currGatherStatus = GatherStatus.NotPlay;
    }

    public void StartGatherMinigame() // plays the gather minigame
    {
        currGatherStatus = GatherStatus.PlayActive;

        // ???
    }

    public void FinishGatherMinigame()  // returns to the WorldStatus.Main screen
    {
        currGatherStatus = GatherStatus.NotPlay;

        // ??? 
    }

    // --------

    public void genRandomExp()
    {
        int expToAdd = UnityEngine.Random.Range(20, 120);
        Debug.Log("Adding exp: " + expToAdd);
        scoreController.AddExp(expToAdd);
    }

    public void genRandomClay()
    {
        int terracottaToAdd = UnityEngine.Random.Range(1, 20);
        int stonewareToAdd = UnityEngine.Random.Range(1, 20);
        int kaolinToAdd = UnityEngine.Random.Range(1, 10);
        Debug.Log("Adding terracotta: " + terracottaToAdd);
        Debug.Log("Adding stoneware: " + stonewareToAdd);
        Debug.Log("Adding kaolin: " + kaolinToAdd);
        scoreController.AddClay(terracottaToAdd, stonewareToAdd, kaolinToAdd);
    }

    public void genRandomGold()
    {
        int goldToAdd = UnityEngine.Random.Range(20, 40);
        Debug.Log("Adding gold: " + goldToAdd);
        scoreController.AddGold(goldToAdd);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
