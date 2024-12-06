using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpTile : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text coolDownText;
    [SerializeField] private Image  iconValue;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text buttonValue;


    // Public methods to set the text fields
    public void SetTitle(string title)
    {
        if (titleText != null)
            titleText.text = title;
    }

    public void SetDescription(string description)
    {
        if (descriptionText != null)
            descriptionText.text = description;
    }

    public void SetCoolDownText(float coolDown)
    {
        if (coolDownText != null)
            coolDownText.text = coolDown.ToString() + "s CD";
    }

    public void SetIconValue(Sprite icon)
    {
        if (iconValue != null)
            iconValue.sprite = icon;
    }

    public void SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners(); // Clear previous listeners
            actionButton.onClick.AddListener(action); // Add the new listener
        }
    }
    public void SetButtonText(string title)
    {
        if (buttonValue != null)
            buttonValue.text = title;
    }
}
