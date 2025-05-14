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
    public ScoreController scoreController;
    public GameObject startButton; // plays the gather minigame
    public GameObject finishButton; // returns to the WorldStatus.Main screen
    public GameObject expGenButton;
    public GameObject clayGenButton;
    public GameObject goldGenButton;

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

    public void GatherMinigameOnHit(Collider2D hit)
    {
        TempGatherMinigameHelper(hit);
        // if (hit.GetComponent<>()) { // later it should be something like this

        // } else {

        // }
    }

    // --------

    private void TempGatherMinigameHelper(Collider2D hit)
    {
        if (hit.gameObject.name == "StartButton")
        { // use names because these buttons will go away, replaced by actual gameplay
            StartGatherMinigame();
        }
        else if (hit.gameObject.name == "FinishButton")
        {
            FinishGatherMinigame();
        }
        else if (hit.gameObject.name == "ExpGen")
        {
            genRandomExp();
        }
        else if (hit.gameObject.name == "ClayGen")
        {
            genRandomClay();
        }
        else if (hit.gameObject.name == "GoldGen")
        {
            genRandomGold();
        }
        else
        {
            Debug.Log("TempGatherMinigameHelper: Didn't recognize obj " + hit);
        }
    }

    public void genRandomExp()
    {
        int expToAdd = UnityEngine.Random.Range(20, 120);
        Debug.Log("Adding random exp: " + expToAdd);
        scoreController.AddExp(expToAdd);
    }

    public void genRandomClay()
    {
        int terracottaToAdd = UnityEngine.Random.Range(1, 20);
        int stonewareToAdd = UnityEngine.Random.Range(1, 20);
        int kaolinToAdd = UnityEngine.Random.Range(1, 10);
        Debug.Log("Adding random terracotta: " + terracottaToAdd + ", stoneware: " + stonewareToAdd + ", kaolin: " + kaolinToAdd);
        scoreController.AddClay(terracottaToAdd, stonewareToAdd, kaolinToAdd);
    }

    public void genRandomGold()
    {
        int goldToAdd = UnityEngine.Random.Range(20, 40);
        Debug.Log("Adding random gold: " + goldToAdd);
        scoreController.AddGold(goldToAdd);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
