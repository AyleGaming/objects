using System.Collections.Generic;
using UnityEngine;

public class PowerUpUIManager : MonoBehaviour
{
    // Singleton instance
    public static PowerUpUIManager Instance;

    public GameObject powerTilePrefab;
    public Transform attackCategoryUI;
    public Transform defenseCategoryUI;
    public Transform specialCategoryUI;

    [SerializeField] private GameObject levelUpPanel; // The UI panel or elements
    [SerializeField] private PowerLibrary powerLibrary;
    [SerializeField] private GameObject BlinkUI;
    [SerializeField] private GameObject UltUI;


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
                tile.SetAdditionalInfo($"{power.powerType} ({power.powerRarity})");
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
        if (selectedPower.powerReusable == false) {
            powerLibrary.RemovePower(selectedPower);
        }
        CloseLevelUpUI();
    }
 
    private void CloseLevelUpUI()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    private void ApplyPower(PowerData power)
    {

        Debug.Log($"APPLY POWER: {power.abilityToModify}");

        switch (power.abilityToModify)  
        {
            case AbilitiesModifiable.health:
                break;
            /*** 
            * Additive Abilities
            */
            case AbilitiesModifiable.additiveWeaponDamage:
                float baseValueAdditiveWeaponDamage = Player.Instance.Stats.GetStat(power.abilityToModify.ToString());
                float newValueAdditiveWeaponDamage = baseValueAdditiveWeaponDamage + power.powerValue;
                Player.Instance.Stats.SetStat(power.abilityToModify.ToString(), newValueAdditiveWeaponDamage);
                break;
            case AbilitiesModifiable.gunsBase:
                int baseValueInt = Player.Instance.Stats.GetStatInt(power.abilityToModify.ToString());
                int newValueInt = baseValueInt + (int)power.powerValue;
                Player.Instance.Stats.SetStat(power.abilityToModify.ToString(), newValueInt);
                break;
            case AbilitiesModifiable.additionalGuns:
                break;
            /*** 
             * Multiplicative Abilities
             */
            case AbilitiesModifiable.damageReduction:
            case AbilitiesModifiable.movementSpeed:
            case AbilitiesModifiable.attackSpeed:
                float baseValue = Player.Instance.Stats.GetStat(power.abilityToModify.ToString());
                float newValue = baseValue * power.powerValue;
                Player.Instance.Stats.SetStat(power.abilityToModify.ToString(), newValue);
                break;
            /*** 
             * Boolean Abilities
             */
           
            case AbilitiesModifiable.hasBlinkAbility:
                BlinkUI.SetActive(true);
                bool baseBlinkValue = Player.Instance.Stats.GetStatBool(power.abilityToModify.ToString());
                bool newBlinkValue = !baseBlinkValue;
                Player.Instance.Stats.SetStatBool(power.abilityToModify.ToString(), newBlinkValue);
                break;
            case AbilitiesModifiable.hasUltimateAbility:
                UltUI.SetActive(true);
                bool baseUltValue = Player.Instance.Stats.GetStatBool(power.abilityToModify.ToString());
                bool newUltValue = !baseUltValue;
                Player.Instance.Stats.SetStatBool(power.abilityToModify.ToString(), newUltValue);
                break;
            case AbilitiesModifiable.hasShieldAbility:
                bool baseBoolValue = Player.Instance.Stats.GetStatBool(power.abilityToModify.ToString());
                bool newBoolValue = !baseBoolValue;
                Player.Instance.Stats.SetStatBool(power.abilityToModify.ToString(), newBoolValue);
                break;
            default:
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