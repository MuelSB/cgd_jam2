using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Core
{
    public static class Scenes
    {
        public const string SystemCore = "SystemCore";
        public const string GameScene  = "GameScene";
        public const string MenuScene  = "MenuScene";
        public const string BossScene = "BossScene";
        public const string LoseScene = "LoseScene";
        public const string WinScene = "WinScene";
    }

    public enum GameState
    {
        None = 0, Loading, InMenu, InGame, InBossSelect, InLose, InWin
    }
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameState _state = GameState.None;
        //[SerializeField] private bool _paused;

        private const LoadSceneMode Add = LoadSceneMode.Additive;
        
        private void OnEnable()
        {
            // Make sure that System Core is always the active scene
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Scenes.SystemCore))
            {
                Debug.LogError("SystemCore is not the active scene");
            }
            
            // Register callbacks
            EventSystem.Subscribe(Events.LoadMenu,  OnLoadMenu);
            EventSystem.Subscribe(Events.LoadGame,  OnLoadGame);
            EventSystem.Subscribe(Events.LoadLose, OnLoadLose);
            EventSystem.Subscribe(Events.LoadWin, OnLoadWin);
            EventSystem.Subscribe(Events.LoadBoss, OnLoadBoss);

            // Load the menu
            EventSystem.Invoke(Events.LoadMenu);
        }

        private void OnDisable()
        {
            EventSystem.Unsubscribe(Events.LoadMenu,  OnLoadMenu);
            EventSystem.Unsubscribe(Events.LoadGame,  OnLoadGame);
            EventSystem.Unsubscribe(Events.LoadLose, OnLoadLose);
            EventSystem.Unsubscribe(Events.LoadWin, OnLoadWin);
            EventSystem.Unsubscribe(Events.LoadBoss, OnLoadBoss);
        }

        private void OnLoadMenu() => LoadScene(Scenes.MenuScene, GameState.InMenu);
        private void OnLoadGame() => LoadScene(Scenes.GameScene, GameState.InGame);
        private void OnLoadLose() => LoadScene(Scenes.LoseScene, GameState.InLose);
        private void OnLoadWin() => LoadScene(Scenes.WinScene, GameState.InWin);
        private void OnLoadBoss() => LoadScene(Scenes.BossScene, GameState.InBossSelect);

        private async void LoadScene(string str, GameState state)
        {
            Debug.Log("LOADING SCENE");

            // start the fade and wait for 1 second
            EventSystem.Invoke(Events.LoadTransition);
            // without making the event system more complex or introducing 
            // ..a global variable the transition time needs to be hard coded
            await Task.Delay(1000); 
            
            // unload current scene
            if (_state != GameState.None)
            {
                switch(_state)
                {
                    case GameState.InGame: SceneManager.UnloadSceneAsync(Scenes.GameScene); break;
                    case GameState.InLose: SceneManager.UnloadSceneAsync(Scenes.LoseScene); break;
                    case GameState.InMenu: SceneManager.UnloadSceneAsync(Scenes.MenuScene); break;
                    case GameState.InWin: SceneManager.UnloadSceneAsync(Scenes.WinScene); break;
                    case GameState.InBossSelect: SceneManager.UnloadSceneAsync(Scenes.BossScene); break;
                    default: Debug.LogError("Unloading undefined game state."); break;
                }
            }
                
            // load scene async
            _state = GameState.Loading;
            var scene = SceneManager.LoadSceneAsync(str, Add);
            
            // stop scene from automatically loading
            scene.allowSceneActivation = false;
            
            // wait until load is finished 
            while (scene.progress < 0.9f) { /*this could be used for a loading bar*/ }
            Debug.Log("Exiting transition");
            // exit transition FX
            _state = state;
            scene.allowSceneActivation = true;
            EventSystem.Invoke(Events.LoadTransition);
            EventSystem.Invoke(Events.LoadComplete);
        }
    }
}
