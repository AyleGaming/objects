using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using TMPro;

public class PowerUpUIManager : MonoBehaviour
{
    // Singleton instance
    public static PowerUpUIManager Instance;

    public GameObject powerTilePrefab;
    public Transform attackCategoryUI;
    public Transform defenseCategoryUI;
    public Transform specialCategoryUI;

    [SerializeField] private GameObject levelUpPanel; // The UI panel or elements


    void OnDestroy()
    {
        Debug.Log("LevelUpUIManager destroyed!");
    }

    public void Show()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void PopulatePowerUpUI(List<PowerData> powers)
    {
        ClearUI(); // Clears previous UI elements

        foreach (PowerData power in powers)
        {
            GameObject powerUpTile = Instantiate(powerTilePrefab);
            PowerUpTile tile = powerUpTile.GetComponent<PowerUpTile>();

            if (tile != null)
            {
                tile.SetTitle(power.powerName);
                tile.SetDescription(power.description);
                if (power.hasCoolDown)
                {
                    tile.SetCoolDownText(power.coolDown);
                }
                tile.SetIconValue(power.icon);
                tile.SetButtonAction(() => SelectPower(power)); // Set the button onClick
                tile.SetButtonText(power.powerName);
            }

            // Place button in the appropriate category UI
            switch (power.category)
            {
                case PowerCategory.Attack:
                    tile.transform.SetParent(attackCategoryUI, false);
                    break;
                case PowerCategory.Defense:
                    tile.transform.SetParent(defenseCategoryUI, false);
                    break;
                case PowerCategory.Special:
                    tile.transform.SetParent(specialCategoryUI, false);
                    break;
            }
        }
    }

    public void ClearUI()
    {
        foreach (Transform child in attackCategoryUI) Destroy(child.gameObject);
        foreach (Transform child in defenseCategoryUI) Destroy(child.gameObject);
        foreach (Transform child in specialCategoryUI) Destroy(child.gameObject);
    }

    public void SelectPower(PowerData selectedPower)
    {
        Debug.Log($"Selected Power: {selectedPower.powerName}");
        ApplyPower(selectedPower);
        CloseLevelUpUI();
    }
 
    private void CloseLevelUpUI()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    private void ApplyPower(PowerData power)
    {

        Debug.Log($"APPLY POW: {power.powerType}");


        switch (power.powerType)
        {
            case PowerType.ability:
                // Set ability to active
                //Player.Instance.Stats.SetStat(power.abilityToModify.ToString(), true);

                // Check of secondary ability to update
                //if (power.secondaryAbilityToModify != AbilitiesModifiableSecondary.none)
                // {
                // Set secondary update
                //    ApplySecondaryPower(power);
                //}
                //break;
                break;
            case PowerType.buff:

                float baseAbilityValue = Player.Instance.Stats.GetStat(power.abilityToModify.ToString());

                Debug.Log($"BASE ABILITY: {power.abilityToModify}, VALUE: {baseAbilityValue}");

                float newAbilityValue = baseAbilityValue * power.powerValue;

                Player.Instance.Stats.SetStat(power.abilityToModify.ToString(), newAbilityValue);

                Debug.Log($"NEW VALUE: {Player.Instance.Stats.GetStat(power.abilityToModify.ToString())}");

                break;

        }
    }

    private void ApplySecondaryPower(PowerData power)
    {
        switch (power.secondaryAbilityToModify)
        {
            case AbilitiesModifiableSecondary.none:
                break;
            case AbilitiesModifiableSecondary.ultimateCooldown:
            case AbilitiesModifiableSecondary.blinkCooldown:
            case AbilitiesModifiableSecondary.shield:
//                Player.Instance.Stats.SetFloatAbilityValue(power.secondaryAbilityToModify.ToString(), power.coolDown);
                break;
            default:
                break;
        }
    }

}