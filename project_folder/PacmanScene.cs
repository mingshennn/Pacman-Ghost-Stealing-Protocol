using GAlgoT2430.Engine;

namespace PacmanGame
{
    public class PacmanScene : GameScene
    {
        public override void CreateScene()
        {
            // Game map
            GameMap gameMap = new GameMap("GameMap");

            //Ghost's Start Position
            gameMap.StartColumn = 0;
            gameMap.StartRow = 14;

            // Pathfinding Tester
            // PathfindingTester pathfindingTester = new PathfindingTester("PathfindingTester");

            PelletTracker pelletTracker = new PelletTracker("PelletTracker");
            pelletTracker.PowerPelletMaxTime = 10;

            // Ghost
            Ghost ghost = new Ghost();

            //Pac Man
            //Pacman pacman = new Pacman();
            //pacman.Speed = 100f;
            //pacman.StartColumn = 1;
            //pacman.StartRow = 1;
            //pacman.NavigableTileLayerName = "Food";
        }
    }
}
