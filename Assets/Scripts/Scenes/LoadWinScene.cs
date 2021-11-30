using UnityEngine;
using Core;

public class LoadWinScene : MonoBehaviour
{
    public void LoadScene()
    {
        EventSystem.Invoke(Events.LoadWin);
    }
}
