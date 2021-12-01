using Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<MapCoordinate> onSelected;
    
    private PlayerInputActions _input = default;
    private Camera _camera = default;
    private const int TileLayerMask = 1 << 6;

    private void OnEnable()
    {
        //cash the cam
        _camera = Camera.main;
        
        // enable input actions
        _input = new PlayerInputActions();
        _input.Player.Enable();

        // activates and deactivates on player turn
        EventSystem.Subscribe(Events.PlayerTurnStarted, () => _input.Player.Select.performed += OnSelect);
        EventSystem.Subscribe(Events.PlayerTurnEnded, () => _input.Player.Select.performed -= OnSelect);
    }

    private void OnDisable()
    {
        // clean up your events
        EventSystem.Unsubscribe(Events.PlayerTurnStarted, () => _input.Player.Select.performed += OnSelect);
        EventSystem.Unsubscribe(Events.PlayerTurnEnded, () => _input.Player.Select.performed -= OnSelect);
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (TryGetMapCoord(out var coord))
        {
            onSelected?.Invoke(coord);
        }
    }

    private bool TryGetMapCoord(out MapCoordinate coord)
    {
        // Raycast from the cursor world position down into the scene
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // cast the ray
        if (Physics.Raycast(ray, out var hit, 999))
        {
            // Get the hit tile's map coordinate
            if (hit.collider.TryGetComponent<MapTile>(out var mapTile))
            {
                coord = mapTile.getLocation();
                return true;
            }
        }
        
        coord = null;
        return false;
    }


    
}