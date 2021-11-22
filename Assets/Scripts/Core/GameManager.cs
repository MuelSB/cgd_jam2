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
    }

    public enum GameState
    {
        None = 0, Loading, InMenu, InGame
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

            // Load the menu
            EventSystem.Invoke(Events.LoadMenu);
        }

        private void OnDisable()
        {
            EventSystem.Unsubscribe(Events.LoadMenu,  OnLoadMenu );
            EventSystem.Unsubscribe(Events.LoadGame,  OnLoadGame);
        }

        private void OnLoadMenu() => LoadScene(Scenes.MenuScene, GameState.InMenu);
        private void OnLoadGame() => LoadScene(Scenes.GameScene, GameState.InGame);

        private async void LoadScene(string str, GameState state)
        {
            // start the fade and wait for 1 second
            EventSystem.Invoke(Events.LoadTransition);
            // without making the event system more complex or introducing 
            // ..a global variable the transition time needs to be hard coded
            await Task.Delay(1000); 
            
            // unload current scene
            if (_state != GameState.None)
            {
                var current = _state != GameState.InGame ? Scenes.MenuScene : Scenes.GameScene;
                SceneManager.UnloadSceneAsync(current);
            }
                
            // load scene async
            _state = GameState.Loading;
            var scene = SceneManager.LoadSceneAsync(str, Add);
            
            // stop scene from automatically loading
            scene.allowSceneActivation = false;
            
            // wait until load is finished 
            while (scene.progress < 0.9f) { /*this could be used for a loading bar*/ }
            
            // exit transition FX
            _state = state;
            scene.allowSceneActivation = true;
            EventSystem.Invoke(Events.LoadTransition);
            EventSystem.Invoke(Events.LoadComplete);
        }
        
        /*//Determines whether we're loading the battle scene.
        [SerializeField] private bool battle_scene_enabled; 
        
        // Start is called before the first frame update
        private void Start()
        {
            // Start Load Order

            if (battle_scene_enabled)
            {
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            }
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
        }*/
    }
}
