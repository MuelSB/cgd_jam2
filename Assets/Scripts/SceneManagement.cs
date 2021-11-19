using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;


    [SerializeField]
    private bool in_menu;
    [SerializeField]
    private bool is_paused;

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


        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
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
}
