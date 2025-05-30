using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;


public class sfx
{
    public CircleShape Shape { get; set; }
    public float Lifetime = 0f;
    public float maxLifetime = 0f;
    public Vector2f Velocity = new();
    public float Elapsed = 0f;
    public bool IsFinished => Elapsed >= Lifetime;

    public float Radius;
}
public class FlashComet : sfx
{
    public FlashComet(Vector2f position, float radius, Vector2f direction, float speed , float lifetime)
    {
        Radius = radius; 

        Lifetime = lifetime; 

        Shape = new CircleShape(radius)
        {
            FillColor = new Color(255, 255, 0),
            Origin = new Vector2f(radius, radius),
            Position = position
        };

        Velocity = Normalize(direction) * speed;
        maxLifetime = Lifetime;
    }

    public void Update(float dt)
    {
        Lifetime -= dt;
        Shape.Position += Velocity * dt;

 
        float t = Lifetime / maxLifetime;
        Shape.FillColor = new Color(255, (byte)(200 * t), 0, (byte)(255 * t));
    }

    public void Draw(RenderWindow window)
    {
        if (Lifetime > 0)
            window.Draw(Shape);
    }

    public bool IsDead => Lifetime <= 0;

    private Vector2f Normalize(Vector2f v)
    {
        float len = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        return len == 0 ? new Vector2f(1, 0) : v / len;
    }
}
public class ExplosionCircle : sfx
{
    
    public ExplosionCircle(Vector2f center, float radius = 20f)
    {
        Lifetime = 0.25f;
        Shape = new CircleShape(radius)
        {
            Origin = new Vector2f(radius, radius),
            Position = center,
            FillColor = new Color(255, 140, 0) 
        };
    }

    public void Update(float dt)
    {
        Lifetime -= dt;
        float t = 1f - Lifetime;
        Shape.Radius = 20f + t * 10f;
        Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        byte alpha = (byte)(255 * (1 - t));
        Shape.FillColor = new Color(255, (byte)(140 + 115 * t), 0, alpha);
    }

    public void Draw(RenderWindow window)
    {
        window.Draw(Shape);
    }

    
}
public class ExplosionEffect : sfx
{
    
    
    private float maxRadius;
    private Vector2f center;
    

    public ExplosionEffect(Vector2f position, float size)
    {
        Lifetime = 0.4f;
        center = position;
        maxRadius = size * 1.5f;

        Shape = new CircleShape(10)
        {
            FillColor = new Color(255, 100, 0, 180),
            Origin = new Vector2f(0, 0),
            Position = center
        };
    }
    public void Update(float deltaTime)
    {
        Elapsed += deltaTime;
        float t = Elapsed / Lifetime;

        float radius = maxRadius * t;
        Shape.Radius = radius;
        Shape.Origin = new Vector2f(radius, radius);
        Shape.Position = center;

        byte alpha = (byte)(180 * (1 - t));
        Shape.FillColor = new Color(255, 100, 0, alpha);
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(Shape);
    }
}