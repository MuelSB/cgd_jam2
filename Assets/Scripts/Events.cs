namespace Core
{
    public static class Events 
    {
        // const event id's
        public const string LoadTransition = "LoadTransition";
        public const string LoadComplete = "LoadComplete";

        public const string LoadMenu  = "LoadMenu";
        public const string LoadGame  = "LoadGame";
        public const string PauseGame = "PauseGame";
        
        // Turn Manager Events
        public const string LevelLoaded  = "LevelLoaded"; 
        public const string RoundStarted = "RoundStarted"; 
        public const string RoundEnded   = "RoundEnded";
        public const string TurnStarted  = "TurnStarted"; 
        public const string TurnEnded    = "TurnEnded";
        
        // Values
        public const string BroadcastPlayer = "BroadcastPlayer";

        // Gameplay
    }
}
