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
