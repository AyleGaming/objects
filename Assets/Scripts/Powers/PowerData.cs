using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerData", menuName = "ScriptableObjects/PowerData")]
public class PowerData : ScriptableObject
{
    public string powerName;
    public string description;
    public Sprite icon;
    public PowerCategory category;
    public int unlockLevel;
    public bool active = false;
    public bool hasCoolDown = false;
    public float coolDown;
    public AudioClip sound;
    public GameObject useEffect;

    [System.Serializable]
    public class PowerUpgrade
    {
        public string upgradeName;
        public string upgradeDescription;
        public int requiredLevel; // Level needed to unlock this upgrade
        public float effectStrength; // Strength or multiplier of the upgrade effect
    }
}

public enum PowerCategory
{
    Attack,
    Defense,
    Special
}
