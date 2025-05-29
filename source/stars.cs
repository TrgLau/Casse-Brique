using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;



public class BackGrndStar
{
    public CircleShape Shape;
    public float Speed;
    public float BaseAlpha;
    private float flickerTimer;
    private float flickerSpeed;

    public BackGrndStar(float x, float y, float size, float speed, byte alpha)
    {
        Shape = new CircleShape(size)
        {
            Position = new Vector2f(x, y),
            FillColor = new Color(255, 255, 255, alpha)
        };

        Speed = speed;
        BaseAlpha = alpha;
        flickerTimer = 0f;
        flickerSpeed = new Random().Next(1, 4);
    }

    public void Update(float deltaTime, float windowHeight, float intensity = 1.0f)
    {
        Shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y + Speed * deltaTime * intensity);

        if (Shape.Position.Y > windowHeight)
        {
            Random rand = new();
            Shape.Position = new Vector2f(rand.Next(0, 800), -10);
        }

        flickerTimer += deltaTime * flickerSpeed * intensity;
        float alphaOffset = (float)Math.Sin(flickerTimer) * 30f * intensity;
        byte newAlpha = (byte)Math.Clamp(BaseAlpha + alphaOffset, 50, 255);
        Shape.FillColor = new Color(255, 255, 255, newAlpha);
    }
}
public class Comet
{
    public CircleShape Head;
    private List<CircleShape> trail = new();
    private int trailLength = 100;

    private Vector2f velocity;
    private float speed = 60f; 
    private static Random rand = new();

    public Comet(Vector2f startPos, Vector2f direction, float radius = 6f)
    {
        Head = new CircleShape(radius)
        {
            FillColor = new Color(255, 140, 0), // orange incandescent
            Position = startPos,
            Origin = new Vector2f(radius, radius)
        };

        velocity = Normalize(direction) * speed;
        
        for (int i = 0; i < trailLength; i++)
        {
            float t = i / (float)trailLength;

            // Orange → Jaune
            byte r = 255;
            byte g = (byte)(140 + t * 115); // 140 → 255
            byte b = 0;
            byte alpha = (byte)(180 - t * 160); // plus transparent vers la fin

            var tShape = new CircleShape(radius)
            {
                FillColor = new Color(r, g, b, alpha),
                Radius = radius,
                Origin = new Vector2f(radius, radius),
                Position = startPos
            };
            trail.Add(tShape);
        }
    }
    public bool IsOffScreen(Vector2u windowSize)
    {
            return Head.Position.X < -100 || Head.Position.Y < -100 || Head.Position.Y > windowSize.Y + 100;
    }
    public void Update(float deltaTime)
    {
        Vector2f newPos = Head.Position + velocity * deltaTime;

       
        for (int i = trailLength - 1; i > 0; i--)
            trail[i].Position = trail[i - 1].Position;

        trail[0].Position = Head.Position;
        Head.Position = newPos;

       
        if (Head.Position.X < -100 || Head.Position.Y > 700 || Head.Position.Y < -100)
            Reset();
    }

    public void Draw(RenderWindow window)
    {
        foreach (var t in trail)
            window.Draw(t);

        window.Draw(Head);
    }

    private void Reset()
    {
        Vector2f start = new Vector2f(900, rand.Next(50, 200));
        Vector2f dir = new Vector2f(-1f, 0.5f + (float)(rand.NextDouble() * 0.1 - 0.05));

        Head.Position = start;
        velocity = Normalize(dir) * speed;

        for (int i = 0; i < trail.Count; i++)
        {
            trail[i].Position = start;
        }
    }

    private Vector2f Normalize(Vector2f v)
    {
        float len = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        return len == 0 ? new Vector2f(-1, 0) : v / len;
    }
}
