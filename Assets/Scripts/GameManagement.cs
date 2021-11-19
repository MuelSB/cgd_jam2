using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance;


    [SerializeField]
    private bool in_menu;
    [SerializeField]
    private bool is_paused;
    [SerializeField]
    private bool battle_scene_enabled; //Determines whether we're loading the battle scene.

    //list of prefabs


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // Awake Load Order
        
    }
    // Start is called before the first frame update
    private void Start()
    {
        // Start Load Order

        if (battle_scene_enabled)
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }
    }

    public void MainMenu()
    {
        //set to Main Menu
    }

    public void WinScene()
    {
        //set to Win SCene
    }

    public void LoseScene()
    {
        //set to Lose Scene
    }

    public void closeBattleUI()
    {
        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            SceneManager.UnloadSceneAsync(1);
        }
    }

    public void openBattleUI()
    {
        if (!SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }
    }
}
