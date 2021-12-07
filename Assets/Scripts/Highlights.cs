using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.VisualScripting;
using UnityEngine;

public class Highlights : MonoBehaviour
{
    [SerializeField] private GameObject prefab = default;

    private List<GameObject> _highlights = new List<GameObject>();
    private readonly int _max = 100;

    private Coroutine _highlightCoroutine = null;
    
    private void OnEnable()
    {
        EventSystem.Subscribe<List<MapCoordinate>>(Events.AbilitySelected, OnAbilitySelected );
        EventSystem.Subscribe(Events.AbilityDeselected, OnAbilityDeselected );
    }
    
    private void OnDisable()
    {
        EventSystem.Unsubscribe<List<MapCoordinate>>(Events.AbilitySelected, OnAbilitySelected );
        EventSystem.Unsubscribe(Events.AbilityDeselected, OnAbilityDeselected );
    }

    private void OnAbilitySelected(List<MapCoordinate> coords)
    {
        // check number is lower than the max
        if (coords.Count > _max)
        {
            var message = "You are passing in more positions than the current max: " + _max;
            Debug.LogWarning(message, this);
        }

        // create more if not enough in list 
        if (coords.Count > _highlights.Count)
        {
            var difference = coords.Count - _highlights.Count;

            for (var i = 0; i < difference; i++)
            {
                var obj = Instantiate(prefab, transform);
                obj.SetActive(false);
            }
        }

        // move highlight sto the correct position and show
        var index = 0;
        foreach (var pos in coords)
        {
            // get the tings
            var highlight = _highlights[index++];
            
            // set the position and enable
            highlight.transform.position = new Vector3(pos.x, pos.y);
            highlight.SetActive(true);
        }
        
        // start hover coroutine
        if (_highlightCoroutine != null) StopCoroutine(_highlightCoroutine);
        _highlightCoroutine = StartCoroutine(HoverBehaviour(coords));
    }
    
    private void OnAbilityDeselected()
    {
        // stop the coroutine
        if (_highlightCoroutine != null) StopCoroutine(_highlightCoroutine);
        // hide all the highlights
        foreach (var h in _highlights)
        {
            h.SetActive(false);
        }
    }

    private IEnumerator HoverBehaviour(List<MapCoordinate> coords)
    {
        
        // highlight the tile the mouse is over
        
        yield return null;
    }
}
