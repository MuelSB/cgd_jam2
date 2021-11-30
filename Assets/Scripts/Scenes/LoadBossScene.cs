using UnityEngine;
using Core;

public class LoadBossScene : MonoBehaviour
{
    public void LoadScene()
    {
        EventSystem.Invoke(Events.LoadBoss);
    }
}
