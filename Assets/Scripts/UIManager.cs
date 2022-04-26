using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Starting UI")]
    public GameObject gameTitle;

    public void UpdateTitle(Sprite newSprite)
    {
        gameTitle.GetComponent<Image>().sprite = newSprite;
    }

    public void UpdateColors()
    {
        // Update the color of all ui text fields
        Component[] textFields = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI textField in textFields)
        {
            textField.color = ThemeManager.colors[textField.gameObject.GetComponent<Tags>().colorTag];
        }

        // Update the color of all ui images
        Component[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            image.color = ThemeManager.colors[image.gameObject.GetComponent<Tags>().colorTag];
        }
    }
}
