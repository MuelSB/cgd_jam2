using Core;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject uiAbilityPrefab = null;
        

        // cash the player though an event
        private Player _player = null;
        private void SetPlayerRef(Player player) => _player = player;

        private void OnEnable()
        {
            // create generic UI
        
            ValidatePrefab(uiAbilityPrefab);

            // create buttons for each ability
            foreach (var a in _player.abilities) CreateAbilityUI(a); 
            
            // add subscriptions
            EventSystem.Subscribe<Player>(Events.BroadcastPlayer, SetPlayerRef);
        }
    
        private void OnDisable()
        {
            EventSystem.Unsubscribe<Player>("BroadcastPlayer", SetPlayerRef);
        }
        
        private bool ValidatePrefab(GameObject prefab)
        {
            if (uiAbilityPrefab != null) return true;
            Debug.LogWarning("Prefab not added in editor", this);
            return false;
        }
        
        private void CreateAbilityUI(Ability ability)
        {
            var obj = Instantiate(uiAbilityPrefab, transform);
            var UIButton = obj.GetComponent<UIAbilityButton>();
            UIButton.SetImage(ability.image);
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
}
