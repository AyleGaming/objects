using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PowerLibrary : MonoBehaviour
{
    public List<PowerData> allPowers;
    public List<PowerData> GetAvailablePowers(PowerCategory category, int currentLevel)
    {
        return allPowers
            .Where(power => power.category == category && power.unlockLevel <= currentLevel)
            .ToList();
    }
}
