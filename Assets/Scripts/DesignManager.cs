using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DesignManager : MonoBehaviour
{
    public static string currentDesign = "Standard";

    [Header("Designs")]
    public List<string> designs;
    public List<int> costs;
    public Transform designsHolder;
    public Image designSelection;
    public GameObject designCard;

    [Header("Balls")]
    public Ball leftBall;
    public Ball rightBall;

    [Header("Unlockable")]
    public GameObject designUnlockCard;
    public Transform playingUI;

    private Dictionary<string, GameObject> designCards;
    private Dictionary<string, int> designCost;

    private void Awake()
    {
        designCards = new Dictionary<string, GameObject>();
        designCost = new Dictionary<string, int>();

        bool upgraded = PlayerPrefs.GetInt("upgrade") == 1;
        int highscore = PlayerPrefs.GetInt("highscore");
        for (int i = 0; i < designs.Count; i++)
        {
            string design = designs[i];
            designCost[design] = costs[i];

            if (upgraded && PlayerPrefs.GetInt(design) == 0) PlayerPrefs.SetInt(design, 1);

            GameObject designObj = Instantiate(designCard, Vector3.zero, Quaternion.identity, designsHolder);
            designCards.Add(design, designObj);
            designObj.GetComponent<Button>().onClick.AddListener(() => { if (currentDesign != design) ChooseDesign(design); });
            designObj.AddComponent<Tags>().colorTag = 4;
            designObj.transform.Find("Design").GetComponent<Image>().sprite = Resources.Load<Sprite>("Designs/UI/" + design);

            int cost = costs[i];
            designObj.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = "" + cost;
            if (currentDesign == design) designObj.transform.Find("Check").gameObject.SetActive(true);
            if (highscore >= cost)
            {
                designObj.transform.Find("Lock").gameObject.SetActive(false);
                designObj.transform.Find("Price").gameObject.SetActive(false);
            }
        }

        if (!PlayerPrefs.HasKey("design")) PlayerPrefs.SetString("design", designs[0]);
        ChooseDesign(PlayerPrefs.GetString("design"));
    }

    public void ChooseDesign(string design)
    {
        print("Choosing design: " + design);
        if (PlayerPrefs.GetInt("highscore") < designCost[design])
        {
            if (GameStateManager.vibrating) Vibration.VibrateNope();
            return;
        }

        if (designCards != null)
        {
            designCards[currentDesign].transform.Find("Check").gameObject.SetActive(false);
            designCards[design].transform.Find("Check").gameObject.SetActive(true);
        }
        currentDesign = design;

        designSelection.sprite = Resources.Load<Sprite>("Designs/UI/" + design);
        Sprite ballSprite = Resources.Load<Sprite>("Designs/Ball/" + design);
        leftBall.SetSprite(ballSprite);
        rightBall.SetSprite(ballSprite);
        Sprite backgroundSprite = ballSprite;
        if (design.Contains("Cutout") || design == "Standard")
        {
            backgroundSprite = Resources.Load<Sprite>("Designs/Ball/Standard");
            leftBall.EnableTrail();
            rightBall.EnableTrail();
            leftBall.DisableRotation();
            rightBall.DisableRotation();
        }
        else
        {
            leftBall.DisableTrail();
            rightBall.DisableTrail();
            leftBall.EnableRotation();
            rightBall.EnableRotation();
        }
        leftBall.SetBackgroundSprite(backgroundSprite);
        rightBall.SetBackgroundSprite(backgroundSprite);

        PlayerPrefs.SetString("design", design);
        PlayerPrefs.Save();
    }

    public void UnlockDesign(string design, bool showPopup = true)
    {
        GameObject designCard = designCards[design];
        designCard.transform.Find("Lock").gameObject.SetActive(false);
        designCard.transform.Find("Price").gameObject.SetActive(false);
        PlayerPrefs.SetInt(design, 1);
        if (showPopup)
        {
            Transform designUnlock = Instantiate(designUnlockCard, playingUI).transform;
            designUnlock.Find("Design Unlocked").gameObject.GetComponent<TextMeshProUGUI>().color = ThemeManager.colors[0];
            Sprite designSprite = Resources.Load<Sprite>("Designs/UI/" + design);
            Image designImg = designUnlock.Find("Design").gameObject.GetComponent<Image>();
            designImg.sprite = designSprite;
            designImg.color = ThemeManager.colors[0];
        }
    }

    public void UnlockAllDesigns()
    {
        foreach (string design in designs) UnlockDesign(design, false);
    }

    public int GetCost(string design)
    {
        return designCost[design];
    }

    public Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Texture2D spriteTexture = LoadTexture(filePath);
        Sprite newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit, 0, spriteType);
        return newSprite;
    }

    public Texture2D LoadTexture(string filePath)
    {
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails
        Texture2D tex2D;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (tex2D.LoadImage(fileData))           // Load the imagedata into the texture (size is set automatically)
                return tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
