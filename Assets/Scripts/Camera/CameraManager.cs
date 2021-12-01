using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

static public class CameraManager
{
    static public void Initialize()
    {
        EventSystem.Subscribe(Events.TurnStarted, OnTurnStarted);
        EventSystem.Subscribe(Events.PlayerTurnStarted, OnPlayerTurnStarted);

        Debug.Log("Camera manager initialized");
    }

    static private void OnTurnStarted()
    {
        Debug.Log("Turn started");
    }

    static private void OnPlayerTurnStarted()
    {
        Debug.Log("Player turn started");
    }
}
