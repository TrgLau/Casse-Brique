using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;
/*


partial class Program
{


    static string fullIntroText = "Tu ouvres les yeux... l'air est humide. Une présence te surveille............ \n Un méchant très trèèèès méchant pixel bot regarde de haut en refusant tout travail ";
    static string currentIntroText = "";
    static float charDisplayTimer = 0f;
    static int introCharIndex = 0;
    static bool introDone = false;

    static RectangleShape ground;
    static float groundTargetY;
    static bool groundRising = false;

    static Clock clock3 = new();

    static void RogueInit()
    {
        float groundHeight = 100f;
        ground = new RectangleShape(new Vector2f(window.Size.X, groundHeight))
        {
            FillColor = new Color(80, 50, 30),
            Position = new Vector2f(0, window.Size.Y) // commence hors de l'écran
        };

        groundTargetY = window.Size.Y - groundHeight;
        groundRising = false;


        window.DispatchEvents();
        window.Clear();
        float dt2 = clock3.Restart().AsSeconds();
        Text intro = new Text(currentIntroText, font, 20)
        {
            Position = new Vector2f(50, 300),
            FillColor = Color.Cyan
        };

        charDisplayTimer += dt2;

        if (charDisplayTimer >= 0.05f && introCharIndex < fullIntroText.Length)
        {
            currentIntroText += fullIntroText[introCharIndex];
            introCharIndex++;
            charDisplayTimer = 0f;
        }

        if (introCharIndex >= fullIntroText.Length)
        {
            introDone = true;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
        {
            introDone = true;
        }

        if (introDone && !groundRising)
        {

            groundRising = true;
        }

        if (groundRising && ground.Position.Y > groundTargetY)
        {
            ground.Position = new Vector2f(0, ground.Position.Y - 100f * dt2); // vitesse montée
        }

        RogueInit();

        window.Draw(intro);
        window.Draw(ground);

    }
}


partial class Program
{


    static string fullIntroText = "Tu ouvres les yeux... l'air est humide. Une présence te surveille............ \n Un méchant très trèèèès méchant pixel bot regarde de haut en refusant tout travail ";
    static string currentIntroText = "";
    static float charDisplayTimer = 0f;
    static int introCharIndex = 0;
    static bool introDone = false;

    static RectangleShape ground;
    static float groundTargetY;
    static bool groundRising = false;

    static Clock clock3 = new();

    static void RogueInit()
    {
        float groundHeight = 100f;
        ground = new RectangleShape(new Vector2f(window.Size.X, groundHeight))
        {
            FillColor = new Color(80, 50, 30),
            Position = new Vector2f(0, window.Size.Y) // commence hors de l'écran
        };

        groundTargetY = window.Size.Y - groundHeight;
        groundRising = false;

        
            window.DispatchEvents();
            window.Clear();
            float dt2 = clock3.Restart().AsSeconds();
            Text intro = new Text(currentIntroText, font, 20)
            {
                Position = new Vector2f(50, 300),
                FillColor = Color.Cyan
            };

            charDisplayTimer += dt2;

            if (charDisplayTimer >= 0.05f && introCharIndex < fullIntroText.Length)
            {
                currentIntroText += fullIntroText[introCharIndex];
                introCharIndex++;
                charDisplayTimer = 0f;
            }

            if (introCharIndex >= fullIntroText.Length)
            {
                introDone = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                introDone = true;
            }

            if (introDone && !groundRising)
            {
                
                groundRising = true;
            }

            if (groundRising && ground.Position.Y > groundTargetY)
            {
                ground.Position = new Vector2f(0, ground.Position.Y - 100f * dt2); // vitesse montée
            }
            window.Draw(intro);
            window.Draw(ground);
        
    }
}*/