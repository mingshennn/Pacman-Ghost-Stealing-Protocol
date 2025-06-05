//using var game = new Pacman_A2.Game1();
using var game = new GAlgoT2430.Engine.GameEngine("Pacman Game", 1224, 720); //Map's Pixel Dimension (Width, Height)
game.AddScene("PacmanScene", new PacmanGame.PacmanScene());
game.Run();
