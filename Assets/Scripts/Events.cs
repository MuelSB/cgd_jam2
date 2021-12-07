namespace Core
{
    public static class Events 
    {
        // const event id's
        public const string LoadTransition = "LoadTransition";
        public const string LoadComplete   = "LoadComplete";

        public const string LoadMenu  = "LoadMenu";
        public const string LoadGame  = "LoadGame";
        public const string LoadLose  = "LoadLose";
        public const string LoadWin   = "LoadWin";
        public const string LoadBoss  = "LoadBoss";
        public const string PauseGame = "PauseGame";
        
        // Turn Manager Events
        public const string LevelLoaded  = "LevelLoaded"; 
        public const string RoundStarted = "RoundStarted"; 
        public const string RoundEnded   = "RoundEnded";
        public const string TurnStarted  = "TurnStarted"; 
        public const string TurnEnded    = "TurnEnded";
        
        //
        public const string PlayerTurnStarted  = "PlayerTurnStarted";
        public const string PlayerTurnEnded    = "PlayerTurnEnded";
        
        public const string AddAbility = "AddAbility";
        public const string AddAP = "AddAP";
        public const string ReduceAP = "ReduceAP";
        public const string ResetAP = "ResetAP";
        public const string HighlightTiles = "HighlightTiles";
        public const string DisableHighlights = "DisableHighlights";

        public const string EntityKilled = "EntityKilled";
    }
}
