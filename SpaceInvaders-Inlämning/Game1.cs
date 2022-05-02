//  
// *********************************
// ** Made By David Persson TE20B **
// *********************************
//
// Space Invaders - 2022/04 - V1.0
// F11 = GameOver (ADMIN KEY WHEN TESTING)
// Left Shift Key => SpaceShip Moves faster (RIGHT OR LEFT)
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace Space_Invaders___Projektuppgift
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Menu, Points and Variables for Lives
        MouseState mouse = Mouse.GetState();
        MouseState oldMouse = Mouse.GetState();

        // Control Spaceship with keyboard
        KeyboardState keyboard = Keyboard.GetState();
        KeyboardState checkedKeyboard = Keyboard.GetState();

        // Content loading and positions etc...
        Song music, MenuSong;
        SoundEffect laser, Enemy, Explosion, GameOverSound, HitEnemy;
        SpriteFont Fonts;
        Texture2D Play, Menu1, Menu2, SpaceShip, SpaceShipLives, SpaceShipShot, Stars, Stars2, Planet, Alien, ShotToAliens, GameOverScreen, Yes, No;
        Vector2 MenuTextPosition, scorePosition, SingleShotPosition, MultiShotPosition, InGameTimer, PlayAgainPos, MyLevelPos;
        Rectangle Menu1BackGround, Menu2BackGround, PlayButton, SingleShotRectangle, MultiShotRectangle, YesRec, NoRec, AlienShot, OneShot;

        Dictionary<string, int> integers = new Dictionary<string, int>();

        // Position on my Backgrounds (BG)
        Rectangle StarBGPosition, Star2BGPos, PlanetPosition, GameOverPosition;
        Random r = new Random();

        // Strings with text
        string MenyText = "Welcome To Space Invaders!";
        string Single = "SingleShot";
        string Multi = "MultiShots";
        string points = " ";
        string IGTimer = " ";
        string MyLevelText = " ";
        string PlayAgainText = "Play Again?";

        int scene = 0;
        float Score;
        int GameTime;
        int GameTimeDecimals;
        int time;
        int timerDecimals;

        // Position on my SpaceShip
        Rectangle SpaceShipRectangle;
        List<Rectangle> SpaceShipRectangleLives = new List<Rectangle>();

        // Position on Aliens && Variables for Aliens
        List<Rectangle> Aliens = new List<Rectangle>();
        int amountOfAliensX = 12;
        int amountOfAliensY = 4;
        int AlienWidth;
        int AlienHeight;
        bool AlienToRight = true;
        bool AlienToLeft = false;
        bool AlienY = false;
        bool tickspeed = false;

        // Variables for the Alien when its shooting
        int RandomAlien;
        int AlienShotSpeed = 10;

        // Spaceship shooting
        int shotingSpeed = 12;
        bool shots = false;
        Rectangle[] Fired = new Rectangle[100];

        // FullScreen
        int DefaultWindowHeight = 480;
        int DefaultWindowWidth = 800;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            FullScreen();

        }

        protected void FullScreen()
        {
            // Copies the Width & Height of the screen and sets that as the new graphics value upon running the code
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;

            _graphics.ApplyChanges();
            DefaultWindowHeight = _graphics.PreferredBackBufferHeight;
            DefaultWindowWidth = _graphics.PreferredBackBufferWidth;
        }


        protected override void Initialize()
        {

            integers["ShipLives"] = 3;
            integers["timer"] = 0;
            integers["level"] = 1;
            integers["AlienSpeed"] = 20;
            integers["pace"] = 5;
            integers["fire"] = 0;
            integers["ammo"] = 100;

            // Gives the array Fired[i] x amount of positions for the shots (ammo) 
            for (int i = 0; i < integers["ammo"]; i++)
            {
                Fired[i] = new Rectangle(0, -100, 5, 15);
            }


            // Lives
            for (int b = 0; b < integers["ShipLives"]; b++)
            {
                SpaceShipRectangleLives.Add(new Rectangle((0 + (b * 100)), 75, 100, 100));
            }

            // Fills the List<Rectangle> Aliens with numbers for the aliens

            AlienWidth = DefaultWindowWidth / 22;
            AlienHeight = DefaultWindowHeight / 16;

            for (int i = 0; i < amountOfAliensX; i++)
            {
                for (int j = 0; j < amountOfAliensY; j++)
                {
                    Aliens.Add(new Rectangle(150 + (i * (DefaultWindowWidth / (amountOfAliensX + 3))), 200 + (j * DefaultWindowHeight / amountOfAliensX), AlienWidth, AlienHeight));
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Menu / Buttons
            Menu1 = Content.Load<Texture2D>("Meny");
            Menu2 = Content.Load<Texture2D>("Meny");
            Play = Content.Load<Texture2D>("Spela");
            Menu1BackGround = new Rectangle(0, 0, DefaultWindowWidth, DefaultWindowHeight);
            Menu2BackGround = new Rectangle(0, (Menu1BackGround.Y - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);
            PlayButton = new Rectangle((DefaultWindowWidth / 2) - (Play.Width / 2), DefaultWindowHeight / 3, 275, 75);
            Yes = Content.Load<Texture2D>("Yes");
            No = Content.Load<Texture2D>("No");
            YesRec = new Rectangle(DefaultWindowWidth / 2 - (Yes.Width * 2), DefaultWindowHeight - (DefaultWindowHeight / 4), 330, 130);
            NoRec = new Rectangle(DefaultWindowWidth / 2 + No.Width, DefaultWindowHeight - (DefaultWindowHeight / 4), 330, 130);

            // Loading Text
            Fonts = Content.Load<SpriteFont>("Fonts");

            // Position On The Text
            MenuTextPosition = new Vector2((DefaultWindowWidth / 3 + 100), DefaultWindowHeight / 5);
            scorePosition = new Vector2(DefaultWindowWidth / 2 - 100, 0);
            InGameTimer = new Vector2(0, 0);
            PlayAgainPos = new Vector2(DefaultWindowWidth / 2 - (DefaultWindowWidth / 13), DefaultWindowHeight - (DefaultWindowHeight / 3));
            MyLevelPos = new Vector2(DefaultWindowWidth - 250, 0);

            SingleShotPosition = new Vector2((DefaultWindowWidth / 2 - 100), DefaultWindowHeight / 4);
            MultiShotPosition = new Vector2((DefaultWindowWidth / 2 - 100), DefaultWindowHeight / 2);

            SingleShotRectangle = new Rectangle(DefaultWindowWidth / 2 - 100, DefaultWindowHeight / 4, 275, 50);
            MultiShotRectangle = new Rectangle(DefaultWindowWidth / 2 - 100, DefaultWindowHeight / 2, 275, 50);

            // Background
            Stars = Content.Load<Texture2D>("stars");
            Stars2 = Content.Load<Texture2D>("stars");
            StarBGPosition = new Rectangle(0, 0, DefaultWindowWidth, DefaultWindowHeight);
            Star2BGPos = new Rectangle(0, (StarBGPosition.Y - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);
            GameOverScreen = Content.Load<Texture2D>("GameOver");
            GameOverPosition = new Rectangle(0, 0, DefaultWindowWidth, DefaultWindowHeight);

            // Planet in the background
            Planet = Content.Load<Texture2D>("planets");
            PlanetPosition = new Rectangle(0, 0, Planet.Width / 2, Planet.Height / 2);

            // Content for the Spaceship
            SpaceShip = Content.Load<Texture2D>("Spaceship - Space Invaders");
            SpaceShipLives = Content.Load<Texture2D>("Spaceship - Space Invaders");
            SpaceShipShot = Content.Load<Texture2D>("Spaceship - shot");
            OneShot = new Rectangle(0, -100, 5, 15);
            SpaceShipRectangle = new Rectangle((DefaultWindowWidth / 2), (DefaultWindowHeight - SpaceShip.Height / 2), (SpaceShip.Width / 2), SpaceShip.Height / 2);

            // Content for the Aliens
            Alien = Content.Load<Texture2D>("Alien");
            ShotToAliens = Content.Load<Texture2D>("Spaceship - shot");
            AlienShot = new Rectangle(0, -100, 6, 18);

            // Music
            music = Content.Load<Song>("music");
            MenuSong = Content.Load<Song>("MenuSong");
            // Start Menu Music
            MediaPlayer.Play(MenuSong);
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.IsRepeating = true;

            // Sound effect
            laser = Content.Load<SoundEffect>("LaserSound");
            Enemy = Content.Load<SoundEffect>("Enemy");
            Explosion = Content.Load<SoundEffect>("Explosion");
            GameOverSound = Content.Load<SoundEffect>("GameOverSound");
            HitEnemy = Content.Load<SoundEffect>("HitEnemy");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            oldMouse = mouse;
            mouse = Mouse.GetState();

            switch (scene)
            {
                // Menu
                case 0:
                    UpdateMenu();
                    MovingBackground();
                    Score = 0;
                    GameTime = 0;
                    GameTimeDecimals = 0;
                    integers["level"] = 1;
                    break;
                // Select Gamemode
                case 1:
                    UpdateMenu();
                    MovingBackground();
                    break;
                // Gamemode - One Shot
                case 2:
                    UpdateGameSingleShot();
                    UpdateMenu();
                    break;
                // Gamemode - Multiple Shot
                case 3:
                    UpdateGameInfinityShots();
                    UpdateMenu();
                    break;
                // GameOver
                case 4:
                    {
                        GameOver();
                        Reset();
                        break;
                    }
            }

            base.Update(gameTime);
        }

        // Menu
        void UpdateMenu()
        {
            // When you click on play you swap scene
            if (LeftClick() == true && PlayButton.Contains(mouse.Position) == true)
            {
                SwapScene(1);
            }
            if (LeftClick() == true && SingleShotRectangle.Contains(mouse.Position) == true)
            {
                SwapScene(2);
                MediaPlayer.Play(music); // Play Game music
                MediaPlayer.Volume = 0.2f;
                MediaPlayer.IsRepeating = true;
            }
            else if (LeftClick() == true && MultiShotRectangle.Contains(mouse.Position) == true)
            {
                SwapScene(3);
                MediaPlayer.Play(music); // Play Game music
                MediaPlayer.Volume = 0.2f;
                MediaPlayer.IsRepeating = true;
            }
            // When you have 0 lives left -> GameOver || F11 for direct gameover (ADMIN KEY)
            if (integers["ShipLives"] == 0 || Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                SwapScene(4);
                MediaPlayer.Stop(); // Music stop
                GameOverSound.Play(volume: 0.1f, pitch: 0.0f, pan: 0.0f);
                integers["ShipLives"] = 3;
            }
        }

        protected void GameOver()
        {
            if (LeftClick() == true && NoRec.Contains(mouse.Position) == true)
            {
                Exit();
            }
            if (LeftClick() == true && YesRec.Contains(mouse.Position) == true)
            {
                SwapScene(0);
            }
        }
        bool LeftClick()
        {
            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void SwapScene(int newScen)
        {
            scene = newScen;
        }

        // Game with one shot
        void UpdateGameSingleShot()
        {
            MovingBackground(); // Makes the background move
            MoveOneShot(); // Moves my shot to the player
            FireOneShot(); // Fire
            SpaceInvaders(); // Control the spaceship && spacebar
            MoveAliens(); // Alien movement
            ShotInterferWithAlien(); // When I hit alien
            AlienShotPlacement(); // Randomly place out aliens shot
            Time(); // timer in the top left corner

            if (Aliens.Count == 0)
            {
                ReDrawAliens();
                integers["timer"] = 0;
                integers["level"]++;
                MyLevelText = $"Level {integers["level"]}";
            }
        }

        // Game with multiple shots
        void UpdateGameInfinityShots()
        {
            MovingBackground(); // Makes the background move
            MoveShots(); // Moves my shot to the player
            FireShots(); // Fire
            SpaceInvaders(); // Control the spaceship && spacebar
            MoveAliens(); // Alien movement
            ShotInterferWithAlien(); // When I hit alien
            AlienShotPlacement(); // Randomly place out aliens shot
            Time(); // timer in the top left corner

            if (Aliens.Count == 0)
            {
                ReDrawAliens(); // When Aliens = 0 -> Respawns aliens
                integers["timer"] = 0;
                integers["level"]++;
                MyLevelText = $"Level {integers["level"]}";
            }
        }

        protected void MovingBackground()
        {
            Menu1BackGround.Y += 1;
            Menu2BackGround.Y += 1;

            int nextValue = r.Next(-1500, -500);
            StarBGPosition.Y += 1;
            Star2BGPos.Y += 1;
            PlanetPosition.X += 2;
            PlanetPosition.Y += 1;

            // Moves the meny background
            if (Menu1BackGround.Y == DefaultWindowHeight)
                Menu1BackGround = new Rectangle(0, (0 - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);

            if (Menu2BackGround.Y == DefaultWindowHeight)
                Menu2BackGround = new Rectangle(0, (0 - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);

            // Moves the planet and randomly puts it in different X values
            if (PlanetPosition.X > DefaultWindowWidth)
            {
                PlanetPosition = new Rectangle(nextValue, 0, Planet.Width / 2, Planet.Height / 2);
            }

            // Moves the Star Background In Game
            if (StarBGPosition.Y == DefaultWindowHeight)
                StarBGPosition = new Rectangle(0, (0 - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);

            if (Star2BGPos.Y == DefaultWindowHeight)
                Star2BGPos = new Rectangle(0, (0 - DefaultWindowHeight), DefaultWindowWidth, DefaultWindowHeight);
        }

        protected void MoveOneShot()
        {
            if (OneShot.Y < 0 && keyboard.IsKeyDown(Keys.Space) && checkedKeyboard.IsKeyUp(Keys.Space))
            {
                laser.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
                OneShot = new Rectangle((SpaceShipRectangle.X + (SpaceShip.Width / 4)), (DefaultWindowHeight - (SpaceShip.Height / 5) * 2), 5, 15);
            }
        }

        protected void FireOneShot()
        {
            if (OneShot.Y > -10)
            {
                OneShot.Y -= shotingSpeed;
            }
        }

        protected void SpaceInvaders()
        {
            checkedKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            // Spaceship movement (Left & Right) && If you hold down shift you moves faster
            if (keyboard.IsKeyDown(Keys.Left) || (keyboard.IsKeyDown(Keys.A)) && (SpaceShipRectangle.X) > 0 - (SpaceShipRectangle.Width / 2))
            {
                SpaceShipRectangle.X -= integers["pace"];
            }
            if ((keyboard.IsKeyDown(Keys.Right) || (keyboard.IsKeyDown(Keys.D))) && ((SpaceShipRectangle.X + SpaceShipRectangle.Width/2)) < DefaultWindowWidth)
            {
                SpaceShipRectangle.X += integers["pace"];
            }
            if (((keyboard.IsKeyDown(Keys.Left) && keyboard.IsKeyDown(Keys.LeftShift))) || ((keyboard.IsKeyDown(Keys.A)) && keyboard.IsKeyDown(Keys.LeftShift)) && (SpaceShipRectangle.X) > 0 - (SpaceShipRectangle.Width / 2))
            {
                SpaceShipRectangle.X -= integers["pace"] + 1;
            }
            if (((keyboard.IsKeyDown(Keys.Right) && keyboard.IsKeyDown(Keys.LeftShift))) || ((keyboard.IsKeyDown(Keys.D)) && keyboard.IsKeyDown(Keys.LeftShift)) && (SpaceShipRectangle.X + (SpaceShipRectangle.Width / 2)) < DefaultWindowWidth)
            {
                SpaceShipRectangle.X += integers["pace"] + 1;
            }

            // If you press Spacebar bool shots = true
            if (keyboard.IsKeyDown(Keys.Space) && checkedKeyboard.IsKeyUp(Keys.Space))
            {
                if (keyboard.IsKeyDown(Keys.Space) && checkedKeyboard.IsKeyUp(Keys.Space) && shots)
                {
                    integers["fire"]++;
                }
                shots = true;
            }
            // Sets back the number of shots to 0, because there is 100 shots and when all have been used it changes back to the first one
            // because by that time it should be out of the window unless you click 100 times / second
            if (integers["fire"] == integers["ammo"])
            {
                integers["fire"] = 0;
            }
        }

        protected void MoveShots()
        {
            // Moves the next shot to the Spaceship
            if (shots && keyboard.IsKeyDown(Keys.Space) && checkedKeyboard.IsKeyUp(Keys.Space))
            {
                laser.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
                Fired[integers["fire"]] = new Rectangle((SpaceShipRectangle.X + (SpaceShip.Width / 4)), (DefaultWindowHeight - (SpaceShip.Height / 5) * 2), 5, 15);
            }
        }

        protected void FireShots()
        {
            if (shots)
            {
                for (int i = 0; i < Fired.Length; i++)
                {
                    // The shot moves up by 'shotingSpeed' 60 times/s when below y = -10
                    if (Fired[i].Y > -10)
                    {
                        Fired[i].Y -= shotingSpeed;
                    }
                }
            }
        }

        protected void MoveAliens()
        {
            int MoveAlienY = DefaultWindowHeight / 20;

            if (integers["timer"] == (60 - (integers["level"] * 5)) && integers["level"] < 11)
            {
                tickspeed = true;
                integers["timer"] = 0;
            }
            else if (integers["timer"] == 5 && integers["level"] == 10)
            {
                tickspeed = true;
                integers["timer"] = 0;
            }

            if (tickspeed && Aliens.Count > 0)
            {
                for (int i = 0; i < Aliens.Count; i++)
                {
                    Rectangle AliensTempX = Aliens[i];
                    if ((Aliens[i].X <= 0 + integers["AlienSpeed"]))
                    {
                        AlienToRight = true;
                        AlienToLeft = false;
                        AlienY = true;
                    }
                    if (Aliens[i].X + AlienWidth >= (DefaultWindowWidth - integers["AlienSpeed"]))
                    {
                        AlienToLeft = true;
                        AlienToRight = false;
                        AlienY = true;
                    }
                    if (AlienToRight)
                    {
                        AliensTempX.X += integers["AlienSpeed"];
                        Aliens[i] = AliensTempX;
                    }
                    if (AlienToLeft)
                    {
                        AliensTempX.X -= integers["AlienSpeed"];
                        Aliens[i] = AliensTempX;
                    }
                }
                tickspeed = false;
            }
            if (AlienY) // moves alien in Y axsis 
            {
                for (int j = 0; j < Aliens.Count; j++)
                {
                    Rectangle AliensTempY = Aliens[j];
                    {
                        if (Aliens[j].Y < SpaceShipRectangle.Y)
                        {
                            AliensTempY.Y += MoveAlienY;
                            Aliens[j] = AliensTempY;
                        }
                        if (Aliens[j].Y >= SpaceShipRectangle.Y)
                        {
                            integers["ShipLives"] = 0;
                        }
                        AlienY = false;
                    }
                }
            }
            integers["timer"]++;
        }

        protected void ShotInterferWithAlien()
        {
            // points scale with your 'level' (level increase depending on how many times you have shot all the aliens)
            int AlienPoints = 20 + (integers["level"] * 5);
            points = $"Your Score: {Score}";

            Rectangle FiredTemp;
            // For Gamemode SingleShot -> Removes alien
            for (int j = 0; j < Aliens.Count; j++)
            {
                if (OneShot.Intersects(Aliens[j]))
                {
                    Aliens.RemoveAt(j);
                    HitEnemy.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                    OneShot = new Rectangle(0, -100, 5, 15);
                    Score += AlienPoints;
                }
            }

            // For Gamemode with multiple shots -> Remove aliens
            for (int i = 0; i < Fired.Length; i++)
            {
                for (int j = 0; j < Aliens.Count; j++)
                {
                    FiredTemp = Fired[i];
                    if (FiredTemp.Intersects(Aliens[j]))
                    {
                        Aliens.RemoveAt(j);
                        HitEnemy.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                        Fired[i] = new Rectangle(0, -100, 5, 15);
                        Score += AlienPoints;
                    }
                }
            }
        }

        protected void AlienShotPlacement()
        {
            Random RandomAliens = new Random();
            int TempPosX;
            int TempPosY;

            if (AlienShot.Y <= 0 && Aliens.Count > 0)
            {
                RandomAlien = RandomAliens.Next(0, Aliens.Count - 1);
                TempPosX = Aliens[RandomAlien].X;
                TempPosY = Aliens[RandomAlien].Y;
                Enemy.Play(volume: 0.05f, pitch: 0.0f, pan: 0.0f);
                AlienShot = new Rectangle(TempPosX + (AlienWidth / 2), TempPosY + (AlienHeight / 2), 6, 18);
            }
            else
            {
                AlienShotMove(); // Moves aliens shot to the screen => Makes the shot move down 
            }
        }

        protected void AlienShotMove() // Move aliens shot down
        {
            Rectangle AlienTemp;

            if (AlienShot.Y < DefaultWindowHeight && Aliens.Count > 0)
            {
                AlienTemp = AlienShot;
                AlienTemp.Y += (AlienShotSpeed + integers["level"]);
                AlienShot = AlienTemp;

                if (AlienTemp.Intersects(SpaceShipRectangle)) // If Aliens shot intersects with player (spaceship) remove one life
                {
                    AlienShot = new Rectangle(0, -100, 6, 18);
                    Explosion.Play(volume: 0.1f, pitch: 0.0f, pan: 0.0f);
                    integers["ShipLives"]--;
                    SpaceShipRectangleLives.RemoveAt(integers["ShipLives"]);
                }
                else if (AlienShot.Y >= DefaultWindowHeight) // When Aliens shot is out of screen moves it back to start position
                {
                    AlienShot = new Rectangle(0, -100, 6, 18);
                }
                for (int i = 0; i < Fired.Length; i++) // If Aliens shot intersects with my shot, 'destroy' aliens shot (For Gamemode with mulitple shots)
                {
                    if (AlienTemp.Intersects(Fired[i]))
                    {
                        AlienShot = new Rectangle(0, -100, 6, 18);
                    }
                }
                if (AlienTemp.Intersects(OneShot)) // If Aliens shot intersects with my shot, 'destroy' aliens shot (For Gamemode with one shot)
                {
                    AlienShot = new Rectangle(0, -100, 6, 18);
                }
            }
        }

        protected void ReDrawAliens()
        {
            for (int i = 0; i < amountOfAliensX; i++)
            {
                for (int j = 0; j < amountOfAliensY; j++)
                {
                    Aliens.Add(new Rectangle(150 + (i * (DefaultWindowWidth / (amountOfAliensX + 3))), 200 + (j * DefaultWindowHeight / amountOfAliensX), AlienWidth, AlienHeight));
                }
            }
        }

        protected void Time() // Timer top left corner
        {
            MyLevelText = $"Level {integers["level"]}";
            if (timerDecimals == 1)
            {
                GameTimeDecimals += 1;
                IGTimer = $"Time: {GameTime}:{GameTimeDecimals}";
                if (GameTimeDecimals == 60)
                {
                    GameTimeDecimals = 0;
                }
                timerDecimals = 0;
            }
            if (time == 60)
            {
                GameTime += 1;
                time = 0;
            }
            timerDecimals++;
            time++;
        }

        protected void Reset() // Resets everything after gameover (If you want to play again without leave the game)
        {
            Aliens.Clear();
            SpaceShipRectangleLives.Clear();
            IGTimer = $"You survived for: {GameTime}.{GameTimeDecimals} Seconds";
            points = $"Your Score: {Score}";

            OneShot = new Rectangle(0, -100, 5, 15);
            for (int a = 0; a < integers["ammo"]; a++)
            {
                Fired[a] = new Rectangle(0, -100, 5, 15);
            }
            for (int b = 0; b < integers["ShipLives"]; b++)
            {
                SpaceShipRectangleLives.Add(new Rectangle((0 + (b * 100)), 75, 100, 100));
            }
            for (int i = 0; i < amountOfAliensX; i++)
            {
                for (int j = 0; j < amountOfAliensY; j++)
                {
                    Aliens.Add(new Rectangle(150 + (i * (DefaultWindowWidth / (amountOfAliensX + 3))), 200 + (j * DefaultWindowHeight / amountOfAliensX), AlienWidth, AlienHeight));
                }
            }
            SpaceShipRectangle = new Rectangle((DefaultWindowWidth / 2), (DefaultWindowHeight - SpaceShip.Height / 2), (SpaceShip.Width / 2), SpaceShip.Height / 2);
        }

        // Highscore **Not Finished**

        /* protected void WriteScoreToTxT()
        {
            string filnamn = @"c:\Antekningar\SpaceInvaders.txt";
            string[] leaderboard;
            if (!File.Exists(filnamn))
            {
                leaderboard = new string[10];
                for (int i = 0; i < leaderboard.Length; i++)
                {
                    leaderboard[i] = $"{i + 1}. ";
                }
                File.WriteAllLines(filnamn, leaderboard);
            }
            else
            {
                leaderboard = File.ReadAllLines(filnamn);
            }
            for (int i = 0; i < 2; i++)
            {
                leaderboard[i] = $"{i + 1}. {Score}";
                File.WriteAllLines(filnamn, leaderboard);
            }
        } */


        // Draw 
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            switch (scene)
            {
                case 0:
                    DrawMenu1(); // Draw Start Menu
                    break;
                case 1:
                    DrawMenu2(); // Draw Gamemode Menu
                    break;
                case 2:
                    DrawGame(); // Draw Game With one shot
                    DrawSingleShot();
                    break;
                case 3:
                    DrawGame(); // Draw Game with multiple shot
                    DrawMultiShots();
                    break;
                case 4:
                    DrawGameOver(); // Draw Game Over
                    break;

            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        void DrawMenu1()
        {
            _spriteBatch.Draw(Menu1, Menu1BackGround, Color.White);
            _spriteBatch.Draw(Menu2, Menu2BackGround, Color.White);
            _spriteBatch.DrawString(Fonts, MenyText, MenuTextPosition, Color.Red);
            _spriteBatch.Draw(Play, PlayButton, Color.White);
        }

        void DrawMenu2()
        {
            _spriteBatch.Draw(Menu1, Menu1BackGround, Color.White);
            _spriteBatch.Draw(Menu2, Menu2BackGround, Color.White);
            _spriteBatch.DrawString(Fonts, Single, SingleShotPosition, Color.Red);
            _spriteBatch.DrawString(Fonts, Multi, MultiShotPosition, Color.Red);
        }
        void DrawGame()
        {

            // Background
            _spriteBatch.Draw(Planet, PlanetPosition, Color.White);
            _spriteBatch.Draw(Stars, StarBGPosition, Color.White);
            _spriteBatch.Draw(Stars2, Star2BGPos, Color.White);
            _spriteBatch.Draw(Planet, PlanetPosition, Color.White);

            // Spaceship & Shots
            _spriteBatch.Draw(SpaceShip, SpaceShipRectangle, Color.White);

            // AlienAmmo
            _spriteBatch.Draw(ShotToAliens, AlienShot, Color.White);

            /* Aliens, XNA has 4 varibles, RGB and the fourth one called 'alpha' which is the transparency of the sprite
            In this case I want the Aliens to have a 20% transparency so you can see the background behind them*/
            foreach (Rectangle a in Aliens)
            {
                _spriteBatch.Draw(Alien, a, Color.White * 0.8f);
            }

            // Draw 'Lives'
            foreach (Rectangle space in SpaceShipRectangleLives)
            {
                _spriteBatch.Draw(SpaceShipLives, space, Color.LightBlue);
            }

            // Just some text if I want to see some values / My points
            _spriteBatch.DrawString(Fonts, IGTimer, InGameTimer, Color.White);
            _spriteBatch.DrawString(Fonts, points, scorePosition, Color.White);
            _spriteBatch.DrawString(Fonts, MyLevelText, MyLevelPos, Color.White);
        }

        void DrawSingleShot()
        {
            _spriteBatch.Draw(SpaceShipShot, OneShot, Color.White);
        }

        void DrawMultiShots()
        {
            foreach (Rectangle s in Fired)
            {
                _spriteBatch.Draw(SpaceShipShot, s, Color.White);
            }
        }

        void DrawGameOver()
        {
            _spriteBatch.Draw(GameOverScreen, GameOverPosition, Color.White);
            _spriteBatch.DrawString(Fonts, points, scorePosition, Color.White);
            _spriteBatch.DrawString(Fonts, IGTimer, InGameTimer, Color.White);
            _spriteBatch.DrawString(Fonts, PlayAgainText, PlayAgainPos, Color.Red);
            _spriteBatch.Draw(Yes, YesRec, Color.White);
            _spriteBatch.Draw(No, NoRec, Color.White);
        }
    }
}