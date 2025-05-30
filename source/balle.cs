using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;

public class Ball
{   
    static SoundBuffer bounceBuffer = new SoundBuffer("bounce.wav");
    static Sound bounceSound = new Sound(bounceBuffer);
    static SoundBuffer bordsBuffer = new SoundBuffer("bords.wav");
    static Sound bordsSound = new Sound(bordsBuffer);
    static SoundBuffer briqueBuffer = new SoundBuffer("brique.wav");
    static Sound briqueSound = new Sound(briqueBuffer);
    static SoundBuffer looseBuffer = new SoundBuffer("loose.wav");
    static Sound looseSound = new Sound(looseBuffer);
    public CircleShape Shape { get; set; }
    private Vector2f Velocity { get; set; }
    public float Speed { get; set; } = 200f;
    private float Radius { get; set; } = 8f;

    private float fireColorTimer = 0f;

    public void AnimateFireColor(float deltaTime)
    {
        fireColorTimer += deltaTime * 6f;
        float t = (MathF.Sin(fireColorTimer) + 1f) / 2f; 

        byte r = 255;
        byte g = (byte)(180 + t * 50); 
        byte b = 0;

        Shape.FillColor = new Color(r, g, b);
    }

    public Ball(Vector2f startPosition, Vector2f direction)
    {
        Velocity = Normalize(direction) * Speed;
        Shape = new CircleShape(Radius)
        {
            FillColor = Color.Cyan,
            Position = startPosition,
            Origin = new Vector2f(Radius, Radius)
        };
    }

    public void Update(float deltaTime)
    {
        Shape.Position += Velocity * deltaTime;
        
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        Velocity = Normalize(Velocity) * Speed * multiplier;
    }

    public void CheckWallCollision(Vector2u windowSize )
    {
        var pos = Shape.Position;


        if (pos.X - Radius <= 8f)
        {
            Velocity = new Vector2f(MathF.Abs(Velocity.X), Velocity.Y);
            Shape.Position = new Vector2f(8f + Radius + 0.5f, pos.Y);
            bordsSound.Play();
        }
        else if (pos.X + Radius >= windowSize.X - 8f)
        {
            Velocity = new Vector2f(-MathF.Abs(Velocity.X), Velocity.Y);
            Shape.Position = new Vector2f(windowSize.X - 8f - Radius - 0.5f, pos.Y);
            bordsSound.Play();
        }


        if (pos.Y - Radius <= 8f)
        {
            Velocity = new Vector2f(Velocity.X, MathF.Abs(Velocity.Y)); 
            Shape.Position = new Vector2f(pos.X, 8f + Radius + 0.5f);
        }


    }

    public void CheckPaddleCollision(RectangleShape paddle)
    {
        if (Shape.GetGlobalBounds().Intersects(paddle.GetGlobalBounds()))
        {
            float paddleCenter = paddle.Position.X + paddle.Size.X / 2f;
            float offset = (Shape.Position.X - paddleCenter) / (paddle.Size.X / 2f);
            float angle = offset * MathF.PI / 3f;

            Velocity = Normalize(new Vector2f(MathF.Sin(angle), -MathF.Cos(angle))) * Speed;
            bounceSound.Play();
        }
    }

    public int CheckBrickCollisions(List<Brick> bricks, List<Bonus> bonuses,bool isFireBallActive,int score , bool isDoubleScoreActive)
    {
        
        var warpBricks = bricks.Where(b => b.Type == "warp" && !b.IsDestroyed).ToList();
        foreach (var brick in bricks.ToList())
        {
            
            if (Shape.GetGlobalBounds().Intersects(brick.Shape.GetGlobalBounds()))
            {
                if (!isFireBallActive) bounceSound.Play();
                brick.Hit();
                if (brick.Type == "warp" && warpBricks.Count == 2)
                {
                    var current = brick;
                    var target = warpBricks.First(b => b != current);

                    Shape.Position = target.Shape.Position + new Vector2f(0, -Radius * 2);

                    break;
                }
                if (brick.IsDestroyed && brick.Health == 0 && !brick.explosionEffect.IsFinished)
                {
                    score += isDoubleScoreActive ? 200 : 100;
                    if (new Random().NextDouble() < 1)
                    {
                        var type = (BonusType)new Random().Next(Enum.GetValues(typeof(BonusType)).Length);
                        Font font = new Font("arial.ttf");
                        bonuses.Add(new Bonus(brick.Shape.Position, type, font));
                    }

                }
                if (!isFireBallActive)
                {
                    FloatRect ballBounds = Shape.GetGlobalBounds();
                    FloatRect brickBounds = brick.Shape.GetGlobalBounds();

                    Vector2f ballCenter = new Vector2f(ballBounds.Left + ballBounds.Width / 2f, ballBounds.Top + ballBounds.Height / 2f);
                    Vector2f brickCenter = new Vector2f(brickBounds.Left + brickBounds.Width / 2f, brickBounds.Top + brickBounds.Height / 2f);

                    Vector2f delta = ballCenter - brickCenter;
                    Vector2f absDelta = new Vector2f(MathF.Abs(delta.X), MathF.Abs(delta.Y));

                    float overlapX = (brickBounds.Width / 2f + Radius) - absDelta.X;
                    float overlapY = (brickBounds.Height / 2f + Radius) - absDelta.Y;

                    if (overlapX > 0 && overlapY > 0) 
                    {
                        if (overlapX < overlapY)
                        {
                         
                            Velocity = new Vector2f(-Velocity.X, Velocity.Y);
                            Shape.Position += new Vector2f(MathF.Sign(delta.X) * overlapX, 0f); 
                        }
                        else
                        {
                          
                            Velocity = new Vector2f(Velocity.X, -Velocity.Y);
                            Shape.Position += new Vector2f(0f, MathF.Sign(delta.Y) * overlapY); 
                        }
                    }
                }

                if (!isFireBallActive)
                    return score;

            }
        }
        return score;
    }

    private Vector2f Normalize(Vector2f v)
    {
        float len = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        return len > 0 ? v / len : new Vector2f(0, -1);
    }

    public bool IsOutOfBounds(uint windowHeight)
    {
        return Shape.Position.Y > windowHeight;
    }
}
