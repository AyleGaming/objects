using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PowerLibrary : MonoBehaviour
{
    public List<PowerData> allPowers;
    private List<PowerData> availablePowers; // Powers still available for selection

    private void Awake()
    {
        // Initialize the available powers list
        availablePowers = new List<PowerData>(allPowers);
    }

    public PowerData GetRandomPower(PowerCategory category, int currentLevel, PowerRarity powerRarity)
    {
        // Filter powers by category (attack/defense/special &&
        // Filter powers by unlock level (1+) &&
        // Filter powers by rarity (common/rare/legendary)
        var filteredPowers = availablePowers.Where(power => power.category == category && power.unlockLevel <= currentLevel && power.powerRarity == powerRarity).ToList();

        if (filteredPowers.Count == 0)
        {
            Debug.LogWarning($"No available powers in category {category}!");
            return null;
        }

        // Select a random power
        PowerData selectedPower = filteredPowers[Random.Range(0, filteredPowers.Count)];

        return selectedPower;
    }

    // Remove it from the available pool if it's not reusable
    public void RemovePower(PowerData powerToRemove)
    {
        availablePowers.Remove(powerToRemove);
    }

}
