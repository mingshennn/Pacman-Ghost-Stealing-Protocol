using GAlgoT2430.Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.Extended.Graphics;
using System;
using Microsoft.Xna.Framework.Graphics;
using GAlgoT2430.AI;

namespace PacmanGame
{
    public class Ghost : AnimationGameObject // GameObject
    {
        public HCFSM FSM;

        // Delegates
        public delegate void TileReachedHandler(Tile location);
        // Events
        public event TileReachedHandler TileReached;

        // Navigation
        private Tile _srcTile;
        //private Tile _destTile;
        //private LinkedList<Tile> _path;

        // Attributes
        public float MaxSpeed;
        // public Texture2D Texture;

        // Visual appearance
        //private Rectangle _ghostRect;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        public Ghost() : base("ghost-animations.sf")
        {
        }

        // Commented out as the Animation is now loaded in the base class
        //public override void LoadContent()
        //{
        //    Texture = _game.Content.Load<Texture2D>("pacman-sprites");
        //}

        public override void Initialize()
        {
            MaxSpeed = 400.0f; // Ghost's Speed

            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

            /********************************************************************************
                PROBLEM 3(A): Initialise the animation.


                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                // Initialize Animation to "ghostRedDown".
                AnimatedSprite.Set________(________);
                AnimatedSprite.TextureRegion = Sprite________.Texture________[AnimatedSprite.Controller.Current________];

            ********************************************************************************/

            // Initialize Animation to "ghostRedDown".
            AnimatedSprite.SetAnimation("ghostRedDown");
            AnimatedSprite.TextureRegion = SpriteSheet.TextureAtlas[AnimatedSprite.Controller.CurrentFrame];

            // Initialize Source Tile
            _srcTile = new Tile(gameMap.StartColumn, gameMap.StartRow);

            // Initialize Position
            Position = Tile.ToPosition(_srcTile, _tiledMap.TileWidth, _tiledMap.TileHeight);

            FSM = new GhostHCFSM(_game, this, _tiledMap, _tileGraph);
            FSM.Initialize();
        }

        public override void Update()
        {
            FSM.Update();
        }

        public void OnTileReached(Tile tile)
        {
            TileReached?.Invoke(tile);
        }


        public override void Draw()
        {
            // Draw the ghost at his position, extracting only the ghost image from the texture
            _game.SpriteBatch.Begin();

            // Commented out as Texture is not used anymore
            // _game.SpriteBatch.Draw(Texture, Position, _ghostRect, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            /********************************************************************************
                PROBLEM 3(D): Fill in the blanks based on the logic below:

                            IF (1) The destination tile is in the tile graph.
                               (2) The destination tile is not the source tile.

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                
                // _game.SpriteBatch.Draw(________, Position, ________, Scale);

            ********************************************************************************/

            _game.SpriteBatch.Draw(AnimatedSprite, Position, Orientation, Scale);

            _game.SpriteBatch.End();
        }

        // Given source (src) and destination (dest) locations, and elapsed time, 
        //     try to move from source to destination at the given speed within elapsed time.
        // If cannot reach dest within the elapsed time, return the location where it will reach.
        // If can reach or overshoot the dest, the return dest.
        public Vector2 Move(Vector2 src, Vector2 dest, float elapsedSeconds)
        {
            Vector2 dP = dest - src;
            float distance = dP.Length();
            float step = MaxSpeed * elapsedSeconds;

            if (step < distance)
            {
                dP.Normalize();
                return src + (dP * step);
            }
            else
            {
                return dest;
            }
        }

        // Select the ghost current animation based on:
        // (a) Which tile the ghost is standing on (ghostTile)
        // (b) Which tile the ghost is heading next (nextTile)
        //
        // Pre-conditions:
        //    The animation name is suffixed by:
        //      "NorthWest", "Up", "NorthEast", "Left", "Centre", "Right", "SouthWest", "Down", "SouthEast"
        //
        // Example:
        //    If nextTile is on the right of ghostTile, the animation to play is "ghostRedRight".
        public void UpdateAnimatedSprite(Tile ghostTile, Tile nextTile)
        {
            string[] directions = {"NorthWest", "Up"    , "NorthEast",
                                   "Left"     , "Centre", "Right"    ,
                                   "SouthWest", "Down"  , "SouthEast"};

            if (ghostTile == null || nextTile == null)
            {
                throw new ArgumentNullException("UpdateAnimatedSprite(): ghostTile or nextTile is null.");
            }
            else
            {
                /********************************************************************************
                    PROBLEM 3(B): Compute the index value that refer to the correct animation
                                  suffix in the 'directions' array based on ghost tile and next
                                  tile.


                    HOWTOSOLVE : 1. Write your own code.

                    // You may write more lines of code before the code below to compute the index
                    int index = ?;

                ********************************************************************************/

                int deltaX = nextTile.Col - ghostTile.Col;
                int deltaY = nextTile.Row - ghostTile.Row;

                int index = (deltaY + 1) * 3 + (deltaX + 1);

                string animationName = $"ghostRed{directions[index]}";

                if (AnimatedSprite.CurrentAnimation != animationName)
                {
                    AnimatedSprite.SetAnimation(animationName);
                    AnimatedSprite.TextureRegion = SpriteSheet.TextureAtlas[AnimatedSprite.Controller.CurrentFrame];
                }
            }
        }
    }
}
