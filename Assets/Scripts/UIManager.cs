using System;
using Core;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiButtonPrefab = null;

    private const string PickUpSpell = "PickUpSpell";
    private const string PickUpGold = "PickUpGold";

    // cash the player though an event
    private Player _player = null;
    private void SetPlayerRef(Player player) => _player = player;

    private void OnEnable()
    {
        // create generic UI
        
        ValidatePrefab(uiButtonPrefab);

        // create buttons for each ability
        //CreateStartingAbilityUI();
        
        // add subscriptions
        EventSystem.Subscribe<Player>(Events.BroadcastPlayer, SetPlayerRef);
        EventSystem.Subscribe(PickUpSpell, OnPickUpSpell);
        EventSystem.Subscribe(PickUpGold, OnPickUpGold);
    }
    
    private void OnDisable()
    {
        EventSystem.Unsubscribe<Player>("BroadcastPlayer", SetPlayerRef);
        EventSystem.Unsubscribe(PickUpSpell, OnPickUpSpell);
        EventSystem.Unsubscribe(PickUpGold, OnPickUpGold);
    }



    [ContextMenu("Is Player Val")]
    private void IsPlayerValid()
    {
        Debug.Log(_player);
    }
    
    private bool ValidatePrefab(GameObject prefab)
    {
        if (uiButtonPrefab != null) return true;
        Debug.LogWarning("Prefab not added in editor", this);
        return false;
    }

    private void CreateStartingAbilityUI()
    {
        foreach (var a in _player.abilities)
        {
            CreateAbilityUI(a);
        }
    }

    private void CreateAbilityUI(Ability ability)
    {
        
    }

    private bool GetPlayerReference()
    {
        // calls an event registered in the player that sets the refrnece
        GameObject obj = null;
        EventSystem.Invoke<GameObject>("GetPlayer", obj);
        _player = obj.GetComponent<Player>();
        print(_player);
        
        // if null then the player does not exist or is not subscribed
        if (_player != null) return true;
        Debug.LogWarning("Null Player Ref : Script Halted", this);
        return false;
    }
    
    private void OnUIButtonPress(Ability ability)
    {
        
    }
    
    private void OnPickUpSpell()
    {
        print("YEEEEE");   
    }

    private void OnPickUpGold()
    {
        
    }
    
}
