using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSystemObject = default; 
    [SerializeField] private RectTransform parent = default;
    [SerializeField] private GameObject prefab = default;
    
    private void OnEnable()
    {
        // add callbacks
        EventSystem.Subscribe<ButtonData>(Events.AddAbility, CreateButton);
        
        // disables ui events when not the players turn
        EventSystem.Subscribe(Events.PlayerTurnStarted, () => eventSystemObject.SetActive(true));
        EventSystem.Subscribe(Events.PlayerTurnEnded, () => eventSystemObject.SetActive(false));
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe<ButtonData>(Events.AddAbility, CreateButton);
        EventSystem.Unsubscribe(Events.PlayerTurnStarted, () => eventSystemObject.SetActive(true));
        EventSystem.Unsubscribe(Events.PlayerTurnEnded, () => eventSystemObject.SetActive(false));
    }
    
    private void CreateButton(ButtonData data)
    {
        // create the instance and add to a list 
        var obj = Instantiate(prefab, parent);

        // get the component and set up the button
        var script = obj.GetComponent<DynamicButton>();
        script.SetUpButton(data);
    }
}