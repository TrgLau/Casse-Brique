using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;



partial class Program
{
    static List<Bonus> bonuses = new();
    static int currentLevel = 0;

    enum GameState { Menu, Briques, Rogue, Exit }

    static GameState gameState = GameState.Menu;
    static RenderWindow window;
    static Clock clock;
    static Clock clock2;
    static Font font;

    static float deltaTime;

    static RectangleShape quitButton;

    static RectangleShape playButton;

    static RectangleShape rogueButton;

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
            rogueButton.FillColor = Color.Green;
            quitButton.FillColor = Color.Red;
        }
        else if (selectedIndex == 1)
        {
            playButton.FillColor = Color.Green;
            rogueButton.FillColor = Color.Yellow;
            quitButton.FillColor = Color.Red;
        }
        else if (selectedIndex == 2)
        {
            playButton.FillColor = Color.Green;
            rogueButton.FillColor = Color.Green;
            quitButton.FillColor = Color.Yellow;
        }
    }

    static void Main()
    {

        window = new RenderWindow(new VideoMode(800, 600), "Debug LAULAU");
        window.SetFramerateLimit(60);
        window.Closed += (s, e) => window.Close();

        const float borderThickness = 8f;

        RectangleShape gameBorder = new RectangleShape(new Vector2f(
            window.Size.X - borderThickness * 2,
            window.Size.Y - borderThickness * 2))
        {
            Position = new Vector2f(borderThickness, borderThickness),
            OutlineThickness = borderThickness,
            OutlineColor = Color.White,
            FillColor = Color.Transparent
        };


        font = new Font("arial.ttf");

        selectedIndex = 0;

        stars = new();
        Random rand2 = new();

        void CreateStarLayer(int count, float speed, float sizeMin, float sizeMax, byte alpha)
        {
            for (int i = 0; i < count; i++)
            {
                float x = rand2.Next(0, 800);
                float y = rand2.Next(0, 600);
                float size = (float)(rand2.NextDouble() * (sizeMax - sizeMin) + sizeMin);
                stars.Add(new BackGrndStar(x, y, size, speed, alpha));
            }
        }


        CreateStarLayer(40, 20f, 0.8f, 1.5f, 80);
        CreateStarLayer(40, 40f, 1.0f, 2.0f, 140);
        CreateStarLayer(40, 70f, 1.5f, 3.0f, 220);


        Random rand = new();

        clock2 = new Clock();


        Text titleText = new Text("Multi-Jeux", font, 60)
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
            Position = new Vector2f(300, 450),
            FillColor = Color.Red
        };

        rogueButton = new RectangleShape(new Vector2f(200, 60))
        {
            Position = new Vector2f(300, 350),
            FillColor = Color.Green
        };


        Text playText = new Text("Brique", font, 24)
        {
            FillColor = Color.Black,
            Position = new Vector2f(
                playButton.Position.X + 60,
                playButton.Position.Y + 15
            )
        };
        Text rogueText = new Text("Rogue", font, 24)
        {
            FillColor = Color.Black,
            Position = new Vector2f(
                rogueButton.Position.X + 60,
                rogueButton.Position.Y + 15
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
                        gameState = GameState.Rogue;
                    else if (selectedIndex == 2)
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
                else if (rogueButton.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    gameState = GameState.Rogue;
            }
        };

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear();

            deltaTime = clock2.Restart().AsSeconds();

            foreach (var star in stars)
            {
                star.Update(deltaTime, 600);
                window.Draw(star.Shape);
            }

            if (gameState == GameState.Menu)
            {

                window.Draw(titleText);
                window.Draw(playButton);
                window.Draw(quitButton);
                window.Draw(playText);
                window.Draw(quitText);
                window.Draw(gameBorder);
                window.Draw(rogueButton);
                window.Draw(rogueText);
            }
            else if (gameState == GameState.Briques)
            {

                gameloop();
            }
            else if (gameState == GameState.Exit)
            {
                window.Close();
            }
            else if (gameState == GameState.Rogue)
            {
               // RogueInit();
            }

            window.Display();
        }


    }

}