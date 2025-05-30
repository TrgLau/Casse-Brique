
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Principal;
using System.IO;

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

public class HighScoreEntry
{
    public string Name { get; set; }
    public int Score { get; set; }
    public string Date { get; set; } 
}

partial class Program
{
    static float doubleScoreTimer = 0f;
    static bool isDoubleScoreActive => doubleScoreTimer > 0;
    
    static bool isLaserActive = false;
    static float laserTimer = 0f;
    static List<RectangleShape> lasers = new();
    const float laserSpeed = 600f;
    static bool isFireBallActive = false;
    static float fireBallTimer = 0f;

    static void SaveScore(int newScore, string name = "Player")
    {
        string path = "highscores.json";
        List<HighScoreEntry> scores = new();

        
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                scores = JsonSerializer.Deserialize<List<HighScoreEntry>>(json);
            }
            catch
            {
                scores = new();
            }
        }

        
        scores.Add(new HighScoreEntry
        {
            Name = name,
            Score = newScore,
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        });

        
        scores = scores
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Date) 
            .Take(10)
            .ToList();


        var options = new JsonSerializerOptions { WriteIndented = true };
        string newJson = JsonSerializer.Serialize(scores, options);
        File.WriteAllText(path, newJson);
    }
    static void gameloop()
    {

        float entitySpawnTimer = 15f;
        bool entitiesSpawned = false;
        List<Entity> entities = new();

        const float borderThickness = 8f;

        RectangleShape gameBorder = new RectangleShape(new Vector2f(
            window.Size.X - borderThickness * 2,
            window.Size.Y - borderThickness * 2))
        {
            Position = new Vector2f(borderThickness, borderThickness),
            OutlineThickness = borderThickness,
            OutlineColor = new Color(237, 107, 26),

            FillColor = Color.Transparent
        };

        float flashStarsTimer = 0f;
        const float starFlashDuration = 0.5f;
        bool isGameOver = false;

        float timeSinceLevelStart = 0f;

        clock = new Clock();

        deltaTime = clock.Restart().AsSeconds();



        foreach (var star in stars)
        {
            star.Update(deltaTime, 600);
            window.Draw(star.Shape);
        }




        var scoreText = new Text("Score: 0", font, 24)
        {
            Position = new Vector2f(10, 10),
            FillColor = Color.White
        };

        Text laserText = new Text("", font, 16)
        {
            FillColor = Color.Red
        };

        var paddle = new RectangleShape(new Vector2f(120, 20))
        {
            FillColor = Color.White,
            Position = new Vector2f(340, 550)
        };

        List<Ball> balls = new();

        balls.Add(new Ball(new Vector2f(400, 300), new Vector2f(150, -150)));

        int score = 0;

        Vector2f ballVelocity = new Vector2f(200, -200);
        float ballSpeed = ballVelocity.Length();

        var bricks = DrawBricks(currentLevel);


        clock = new Clock();
        Text doubleScoreText = new Text("", font, 16)
        {
            FillColor = Color.Yellow
        };

        float laserCooldown = 0.2f;
        void SpawnEntities()
        {
            entities.Add(new Entity(new Vector2f(300, 200)));
            entities.Add(new Entity(new Vector2f(500, 300)));
        }

        void ApplyBonus(BonusType type)
        {
            switch (type)
            {
                case BonusType.PaddleGrow:
                    paddle.Size = new Vector2f(paddle.Size.X + 40f, paddle.Size.Y);
                    break;
                case BonusType.PaddleShrink:
                    paddle.Size = new Vector2f(Math.Max(40f, paddle.Size.X - 40f), paddle.Size.Y);
                    break;
                case BonusType.ExtraBall:
                    balls.Add(new Ball(paddle.Position + new Vector2f(paddle.Size.X / 2, -10), new Vector2f(1, -1)));
                    break;
                case BonusType.SlowBall:
                    foreach (var b in balls)
                        b.Speed *= 0.8f;
                    break;
                case BonusType.SpeedBall:
                    foreach (var b in balls)
                        b.Speed *= 1.2f;
                    break;
                case BonusType.DoubleScore:
                    doubleScoreTimer = 10f;
                    break;
                case BonusType.Laser:
                    isLaserActive = true;
                    laserTimer = 10f;
                    break;
                case BonusType.FireBall:
                    isFireBallActive = true;
                    fireBallTimer = 10f;
                    break;
            }
        }
        void ResetLevel()
        {
            bricks = LevelLoader.LoadLevel("levels.json", 0);
            balls.Clear();
            balls.Add(new Ball(new Vector2f(400, 500), new Vector2f(200, -200)));

            isGameOver = false;

        }


        List<Comet> comets = new();
        List<ExplosionCircle> explosions = new();
        List<FlashComet> flashComets = new();

        void SpawnCometExplosion(Vector2f center, int count = 24)
        {
            explosions.Add(new ExplosionCircle(center));

            float angleStep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float angleRad = (i * angleStep) * (MathF.PI / 180f);
                Vector2f dir = new Vector2f(MathF.Cos(angleRad), MathF.Sin(angleRad));
                flashComets.Add(new FlashComet(center, 3, dir, 500f, 0.45f));
            }
        }


        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(Color.Black);
            window.Draw(gameBorder);

            deltaTime = clock.Restart().AsSeconds();

            if (!entitiesSpawned)
            {
                entitySpawnTimer -= deltaTime;

                if (entitySpawnTimer <= 0f)
                {
                    SpawnEntities();
                    entitiesSpawned = true;
                }
            }

            if (isLaserActive)
            {
                laserTimer -= deltaTime;
                if (laserTimer <= 0)
                {
                    isLaserActive = false;
                    lasers.Clear();
                }


                if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && laserCooldown <= 0f)
                {
                    var p1 = new RectangleShape(new Vector2f(4, 20))
                    {
                        FillColor = Color.Red,
                        Position = new Vector2f(paddle.Position.X + paddle.Size.X / 2 - 2, paddle.Position.Y - 20)
                    };
                    lasers.Add(p1);
                    laserCooldown = 0.2f;
                }

                laserCooldown -= deltaTime;
            }


            timeSinceLevelStart += deltaTime;


            if (doubleScoreTimer > 0)
                doubleScoreTimer -= deltaTime;

            if (doubleScoreTimer > 0f)
            {
                doubleScoreText.DisplayedString = $"x2 : {doubleScoreTimer:F1}s";

                float margin = 10f;
                float textWidth = doubleScoreText.GetGlobalBounds().Width;
                float textHeight = doubleScoreText.GetGlobalBounds().Height;

                doubleScoreText.Position = new Vector2f(
                    window.Size.X - textWidth - margin - 8f,
                    margin + 8f
                );
            }
            else
            {
                doubleScoreText.DisplayedString = "";
            }

            int baseScore = 100;
            if (isDoubleScoreActive)
                baseScore *= 2;


            float paddleSpeed = 400f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                paddle.Position -= new Vector2f(paddleSpeed * deltaTime, 0);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                paddle.Position += new Vector2f(paddleSpeed * deltaTime, 0);

            paddle.Position = new Vector2f(Math.Clamp(paddle.Position.X, 8, 800 - paddle.Size.X - 8), paddle.Position.Y);

            if (flashStarsTimer > 0f)
                flashStarsTimer -= deltaTime;


            foreach (var star in stars)
            {
                float starIntensity = flashStarsTimer > 0 ? 3.5f : 1.0f;
                star.Update(deltaTime, 600, starIntensity);
                window.Draw(star.Shape);
            }

            if (isFireBallActive)
            {
                fireBallTimer -= deltaTime;
                if (fireBallTimer <= 0)
                    isFireBallActive = false;
            }

            foreach (var brick in bricks)
                brick.Update(deltaTime);

            foreach (var brick in bricks)
                brick.Draw(window);



            if (bricks.Count == 0)
            {
                currentLevel++;
                balls.Clear();
                bricks = DrawBricks(currentLevel);
                ballVelocity = new Vector2f(200, -200);
                balls.Add(new Ball(new Vector2f(400, 300), new Vector2f(150, -150)));
            }

            foreach (var bonus in bonuses)
                bonus.Update(deltaTime);

            foreach (var bonus in bonuses.ToList())
            {
                if (bonus.Intersects(paddle))
                {
                    ApplyBonus(bonus.Type);
                    bonuses.Remove(bonus);
                }
            }
            scoreText.DisplayedString = "Score: " + score;

            foreach (var e in entities)
                e.Update(deltaTime, window.Size);

            window.Draw(paddle);


            foreach (var bonus in bonuses)
            {
                if (bonus.IsActive)
                    bonus.Draw(window);
            }
            window.Draw(scoreText);


            foreach (var ball in balls)
            {
                float speedMultiplier = 1.0f + (timeSinceLevelStart / 20f * 0.1f);
                ball.SetSpeedMultiplier(speedMultiplier);
                ball.Update(deltaTime);
                ball.CheckWallCollision(window.Size);
                ball.CheckPaddleCollision(paddle);
                int newscore = ball.CheckBrickCollisions(bricks, bonuses, isFireBallActive, score, isDoubleScoreActive);

                if (newscore > score)
                {
                    flashStarsTimer = starFlashDuration;
                    score = newscore;
                }

                window.Draw(ball.Shape);

                if (isFireBallActive)
                    ball.AnimateFireColor(deltaTime);
                else
                    ball.Shape.FillColor = Color.White;
            }
            if (balls.Count == 0 && !isGameOver)
            {
                isGameOver = true;
                SaveScore(score, "Player");
                score = 0;


            }





            foreach (var e in explosions.ToList())
            {
                e.Update(deltaTime);
                if (e.IsFinished)
                    explosions.Remove(e);
            }

            foreach (var comet in flashComets.ToList())
            {
                comet.Update(deltaTime);
                if (comet.IsDead)
                    flashComets.Remove(comet);
            }


            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                // TODO pour faire des tests
            }



            //////////////////////////////////
            /// 
            ///  Gestion des bonus
            /// 
            /// 

            foreach (var entity in entities)
            {
                foreach (var ball in balls)
                {
                    if (entity.CheckCollision(ball))
                    {
                        entity.IsAlive = false;
                        score += isDoubleScoreActive ? 1000 : 500;
                    }
                }
            }
            foreach (var e in entities)
            {
                if (e.IsAlive)
                    window.Draw(e.Shape);
            }



            foreach (var laser in lasers)
                window.Draw(laser);
            foreach (var laser in lasers.ToList())
            {
                laser.Position -= new Vector2f(0, laserSpeed * deltaTime);

                foreach (var brick in bricks.ToList())
                {
                    if (laser.GetGlobalBounds().Intersects(brick.Shape.GetGlobalBounds()))
                    {
                        brick.Hit();
                        SpawnCometExplosion(new Vector2f(laser.Position.X, laser.Position.Y));
                        if (brick.IsDestroyed)
                            score += isDoubleScoreActive ? 200 : 100;

                        lasers.Remove(laser);
                        break;
                    }
                }

                if (laser.Position.Y < 0)
                    lasers.Remove(laser);
            }

            if (isLaserActive)
            {
                laserText.DisplayedString = $"Laser : {laserTimer:F1}s";
                laserText.Position = new Vector2f(10f + 8f, 32f + 8f);
                window.Draw(laserText);
            }
            if (isFireBallActive)
            {
                Text fireBallText = new Text($"Feu : {fireBallTimer:F1}s", font, 16)
                {
                    FillColor = Color.Red,
                    Position = new Vector2f(10f + 8f, 56f + 8f)
                };
                window.Draw(fireBallText);
            }

            foreach (var e in explosions)
                e.Draw(window);

            foreach (var c in comets)
                c.Draw(window);



            window.Draw(doubleScoreText);





            //////////////////////////////////
            /// 
            ///  Ménage

            balls.RemoveAll(b => b.IsOutOfBounds(window.Size.Y));
            comets.RemoveAll(c => c.IsOffScreen(window.Size));
            bricks.RemoveAll(b => b.CanBeRemoved());

            if (isGameOver)
            {
                ResetLevel();
                timeSinceLevelStart = 0f;
            }

            window.Display();

        }
    }
}

