using GAlgoT2430.Engine;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace PacmanGame
{
    public class GameMap : GameObject
    {
        public TiledMap TiledMap { get; private set; }
        public TiledMapRenderer TiledMapRenderer { get; private set; }

        /********************************************************************************
            PROBLEM 7 : Define Tile Graph.

            HOWTOSOLVE : 1. Just uncomment the given code below.

            public TileGraph TileGraph { get; private set; }
        ********************************************************************************/

        public TileGraph TileGraph { get; private set; }

        // Defines the row and column of any navigable tile to construct the tile graph
        public ushort StartColumn;
        public ushort StartRow;

        public GameMap(string name) : base(name)
        {
        }

		public override void LoadContent()
        {
            //Tile Map 1
            TiledMap = _game.Content.Load<TiledMap>("pacman1");     // Program.cs - (Width: 1224, Height: 720)

            //Tile Map 2
            //TiledMap = _game.Content.Load<TiledMap>("pacman2");   // Program.cs - (Width: 1200, Height: 720)
        }

        public override void Initialize()
        {
            LoadContent();

            // Initialize tiled map renderer
            TiledMapRenderer = new TiledMapRenderer(_game.GraphicsDevice, TiledMap);

            // Get the Food layer from the tiled map
            TiledMapTileLayer foodLayer = TiledMap.GetLayer<TiledMapTileLayer>("Food");

            /********************************************************************************
                PROBLEM 7 : Construct tile graph from the food layer.

                HOWTOSOLVE : 1. Just uncomment the given code.

                TileGraph = new TileGraph();
                TileGraph.CreateFromTiledMapTileLayer(foodLayer, StartColumn, StartRow);
            ********************************************************************************/

            TileGraph = new TileGraph();
            TileGraph.CreateFromTiledMapTileLayer(foodLayer, StartColumn, StartRow);
        }

        public override void Update()
        {
            TiledMapRenderer.Update(ScalableGameTime.GameTime);
        }

        public override void Draw()
        {
            TiledMapRenderer.Draw();
        }
    }
}
