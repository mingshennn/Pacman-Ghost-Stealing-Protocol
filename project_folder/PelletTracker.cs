using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;

namespace PacmanGame
{
    public class PelletTracker : GameObject
    {
        /********************************************************************************
            PROBLEM 2(B) : Create C# delegates and events (PelletsCleared, PowerPellet
                         , PowerPelletRunning, PowerPelletEnded)


            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            public delegate void PelletsClearedHandler();
            public delegate void PowerPelletStartedHandler();
            public delegate void PowerPelletRunningHandler(float remainingSeconds);
            public delegate void PowerPelletEndedHandler();

            public event ________ PelletsCleared;
            public event ________ PowerPelletStarted;
            public event ________ PowerPelletRunning;
            public event PowerPelletEndedHandler PowerPelletEnded;
        ********************************************************************************/

        public delegate void PelletsClearedHandler();
        public delegate void PowerPelletStartedHandler();
        public delegate void PowerPelletRunningHandler(float remainingSeconds);
        public delegate void PowerPelletEndedHandler();

        public event PelletsClearedHandler PelletsCleared;
        public event PowerPelletStartedHandler PowerPelletStarted;
        public event PowerPelletRunningHandler PowerPelletRunning;
        public event PowerPelletEndedHandler PowerPelletEnded;

        public float PowerPelletMaxTime;
        public Texture2D Texture;

        private HashSet<Tile> _coverTiles;
        private Rectangle _coverTileRect;
        private TiledMapTileLayer _pelletLayer;
        private Pacman _pacman;
        private Ghost _ghost;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;
        private float _powerPelletActiveTime;

        public PelletTracker(string name) : base(name)
        {
            _coverTiles = new HashSet<Tile>();
        }

        public override void LoadContent()
        {   //Tile Map 1
            Texture = _game.Content.Load<Texture2D>("pacman1-map");

            //Tile Map 2
            //Texture = _game.Content.Load<Texture2D>("pacman-wall-24");
        }

        public override void Initialize()
        {
            _coverTileRect = new Rectangle(96, 0, 24, 24);
            _powerPelletActiveTime = 0f;

            // Get graph
            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

            // Get pellet layer
            _pelletLayer = _tiledMap.GetLayer<TiledMapTileLayer>("Food");

            // Get pacman
            _pacman = (Pacman)GameObjectCollection.FindByName("Pacman");
            _ghost = (Ghost)GameObjectCollection.FindByName("Ghost_2");

            /********************************************************************************
                PROBLEM 1 : Register CoverPelletTileWithEmptyTile() method to listen to
                            Pacman's TileReached event.

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                // Register events
                _pacman.TileReached += ________;
            ********************************************************************************/

            // Register events
            //_pacman.TileReached += CoverPelletTileWithEmptyTile;
            _ghost.TileReached += CoverPelletTileWithEmptyTile;
        }

        public override void Update()
        {
            /********************************************************************************
			PROBLEM 2(C) : Update the power pellet timer and invoke event handlers


			HOWTOSOLVE : 1. Copy the code below.
						 2. Paste it below this block comment.
						 3. Fill in the blanks.

			// Power pellet still active
			if (_powerPelletActiveTime > ________)
			{
				// Update power pellet timer
				_powerPelletActiveTime -= ScalableGameTime.DeltaTime;

				// Invoke the PowerPelletEnded event if the power
				//   pellet active time has ended
				if (_powerPelletActiveTime <= ________)
				{
					_powerPelletActiveTime = 0f;
					PowerPelletEnded?.Invoke();
				}
				else
				{
					// Run callbacks that listen to active power pellet
					PowerPelletRunning?.Invoke(________);
				}
			}
		********************************************************************************/

            if (_powerPelletActiveTime > 0f)
            {
                // Update power pellet timer
                _powerPelletActiveTime -= ScalableGameTime.DeltaTime;

                // Invoke the PowerPelletEnded event if the power
                //   pellet active time has ended
                if (_powerPelletActiveTime <= 0f)
                {
                    _powerPelletActiveTime = 0f;
                    PowerPelletEnded?.Invoke();
                }
                else
                {
                    // Run callbacks that listen to active power pellet
                    PowerPelletRunning?.Invoke(_powerPelletActiveTime);
                }
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();

            // Draw all cover tiles
            foreach (Tile t in _coverTiles)
            {
                Vector2 position = Tile.ToPosition(t, _pelletLayer.TileWidth, _pelletLayer.TileHeight);
                _game.SpriteBatch.Draw(Texture, position, _coverTileRect, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 1f);
            }

            _game.SpriteBatch.End();
        }

        public void RestartPowerPelletTime()
        {
            _powerPelletActiveTime = PowerPelletMaxTime;
            PowerPelletStarted?.Invoke();
        }

        public void CoverPelletTileWithEmptyTile(Tile pelletTileLocation)
        {
            bool hasTile = _pelletLayer.TryGetTile((ushort)pelletTileLocation.Col, (ushort)pelletTileLocation.Row, out TiledMapTile? ghostTile);

            if (hasTile)
            {
                // Pellet tile (3) or Power Pellet tile (4)
                if (/*pacmanTile.Value.GlobalIdentifier == 3 ||*/ ghostTile.Value.GlobalIdentifier == 4)
                {
                    /********************************************************************************
                     PROBLEM 1 : Fill up the pellet tiles with empty tiles


                     HOWTOSOLVE : 1. Copy the code below.
                                  2. Paste it below this block comment.
                                  3. Fill in the blanks.

                     // Add empty tile only to cover newly discovered pellet tiles
                     if (!_coverTiles.Contains(pelletTileLocation))
                     {
                         _coverTiles.Add(________);

                         // Fires PelletsCleared event if all pellets has been cleared
                         // (i.e. number of cover tiles == number of navigable nodes in the graph)
                         if (_coverTiles.Count == _tileGraph.________.________)
                         {
                             PelletsCleared?.Invoke();
                         }

                         // Restart power pellet count down timer if one is found
                         if (pacmanTile.Value.GlobalIdentifier == 4)
                         {
                             RestartPowerPelletTime();
                         }
                     }
                 ********************************************************************************/

                    if (!_coverTiles.Contains(pelletTileLocation))
                    {
                        _coverTiles.Add(pelletTileLocation);

                        // Fires PelletsCleared event if all pellets has been cleared
                        // (i.e. number of cover tiles == number of navigable nodes in the graph)
                        if (_coverTiles.Count == _tileGraph.Nodes.Count)
                        {
                            PelletsCleared?.Invoke();
                        }

                        // Restart power pellet count down if one is found
                        if (ghostTile.Value.GlobalIdentifier == 4)
                        {
                            RestartPowerPelletTime();
                        }
                    }
                }
            }
        }
    }
}
