using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUIManager : MonoBehaviour
{
    // Singleton instance
    public static PowerUpUIManager Instance { get; private set; }

    public GameObject powerTilePrefab;
    public Transform attackCategoryUI;
    public Transform defenseCategoryUI;
    public Transform specialCategoryUI;

    [SerializeField] private GameObject levelUpPanel; // The UI panel or elements

    void Awake()
    {
        // Ensure only one instance and prevent destruction across scenes
        if (Instance == null)
        {
            Debug.Log("DONTDESTROY: INSTANCE NULL");
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the object alive across scenes
        }
        else
        {
            Debug.Log("DESTROY: INSTANCE NOT NULL");
            Destroy(gameObject);  // Prevent duplicate instances
        }
    }

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
        //ClearUI(); // Clears previous UI elements


        Debug.Log("POWARS"+powers.Count);

        foreach (PowerData power in powers)
        {
            GameObject powerUpTile = Instantiate(powerTilePrefab);

            // Set button details
            powerUpTile.GetComponentInChildren<TMP_Text>().text = power.powerName;
            powerUpTile.GetComponentInChildren<Image>().sprite = power.icon;
            powerUpTile.GetComponentInChildren<Button>().onClick.AddListener(() => SelectPower(power));

            // Place button in the appropriate category UI
            switch (power.category)
            {
                case PowerCategory.Attack:
                    powerUpTile.transform.SetParent(attackCategoryUI, false);
                    break;
                case PowerCategory.Defense:
                    powerUpTile.transform.SetParent(defenseCategoryUI, false);
                    break;
                case PowerCategory.Special:
                    powerUpTile.transform.SetParent(specialCategoryUI, false);
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
        switch (power.category)
        {
            case PowerCategory.Attack:
                ApplyAttackPower(power);
                break;
            case PowerCategory.Defense:
                ApplyDefensePower(power);
                break;
            case PowerCategory.Special:
                ApplySpecialPower(power);
                break;
        }
    }
    private void ApplyAttackPower(PowerData power)
    {
        // Example: Add to player's damage multiplier
        //Player.Instance.Stats.weaponStats.attackMultiplier += power.upgrades[0].effectStrength;
    }

    private void ApplyDefensePower(PowerData power)
    {
        // Example: Add to player's health
        //Player.Instance.healthStats.additionalHealth += (int)power.upgrades[0].effectStrength;
    }

    private void ApplySpecialPower(PowerData power)
    {
        if (power.powerName == "Teleport")
        {
            // Enable teleport ability
            //Player.Instance.EnableTeleport();
        }
        else if (power.powerName.Contains("Teleport Upgrade"))
        {
            // Upgrade teleport (e.g., movement boost)
            //Player.Instance.UpgradeTeleport(power.upgrades[0].effectStrength);
        }
    }
}