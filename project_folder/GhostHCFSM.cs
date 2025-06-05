using GAlgoT2430.AI;
using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PacmanGame
{
    public class GhostHCFSM : HCFSM
    {
        public enum State { Steal, Home, Goal, Stop };

        public State CurrentState;

        private GameEngine _game;
        private Ghost _ghost;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        private Tile _srcTile;
        private Tile _destTile;
        private LinkedList<Tile> _path;
        private Tile _altDestTile;
        private LinkedList<Tile> _altPath;

        private int _totalWaypoints;
        private Tile[] _waypoints;
        private Tile[] _altWaypoints;
        private int nextWaypoint;

        public GhostHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph)
        {
            _game = game;
            _ghost = ghost;
            _tiledMap = map;
            _tileGraph = graph;
        }

        public override void Initialize()
        {
            CurrentState = State.Stop;

            Steal_Initialize();
        }

        public override void Update()
        {
            Steal_Action();
        }

        private void Steal_Initialize()
        {
            nextWaypoint = 0;

            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

            TiledMapObjectLayer waypoints = gameMap.TiledMap.GetLayer<TiledMapObjectLayer>("Waypoints");
            _totalWaypoints = 0;

            foreach (TiledMapObject powerPellet in waypoints.Objects)
            {
                if (powerPellet.Name != "Home" && powerPellet.Name != "Goal")
                {
                    _totalWaypoints++;

                }

            }
            Debug.WriteLine($"Total waypoints: {_totalWaypoints}.");

            _waypoints = new Tile[_totalWaypoints];
            _altWaypoints = new Tile[2];

            Tile homeTile = null;

            foreach (TiledMapObject powerPellet in waypoints.Objects)
            {
                if (powerPellet.Name == "Home")
                {
                    homeTile = Tile.ToTile(powerPellet.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
                    _altWaypoints[0] = homeTile; // Store Home
                }
                else if (powerPellet.Name == "Goal")
                {
                    _altWaypoints[1] = Tile.ToTile(powerPellet.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
                }
            }

            if (homeTile == null)
            {
                throw new Exception("Error: Home tile not found in the map.");
            }

            List<(Tile pellet, int pathLength)> pelletPaths = new List<(Tile, int)>();

            foreach (TiledMapObject powerPellet in waypoints.Objects)
            {
                if (powerPellet.Name != "Home" && powerPellet.Name != "Goal")
                {
                    Tile pelletTile = Tile.ToTile(powerPellet.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
                    LinkedList<Tile> path = AStar.Compute(_tileGraph, homeTile, pelletTile, AStarHeuristic.EuclideanSquared);

                    if (path != null)
                    {
                        pelletPaths.Add((pelletTile, path.Count)); // Store Pellet and Path Length
                    }
                }
            }

            // Sort the Power Pellets based on path length from Home
            pelletPaths.Sort((a, b) => a.pathLength.CompareTo(b.pathLength));

            // Store the sorted Power Pellets in _waypoints
            _totalWaypoints = pelletPaths.Count;
            _waypoints = new Tile[_totalWaypoints];

            for (int i = 0; i < _totalWaypoints; i++)
            {
                _waypoints[i] = pelletPaths[i].pellet;
            }

            //Debug.WriteLine("Sorted Power Pellets (by distance from Home):");
            for (int i = 0; i < _totalWaypoints; i++)
            {
                Debug.WriteLine($"Pellet {i + 1}: {_waypoints[i]}");
            }


            _srcTile = new Tile(gameMap.StartColumn, gameMap.StartRow);

            _ghost.Position = Tile.ToPosition(_srcTile, _tiledMap.TileWidth, _tiledMap.TileHeight);

        }

        private void Steal_Action()
        {
            int tileWidth = _tiledMap.TileWidth;
            int tileHeight = _tiledMap.TileHeight;

            if (CurrentState == State.Stop)
            {
                _destTile = _waypoints[nextWaypoint];
                Debug.WriteLine($"Current State: {CurrentState}, Previous Tile {_srcTile}, Next Waypoint: {nextWaypoint}, Destination Tile: {_destTile}");

                if (_tileGraph.Nodes.Contains(_destTile) && !_destTile.Equals(_srcTile))
                {
                    _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);

                    _path.RemoveFirst(); // Remove the source tile from the path

                    _ghost.UpdateAnimatedSprite(_srcTile, _path.First.Value);

                    CurrentState = State.Steal;
                }
            }
            else if (CurrentState == State.Steal)
            {
                float elapsedSeconds = ScalableGameTime.DeltaTime;

                if (_path.Count == 0 ||
                    _ghost.Position.Equals(Tile.ToPosition(_destTile, tileWidth, tileHeight))
                   )
                {

                    // Update source tile to destination tile
                    _srcTile = _destTile;
                    _destTile = null;

                    _ghost.OnTileReached(_srcTile);
                    _altDestTile = _altWaypoints[0];
                    _altPath = AStar.Compute(_tileGraph, _srcTile, _altDestTile, AStarHeuristic.EuclideanSquared);
                    _altPath.RemoveFirst();
                    _ghost.UpdateAnimatedSprite(_srcTile, _altPath.First.Value);

                    CurrentState = State.Home;

                    if (nextWaypoint < _totalWaypoints - 1)
                    {
                        nextWaypoint++;
                    }
                    else
                    {
                        _altDestTile = _altWaypoints[1];
                        _altPath = AStar.Compute(_tileGraph, _srcTile, _altDestTile, AStarHeuristic.EuclideanSquared);
                        _altPath.RemoveFirst();
                        _ghost.UpdateAnimatedSprite(_srcTile, _altPath.First.Value);
                        
                        CurrentState = State.Goal;
                    }

                }
                else
                {
                    Moving();
                }

            }
            else if (CurrentState == State.Home)
            {
                float elapsedSeconds = ScalableGameTime.DeltaTime;
                Vector2 homeTile = Tile.ToPosition(_altWaypoints[0], tileWidth, tileHeight);

                if (_altPath.Count == 0 ||
                    _ghost.Position.Equals(Tile.ToPosition(_altDestTile, tileWidth, tileHeight)))
                {
                    _srcTile = _altDestTile;
                    _destTile = null;

                    CurrentState = State.Stop;

                }
                else
                {
                    MovingBackHome();
                }
            }
            else if (CurrentState == State.Goal)
            {
                float elapsedSeconds = ScalableGameTime.DeltaTime;
                Vector2 goalTile = Tile.ToPosition(_altWaypoints[1], tileWidth, tileHeight);

                if (_altPath.Count == 0 ||
                    _ghost.Position.Equals(Tile.ToPosition(_altDestTile, tileWidth, tileHeight)))
                {
                    _srcTile = _altDestTile;
                    _destTile = null;
                }
                else
                {
                    MovingBackHome();
                }
            }
        }

        private void Moving()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            int tileWidth = _tiledMap.TileWidth;
            int tileHeight = _tiledMap.TileHeight;

            Tile nextTile = _path.First.Value;

            Vector2 nextTilePosition = Tile.ToPosition(nextTile, tileWidth, tileHeight);

            if (_ghost.Position.Equals(nextTilePosition))
            {
                Debug.WriteLine($"Reach next tile (Col = {nextTile.Col}, Row = {nextTile.Row}).");

                Tile newNextTile = _path.First.Next.Value;
                _ghost.UpdateAnimatedSprite(nextTile, newNextTile);

                Debug.WriteLine($"Removing next tile. Get new next tile from path.");
                _path.RemoveFirst();

                // Get the next destination position
                newNextTile = _path.First.Value;
            }

            // Move the ghost to the new tile location
            _ghost.Position = _ghost.Move(_ghost.Position, nextTilePosition, elapsedSeconds);

            _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
        }

        private void MovingBackHome()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            int tileWidth = _tiledMap.TileWidth;
            int tileHeight = _tiledMap.TileHeight;

            Tile nextTile2 = _altPath.First.Value;

            Vector2 headTilePosition = Tile.ToPosition(nextTile2, tileWidth, tileHeight);

            if (_ghost.Position.Equals(headTilePosition))
            {
                Debug.WriteLine($"Reach new tile ({nextTile2}).");
                
                Tile newNextTile2 = _altPath.First.Next.Value;
                _ghost.UpdateAnimatedSprite(nextTile2, newNextTile2);

                Debug.WriteLine($"Removing new tile. Get new next tile from path.");
                _altPath.RemoveFirst();

                // Get the next destination position
                newNextTile2 = _altPath.First.Value;

            }

            // Move the ghost to the new tile location
            _ghost.Position = _ghost.Move(_ghost.Position, headTilePosition, elapsedSeconds);

            _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
        }

    }
}