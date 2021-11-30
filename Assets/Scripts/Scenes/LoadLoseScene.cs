using UnityEngine;
using Core;

public class LoadLoseScene : MonoBehaviour
{
    public void LoadScene()
    {
        EventSystem.Invoke(Events.LoadLose);
    }
}
