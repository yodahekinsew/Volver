using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeManager : MonoBehaviour
{

    public static string currentTheme = "Popsicle";
    public static List<Color> colors = new List<Color>();

    [Header("Themes")]
    public List<string> themes;
    public List<int> costs;
    public Transform themeHolder;
    public GameObject themePalette;
    public Image themeSelection;

    [Header("Game Components")]
    public Player player;
    public WallSpawner wallSpawner;
    public UIManager ui;

    [Header("Font Materials")]
    public Material fontBackground;
    public Material fontForeground;

    [Header("Unlockable")]
    public GameObject themeUnlockCard;
    public Transform playingUI;

    private Dictionary<string, GameObject> palettes;
    private Dictionary<string, int> themeCost;

    private void Awake()
    {
        palettes = new Dictionary<string, GameObject>();
        themeCost = new Dictionary<string, int>();

        bool upgraded = PlayerPrefs.GetInt("upgrade") == 1;
        int highscore = PlayerPrefs.GetInt("highscore");
        for (int i = 0; i < themes.Count; i++)
        {
            // Construct the theme to cost mapping
            string theme = themes[i];
            themeCost[theme] = costs[i];

            if (upgraded && PlayerPrefs.GetInt(theme) == 0) PlayerPrefs.SetInt(theme, 1);

            GameObject themeObj = Instantiate(themePalette, Vector3.zero, Quaternion.identity, themeHolder);
            palettes.Add(theme, themeObj);
            themeObj.GetComponent<Button>().onClick.AddListener(() => { if (currentTheme != theme) ChooseTheme(theme); });
            themeObj.AddComponent<Tags>().colorTag = 4;
            themeObj.transform.Find("Palette").GetComponent<Image>().sprite = Resources.Load<Sprite>("Themes/" + theme + "/" + theme);

            int cost = costs[i];
            themeObj.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = "" + cost;
            if (currentTheme == theme) themeObj.transform.Find("Check").gameObject.SetActive(true);
            if (highscore >= cost)
            {
                themeObj.transform.Find("Lock").gameObject.SetActive(false);
                themeObj.transform.Find("Price").gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("theme")) PlayerPrefs.SetString("theme", themes[0]);
        ChooseTheme(PlayerPrefs.GetString("theme"), false);
    }

    public void ChooseTheme(string theme, bool updateIcon = true)
    {
        print("Choosing theme: " + theme);
        if (PlayerPrefs.GetInt("highscore") < themeCost[theme])
        {
            if (GameStateManager.vibrating) Vibration.VibrateNope();
            return;
        }

        if (palettes != null)
        {
            palettes[currentTheme].transform.Find("Check").gameObject.SetActive(false);
            palettes[theme].transform.Find("Check").gameObject.SetActive(true);
        }
        currentTheme = theme;

        ui.UpdateTitle(Resources.Load<Sprite>("Themes/" + theme + "/Title"));
        var colorsText = Resources.Load<TextAsset>("Themes/" + theme + "/colors");
        themeSelection.sprite = Resources.Load<Sprite>("Themes/" + theme + "/" + theme);

        string[] colorLines = colorsText.text.Split('\n');
        colors = new List<Color>();
        foreach (string line in colorLines)
        {
            if (line == "") break;
            float r = int.Parse("" + line[1] + line[2], System.Globalization.NumberStyles.HexNumber) / 255f;
            float g = int.Parse("" + line[3] + line[4], System.Globalization.NumberStyles.HexNumber) / 255f;
            float b = int.Parse("" + line[5] + line[6], System.Globalization.NumberStyles.HexNumber) / 255f;
            colors.Add(new Color(r, g, b));
        }
        player.UpdateColors(colors[0], colors[1]);
        ui.UpdateColors();
        wallSpawner.UpdateColors();
        Camera.main.backgroundColor = new Color(colors[2].r + 1f / 255f, colors[2].g, colors[2].b);

        // Update font materials
        fontBackground.SetColor("_FaceColor", colors[2]);
        fontBackground.SetColor("_UnderlayColor", colors[0]);
        // player.UpdateColors();

        // Update app icon
        if (updateIcon) ChangeAppIcon.ChangeIcon(theme);

        PlayerPrefs.SetString("theme", theme);
        PlayerPrefs.Save();
    }

    public void UnlockTheme(string theme, bool showPopup = true)
    {
        GameObject themePalette = palettes[theme];
        themePalette.transform.Find("Lock").gameObject.SetActive(false);
        themePalette.transform.Find("Price").gameObject.SetActive(false);
        PlayerPrefs.SetInt(theme, 1);
        if (showPopup)
        {
            Transform themeUnlock = Instantiate(themeUnlockCard, playingUI).transform;
            Sprite palette = Resources.Load<Sprite>("Themes/" + theme + "/" + theme);
            themeUnlock.Find("Palette").gameObject.GetComponent<Image>().sprite = palette;
            themeUnlock.Find("Theme Unlocked").gameObject.GetComponent<TextMeshProUGUI>().color = colors[0];
        }
    }

    public void UnlockAllThemes()
    {
        foreach (string theme in themes) UnlockTheme(theme, false);
    }

    public int GetCost(string theme)
    {
        return themeCost[theme];
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
