using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;



partial class Program
{
    static List<Bonus> bonuses = new();
    static int currentLevel = 0;
    enum GameState { Menu, Briques,Options, Exit }
    static GameState gameState = GameState.Menu;
    static RenderWindow window;
    static Clock clock;
    static Clock clock2;
    static Font font;

    static float deltaTime;

    static RectangleShape quitButton;

    static RectangleShape playButton;

    static int selectedIndex = 0;

    static List<BackGrndStar> stars;

    public static List<Brick> DrawBricks(int levelIndex)
    {
        string jsonText = File.ReadAllText("levels.json");

        LevelData levelData = JsonSerializer.Deserialize<LevelData>(jsonText);

        List<Brick> bricks = new();

        bricks.Clear();
        if (levelIndex >= levelData.levels.Count)
        {
            Console.WriteLine("Tous les niveaux terminés !");
            return bricks;
        }

        bricks = LevelLoader.LoadLevel("levels.json", 0);
        return bricks;
    }

    public static void UpdateMenuSelection()
    {
        if (selectedIndex == 0)
        {
            playButton.FillColor = Color.Yellow;
         
            quitButton.FillColor = Color.Red;
        }
        else if (selectedIndex == 1)
        {
            playButton.FillColor = Color.Green;
          
            quitButton.FillColor = Color.Yellow;
        }
    }

    static void Main()
    {


        ////////////////////////////////////
        /// Gestion fenetre
        /// 

        window = new RenderWindow(new VideoMode(800, 600), "Casse-Briques ( Debug Version )");
        window.SetFramerateLimit(60);
        window.Closed += (s, e) => window.Close();


        ////////////////////////////////////
        /// Gestion de la bordure
        /// 

        const float borderThickness = 8f;  // épaisseur de la bordure

        RectangleShape gameBorder = new RectangleShape(new Vector2f(
            window.Size.X - borderThickness * 2,
            window.Size.Y - borderThickness * 2))
        {
            Position = new Vector2f(borderThickness, borderThickness),
            OutlineThickness = borderThickness,
            OutlineColor = Color.White,
            FillColor = Color.Transparent
        };

        ////////////////////////////
        /// Variables
        /// 

        font = new Font("arial.ttf");

        Random rand = new();
 
        clock2 = new Clock();

        selectedIndex = 0;

        ///////////////////////////////
        /// étoiles du background
        /// 


        stars = new();

        void CreateStarLayer(int count, float speed, float sizeMin, float sizeMax, byte alpha)
        {
            for (int i = 0; i < count; i++)
            {
                float x = rand.Next(0, 800);
                float y = rand.Next(0, 600);
                float size = (float)(rand.NextDouble() * (sizeMax - sizeMin) + sizeMin);
                stars.Add(new BackGrndStar(x, y, size, speed, alpha));
            }
        }

        CreateStarLayer(100, 2f, 0.8f, 1.5f, 80);
        CreateStarLayer(100, 4f, 1.0f, 2.0f, 140);
        CreateStarLayer(40, 7f, 1.5f, 3.0f, 220);


        Comet comet = new Comet(new Vector2f(-100, 200), new Vector2f(1f, 0.2f));


        //////////////////////////////
        /// Menu
        /// 


        Text titleText = new Text("Casse-Briques", font, 60)
        {
            Position = new Vector2f(200, 100),
            FillColor = Color.Cyan
        };

        playButton = new RectangleShape(new Vector2f(200, 60))
        {
            Position = new Vector2f(300, 250),
            FillColor = Color.Green
        };

        quitButton = new RectangleShape(new Vector2f(200, 60))
        {
            Position = new Vector2f(300, 350),
            FillColor = Color.Red
        };

    

        Text playText = new Text("Jouer", font, 24)
        {
            FillColor = Color.Black,
            Position = new Vector2f(
                playButton.Position.X + 60,
                playButton.Position.Y + 15
            )
        };
        

        Text quitText = new Text("Quitter", font, 24)
        {
            FillColor = Color.Black,
            Position = new Vector2f(
                quitButton.Position.X + 50,
                quitButton.Position.Y + 15
            )
        };
        UpdateMenuSelection();


        //////////////////
        /// Menu
        /// 

        window.KeyPressed += (sender, e) =>
        {
            if (gameState == GameState.Menu)
            {
                if (e.Code == Keyboard.Key.Down)
                {
                    selectedIndex = (selectedIndex + 1) % 3;
                    UpdateMenuSelection();
                }
                else if (e.Code == Keyboard.Key.Up)
                {
                    selectedIndex = (selectedIndex - 1) % 3;
                    UpdateMenuSelection();
                }
                else if (e.Code == Keyboard.Key.Enter)
                {
                    if (selectedIndex == 0)
                        gameState = GameState.Briques;
                    else if (selectedIndex == 1)
                        gameState = GameState.Exit;
                }
            }
        };

        window.MouseButtonPressed += (sender, e) =>
        {
            if (gameState == GameState.Menu && e.Button == Mouse.Button.Left)
            {
                var mousePos = Mouse.GetPosition(window);
                var worldPos = new Vector2f(mousePos.X, mousePos.Y);

                if (playButton.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    gameState = GameState.Briques;

                else if (quitButton.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    gameState = GameState.Exit;
            }
        };

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear();

            comet.Update(deltaTime);

            deltaTime = clock2.Restart().AsSeconds();

            foreach (var star in stars)
            {
                star.Update(deltaTime, 600);
                window.Draw(star.Shape);
            }

            if (gameState == GameState.Menu)
            {
                comet.Draw(window);
                window.Draw(titleText);
                window.Draw(playButton);
                window.Draw(quitButton);
                window.Draw(playText);
                window.Draw(quitText);
                window.Draw(gameBorder);
            }
            else if (gameState == GameState.Briques)
            {
                gameloop();
            }
            else if (gameState == GameState.Exit)
            {
                window.Close();
            }

            window.Display();
        }


    }

}