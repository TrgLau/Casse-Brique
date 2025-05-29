using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;



partial class Program
{
    static List<Bonus> bonuses = new();
    static int currentLevel = 0;
    enum GameState { Menu,Jouer,Options, Exit }
    static GameState gameState = GameState.Menu;
    static RenderWindow window;
    static Clock clock;
    static Font font;

    static float deltaTime;

    static RectangleShape quitButton;

    static RectangleShape playButton;

    static int selectedIndex = 0;

    static GameState currentState = new();

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
 
        clock = new Clock();

        List<string> menuItems = new() {"Jouer","Options","Quitter"};
        int selectedIndex = 0;

        

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


        List<Comet> comets = new();
        float cometWaveTimer = 0f;
        float cometWaveInterval = 20f; 
        int cometWaveSize = 2;



        //////////////////
        /// Menu
        /// 
        /// 
        /// 
        /// 


        void UpdateOptions()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                currentState = GameState.Menu;
        }
        
        void DrawOptions()
        {
            Text title = new("OPTIONS", font, 32)
            {
                Position = new Vector2f(100, 100),
                FillColor = Color.Cyan
            };

            Text hint = new("[Échap] Retour", font, 16)
            {
                Position = new Vector2f(100, 160),
                FillColor = Color.Blue
            };

            window.Draw(title);
            window.Draw(hint);
        }


        window.KeyPressed += (sender, e) =>
        {
            if (gameState == GameState.Menu)
            {
                if (e.Code == Keyboard.Key.Up)
                {
                    selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
                }
                else if (e.Code == Keyboard.Key.Down)
                {
                    selectedIndex = (selectedIndex + 1) % menuItems.Count;
                }
                else if (e.Code == Keyboard.Key.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0:
                            currentState = GameState.Jouer;
                            gameloop();
                            break;
                        case 1:
                            currentState = GameState.Options;
                            break;
                        case 2:
                            currentState = GameState.Exit;
                            break;
                    }
                }

            }
        };

        void SpawnCometWave()
        {
            Random rand = new();
            for (int i = 0; i < cometWaveSize; i++)
            {
                float y = rand.Next(0, 1); // haut de l'écran
                float angleOffset = (float)(rand.NextDouble() * 0.1 - 0.10); // petite variation

                Vector2f startPos = new Vector2f(600 + i * 40, y);

                // ✅ angle vers le bas-gauche, plus marqué
                Vector2f dir = new Vector2f(-1f, 0.5f + angleOffset);

                comets.Add(new Comet(startPos, dir,2));
            }
        }

        
        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear();

            deltaTime = clock.Restart().AsSeconds();
            cometWaveTimer += deltaTime;

            // Mise à jour des comètes existantes
            foreach (var comet in comets)
            {
                comet.Update(deltaTime);
                comet.Draw(window);
            }
            // Nettoyage des comètes hors écran
            comets.RemoveAll(c => c.IsOffScreen(window.Size));

            // Timer pour les vagues
            cometWaveTimer += deltaTime;

            if (cometWaveTimer >= cometWaveInterval)
            {
                SpawnCometWave();
                cometWaveTimer = 0f;
            }



            foreach (var star in stars)
            {
                star.Update(deltaTime, 600);
                window.Draw(star.Shape);
            }

            if (gameState == GameState.Menu)
            {
                foreach (var c in comets)
                    c.Draw(window);

                comets.RemoveAll(c => c.IsOffScreen(window.Size));

                switch (currentState)
                {
                    case GameState.Jouer:
                        gameloop();
                        break;
                    case GameState.Options:
                        UpdateOptions();
                        DrawOptions();
                        break;
                    case GameState.Exit:
                        window.Close();
                        break;
                }
                for (int i = 0; i < menuItems.Count; i++)
                {
                    var text = new Text(menuItems[i], font, 24)
                    {
                        Position = new Vector2f(350, 200 + i * 40),
                        FillColor = i == selectedIndex ? Color.Yellow : Color.White
                    };
                    window.Draw(text);
                }
            
                window.Draw(gameBorder);
            }
            else if (gameState == GameState.Jouer)
            {
                gameloop();
            }
            else if (gameState == GameState.Exit)
            {
                window.Close();
            }

            comets.RemoveAll(c => c.IsOffScreen(window.Size));

            window.Display();
        }


    }

}