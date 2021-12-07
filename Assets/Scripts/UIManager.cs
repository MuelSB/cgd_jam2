using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSystemObject = default; 
    [SerializeField] private RectTransform parent = default;
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private RectTransform apParent = default;
    [SerializeField] private GameObject apImagePrefab = default;
    
    private void OnEnable()
    {
        // add callbacks
        EventSystem.Subscribe<ButtonData>(Events.AddAbility, CreateButton);
        EventSystem.Subscribe<UIData>(Events.AddAP, CreateUI);
        EventSystem.Subscribe<APpackage>(Events.ReduceAP, HideAP);
        EventSystem.Subscribe(Events.ResetAP, resetAP);
        // disables ui events when not the players turn
        EventSystem.Subscribe(Events.PlayerTurnStarted, () => eventSystemObject.SetActive(true));
        EventSystem.Subscribe(Events.PlayerTurnEnded, () => eventSystemObject.SetActive(false));
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe<ButtonData>(Events.AddAbility, CreateButton);
        EventSystem.Unsubscribe<UIData>(Events.AddAP, CreateUI);
        EventSystem.Unsubscribe<APpackage>(Events.ReduceAP, HideAP);
        EventSystem.Unsubscribe(Events.ResetAP, resetAP);
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

    private void CreateUI(UIData data)
    {
        var obj = Instantiate(apImagePrefab, apParent);
        var script = obj.GetComponent<DynamicImage>();
        script.SetUpImage(data);
    }

    private void HideAP(APpackage aPackage)
    {
        for ( int i = 0; i < apParent.transform.childCount; i ++)
        {
            apParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        var APLeft = aPackage.CurrentAP - aPackage.APToReduce;
        for (int i = 0; i < APLeft+1; i++)
        {
            apParent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void resetAP()
    {
        for (int i = 0; i < apParent.transform.childCount; i++)
        {
            apParent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}