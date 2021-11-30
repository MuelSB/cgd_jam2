using UnityEngine;
using Core;

public class LoadMenuScene : MonoBehaviour
{
    public void LoadScene()
    {
        EventSystem.Invoke(Events.LoadMenu);
    }
}
