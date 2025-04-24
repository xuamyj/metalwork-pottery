using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public int lvl;
    public int exp;
    public int numTerracotta;
    public int numStoneware;
    public int numKaolin;
    public int gold;

    /* DRAGGABLE */
    public TextMeshProUGUI lvlText;
    public TextMeshProUGUI numTerracottaText;
    public TextMeshProUGUI numStonewareText;
    public TextMeshProUGUI numKaolinText;
    public TextMeshProUGUI goldText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateAllText();
    }

    public void UpdateAllText()
    {
        lvlText.text = "Lvl " + lvl;
        numTerracottaText.text = numTerracotta.ToString();
        numStonewareText.text = numStoneware.ToString();
        numKaolinText.text = numKaolin.ToString();
        goldText.text = gold.ToString();
    }

    public void AddExp(int expToAdd)
    {
        exp += expToAdd;
        while (exp > 100)
        {
            lvl += 1;
            exp -= 100;
        }
        UpdateAllText();
    }

    public void AddClay(int terracottaToAdd, int stonewareToAdd, int kaolinToAdd)
    {
        numTerracotta += terracottaToAdd;
        numStoneware += stonewareToAdd;
        numKaolin += kaolinToAdd;
        UpdateAllText();
    }

    public void AddGold(int goldToAdd)
    {
        gold += goldToAdd;
        UpdateAllText();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
