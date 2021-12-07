using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Highlights : MonoBehaviour
{
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private LayerMask _layerMask;

    
    private List<GameObject> _highlights = new List<GameObject>();
    private readonly int _max = 100;

    private GameObject _highlighted = null; 
    private Coroutine _highlightCoroutine = null;

    private Camera _camera = default;
    
    private void OnEnable()
    {
        _camera = Camera.main;
        
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
                _highlights.Add(obj);
                obj.SetActive(false);
            }
        }

        // move highlight to the correct position and show
        var index = 0;
        foreach (var coord in coords)
        {
            // get the tings
            var highlight = _highlights[index];
            
            // set the position and enable
            var newPos = MapManager.GetMap().GetTile(coord).transform.position + (Vector3.up * 1.1f);
            var newRot = MapManager.GetMap().GetTile(coord).transform.rotation;
            highlight.transform.SetPositionAndRotation(newPos, newRot);
            SetColour(highlight, coord);
            highlight.SetActive(true);

            index++;
        }
        
        // start hover coroutine
        if (_highlightCoroutine != null) StopCoroutine(_highlightCoroutine);
        _highlightCoroutine = StartCoroutine(HoverBehaviour(coords));
    }

    private void SetColour(GameObject highlight, MapCoordinate coord)
    {
        var entity = MapManager.GetMap().GetTileProperties(coord).tile_enitity;
        var ren = highlight.GetComponent<Renderer>();

        ren.material.color = entity.is_some ? Color.red : Color.green;
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
        while (true)
        {
            CheckForTile();
        
            yield return null;
        }
    }

    private void CheckForTile()
    {
        // Raycast from the cursor world position down into the scene
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // cast the ray
        if (Physics.Raycast(ray, out var hit, 999, _layerMask))
        {
            _highlighted = hit.collider.gameObject;
        }
        
        
    }
}
