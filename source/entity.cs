using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;


public class Entity
{
    public CircleShape Shape;
    public bool IsAlive = true;
    private Vector2f velocity;
    private float speed = 60f;
    private float directionTimer = 0f;
    private static Random rand = new();
    private float spawnFlashTimer = 0.5f;
    
    public Entity(Vector2f position, float radius = 12f)
    {
        Shape = new CircleShape(radius)
        {
            FillColor = new Color(255, 0, 255),
            Position = position,
            Origin = new Vector2f(radius, radius)
        };

        velocity = GetRandomDirection() * speed;
    }
    
    public void Update(float deltaTime, Vector2u windowSize)
    {
        if (!IsAlive) return;

        if (spawnFlashTimer > 0f)
        {
            spawnFlashTimer -= deltaTime;

            float pulse = (float)(Math.Sin(spawnFlashTimer * 20f) + 1f) / 2f;
            Shape.FillColor = new Color(255, 255, 255, (byte)(255 * pulse));
        }
        else
        {

            if (Shape.FillColor != new Color(255, 0, 255))
                Shape.FillColor = new Color(255, 0, 255);
        }

        directionTimer -= deltaTime;
        if (directionTimer <= 0)
        {
            velocity = GetRandomDirection() * speed;
            directionTimer = (float)(0.5 + rand.NextDouble() * 1.5);
        }

        Vector2f pos = Shape.Position + velocity * deltaTime;

        if (pos.X < 0 + 8f || pos.X > windowSize.X - 8f)
            velocity.X *= -1;
        if (pos.Y < 0 + 8f || pos.Y > windowSize.Y)
            velocity.Y *= -1;

        Shape.Position += velocity * deltaTime;
    }

    public bool CheckCollision(Ball ball)
    {
        return IsAlive && Shape.GetGlobalBounds().Intersects(ball.Shape.GetGlobalBounds());
    }

    private Vector2f GetRandomDirection()
    {
        float angle = (float)(rand.NextDouble() * 2 * Math.PI);
        return new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));
    }
}