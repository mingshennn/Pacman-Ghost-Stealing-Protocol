//using GAlgoT2430.AI;
//using GAlgoT2430.Engine;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Tiled;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace PacmanGame
//{
//    using Autofac.Features.OwnedInstances;
//    using Microsoft.Xna.Framework.Input;
//    using MonoGame.Extended.Graphics;
//    using System;
//    using System.IO;
//    using System.Linq;

//    public class NavigationHCFSM : HCFSM
//    {
//        // FSM for navigation
//        enum NavigationState { STOP, MOVING };

//        // Navigation current state
//        private NavigationState _currentState = NavigationState.STOP;

//        // Navigation
//        private Tile _srcTile;
//        private Tile _destTile;
//        private LinkedList<Tile> _path;

//        // Visual appearance
//        private Rectangle _ghostRect;
//        private TiledMap _tiledMap;
//        private TileGraph _tileGraph;

//        private Ghost _ghost;
//        private GameEngine _game;

//        public NavigationHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph) : base()
//        {
//            _game = game;
//            _ghost = ghost;
//            _tiledMap = map;
//            _tileGraph = graph;
//        }

//        public override void Initialize()
//        {
//            //GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
//            //_tiledMap = gameMap.TiledMap;
//            //_tileGraph = gameMap.TileGraph;

//            //_srcTile = new Tile(gameMap.StartColumn, gameMap.StartRow);

//            //_tiledMap = gameMap.TiledMap;
//            //_tileGraph = gameMap.TileGraph;

//            _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
//        }

//        public override void Update()
//        {
//            MouseState mouse = Mouse.GetState();

//            int tileWidth = _tiledMap.TileWidth;
//            int tileHeight = _tiledMap.TileHeight;

//            // Implement the movement behaviour
//            if (_currentState == NavigationState.STOP)
//            {
//                // Left mouse button pressed
//                if (mouse.LeftButton == ButtonState.Pressed)
//                {
//                    // Get destination tile as the mouse-selected tile
//                    _destTile = Tile.ToTile(mouse.Position.ToVector2(), tileWidth, tileHeight);

//                    if (_tileGraph.Nodes.Contains(_destTile) &&
//                        !_destTile.Equals(_srcTile)
//                       )
//                    {
//                        // Transition Actions
//                        // 1. Compute an A* path
//                        _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);
//                        // 2. Remove the source tile from the path
//                        _path.RemoveFirst();

//                        /********************************************************************************
//                            PROBLEM 3(C): Switch animation based on the source tile and the next tile.


//                            HOWTOSOLVE : 1. Copy the code below.
//                                         2. Paste it below this block comment.
//                                         3. Fill in the blanks.

//                            // The animation to play is determined based on difference between:
//                            // (a) The tile the ghost is standing on (i.e. the source tile in this case)
//                            // (b) The next tile the ghost will move towards
//                            //     (i.e. the first tile in the path after the source tile is removed)
//                            UpdateAnimatedSprite(________, ________);

//                        ********************************************************************************/

//                        _ghost.UpdateAnimatedSprite(_srcTile, _path.First.Value);

//                        // Change to MOVING state
//                        _currentState = NavigationState.MOVING;
//                    }

//                    // NOTE: No action to execute for STOP state
//                }
//            }
//            else if (_currentState == NavigationState.MOVING)
//            {
//                float elapsedSeconds = ScalableGameTime.DeltaTime;

//                if (_path.Count == 0 ||
//                    _ghost.Position.Equals(Tile.ToPosition(_destTile, tileWidth, tileHeight))
//                   )
//                {
//                    // Update source tile to destination tile
//                    _srcTile = _destTile;
//                    _destTile = null;

//                    // Change to STOP state
//                    _currentState = NavigationState.STOP;
//                }

//                // Action to execute on the MOVING state
//                else
//                {
//                    Tile nextTile = _path.First.Value; // throw exception if path is empty

//                    Vector2 nextTilePosition = Tile.ToPosition(nextTile, tileWidth, tileHeight);

//                    if (_ghost.Position.Equals(nextTilePosition))
//                    {
//                        Debug.WriteLine($"Reached the next tile (Col = {nextTile.Col}, Row = {nextTile.Row}).");

//                        Tile newNextTile = _path.First.Value;
//                        _ghost.UpdateAnimatedSprite(nextTile, newNextTile);

//                        Debug.WriteLine($"Removing this tile from the path and getting the new next tile from path.");

//                        _path.RemoveFirst();
//                        /********************************************************************************
//                            PROBLEM 3(C): Update the animation based on the current tile and next tile .


//                            HOWTOSOLVE : 1. Copy the code below.
//                                         2. Paste it below this block comment.
//                                         3. Fill in the blanks.

//                            // Get the position of the new next tile from the path
//                            _path.RemoveFirst();
//                            Tile newNextTile = _path.________.________;
//                            nextTilePosition = Tile.ToPosition(________, tileWidth, ________);

//                            // Update the animation
//                            UpdateAnimatedSprite(nextTile, ________);

//                        ********************************************************************************/

//                        // Get the position of the new next tile from the path


//                        //nextTilePosition = Tile.ToPosition(newNextTile, tileWidth, tileHeight);
//                        newNextTile = _path.First.Value;
//                        // Update the animation

//                    }

//                    // Move the ghost to the new tile location
//                    _ghost.Position = _ghost.Move(_ghost.Position, nextTilePosition, elapsedSeconds);

//                    /********************************************************************************
//                        PROBLEM 3(C): Running the ghost animation.


//                        HOWTOSOLVE : 1. Copy the code below.
//                                     2. Paste it below this block comment.
//                                     3. Fill in the blanks.

//                        AnimatedSprite.Update(________);

//                    ********************************************************************************/

//                    _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
//                }
//            }
//        }

//        //public override void Update()
//        //{
//        //    if (_currentState == NavigationState.STOP)
//        //    {
//        //        if (_waypoints.Count > 0)
//        //        {
//        //            // If at start, use the Home position as the source
//        //            if (_srcTile == null)
//        //            {
//        //                _srcTile = Tile.ToTile(_home, _tiledMap.TileWidth, _tiledMap.TileHeight);
//        //            }

//        //            // Find the nearest waypoint
//        //            Vector2 nextPellet = _waypoints.OrderBy(p => Vector2.Distance(_ghost.Position, p)).First();

//        //            _destTile = Tile.ToTile(nextPellet, _tiledMap.TileWidth, _tiledMap.TileHeight);
//        //            _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);

//        //            if (_path == null || _path.Count == 0)
//        //            {
//        //                Debug.WriteLine("A* failed to compute a valid path.");
//        //                _currentState = NavigationState.STOP;
//        //                return;
//        //            }

//        //            _path.RemoveFirst();
//        //            _currentState = NavigationState.MOVING;
//        //        }
//        //        else if (_waypoints.Count == 0 && _destTile == null)
//        //        {
//        //            _destTile = Tile.ToTile(_goal, _tiledMap.TileWidth, _tiledMap.TileHeight);
//        //            _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);

//        //            if (_path == null || _path.Count == 0)
//        //            {
//        //                Debug.WriteLine("A* failed to compute a path to Goal.");
//        //                _currentState = NavigationState.STOP;
//        //                return;
//        //            }

//        //            _path.RemoveFirst();
//        //            _currentState = NavigationState.MOVING;
//        //        }
//        //    }
//        //    else if (_currentState == NavigationState.MOVING)
//        //    {
//        //        if (_path == null || _path.Count == 0)
//        //        {
//        //            Debug.WriteLine("No path available. Returning to STOP state.");
//        //            _currentState = NavigationState.STOP;
//        //            return;
//        //        }

//        //        Tile nextTile = _path.First.Value;
//        //        Vector2 nextPosition = Tile.ToPosition(nextTile, _tiledMap.TileWidth, _tiledMap.TileHeight);

//        //        if (_ghost.Position.Equals(nextPosition))
//        //        {
//        //            _path.RemoveFirst();

//        //            if (_path.Count == 0)
//        //            {
//        //                GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
//        //                if (_waypoints.Contains(Tile.ToPosition(_srcTile, _tiledMap.TileWidth, _tiledMap.TileHeight)))
//        //                {
//        //                    gameMap.RemovePowerPellet(Tile.ToPosition(_srcTile, _tiledMap.TileWidth, _tiledMap.TileHeight));
//        //                }

//        //                _currentState = NavigationState.STOP;
//        //                _srcTile = _destTile;
//        //                _destTile = null;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            _ghost.Position = _ghost.Move(_ghost.Position, nextPosition, ScalableGameTime.DeltaTime);
//        //        }
//        //    }
//        //}
//    }
//}
