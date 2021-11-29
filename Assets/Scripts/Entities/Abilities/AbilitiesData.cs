using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreateAbilitiesData", order = 1)]
public class AbilitiesData : ScriptableObject
{
    public static AbilitiesData Instance;

    [System.Serializable]
    public struct AbilityData
    {
        public Sprite abilityImage;
        public string abilityName;
        public int abilityRange;
        public Ability.AbilityTarget targetType;
        public int abilityCooldown;
        public bool freeUse;
        public List<AbilityEffect.AbilityEffectData> abilityEffects;
    }

    public List<AbilityData> abilities;

    private void OnEnable()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Abilities data object already exists! Only one instance of this data file should exist.");
            DestroyImmediate(this);
        }
    }
}
