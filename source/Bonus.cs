using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;


public enum BonusType
{
    PaddleGrow,
    PaddleShrink,
    ExtraBall,
    SlowBall,
    SpeedBall,
    DoubleScore,
    Laser,
    FireBall,
}
public class Bonus
{
    public RectangleShape Shape;
    public BonusType Type;
    public bool IsActive = true;

    private float speed = 100f;
    private float oscillationTimer = 0f;
    private float flickerTimer = 0f;
    private float flickerSpeed;
    private byte baseAlpha;
    private Text label;
    private Vector2f initialPosition;

    public Bonus(Vector2f position, BonusType type, Font font)
{
    Type = type;

    Shape = new RectangleShape(new Vector2f(20, 20))
    {
        Position = position,
        FillColor = GetColorByType(type),
        Origin = new Vector2f(10, 10)
    };

    baseAlpha = Shape.FillColor.A;
    initialPosition = position;
    flickerSpeed = new Random().Next(2, 5);

    label = new Text(GetLetter(type), font, 14)
    {
        FillColor = Color.Black,
        OutlineThickness = 1f,
        OutlineColor = Color.White,
        Origin = new Vector2f(5, 7), 
        Position = Shape.Position
    };
}
private string GetLetter(BonusType type)
{
    return type switch
    {
        BonusType.PaddleGrow => "G",
        BonusType.PaddleShrink => "S",
        BonusType.ExtraBall => "M",
        BonusType.SlowBall => "L",
        BonusType.SpeedBall => "V",
        BonusType.DoubleScore => "D",
        BonusType.Laser => "A",
        BonusType.FireBall => "F",
        
        _ => "?"
    };
}
    public void Update(float deltaTime)
    {
        if (!IsActive) return;
        label.Position = Shape.Position;
        

        Shape.Position += new Vector2f(0, speed * deltaTime);

    
        oscillationTimer += deltaTime;
        float offsetX = MathF.Sin(oscillationTimer * 5f) * 10f;
        Shape.Position = new Vector2f(initialPosition.X + offsetX, Shape.Position.Y);

 
        flickerTimer += deltaTime * flickerSpeed;
        float alphaOffset = MathF.Sin(flickerTimer) * 50f;
        byte alpha = (byte)Math.Clamp(baseAlpha + alphaOffset, 60, 255);
        var c = Shape.FillColor;
        Shape.FillColor = new Color(c.R, c.G, c.B, alpha);
    }
    public void Draw(RenderWindow window)
    {
        if (IsActive)
        {
            window.Draw(Shape);
            window.Draw(label);
        }
    }
    public bool Intersects(RectangleShape paddle)
    {
        return IsActive && Shape.GetGlobalBounds().Intersects(paddle.GetGlobalBounds());
    }

    private Color GetColorByType(BonusType type)
    {
        return type switch
        {
            BonusType.PaddleGrow => new Color(0, 255, 0, 200),
            BonusType.PaddleShrink => new Color(255, 0, 0, 200),
            BonusType.ExtraBall => new Color(255, 255, 0, 200),
            BonusType.SlowBall => new Color(0, 255, 255, 200),
            BonusType.SpeedBall => new Color(255, 0, 255, 200),
            BonusType.DoubleScore => new Color(255, 215, 0, 200),
            BonusType.Laser => new Color(255, 80, 80, 200),
            BonusType.FireBall => new Color(255, 140, 0, 200),
            _ => new Color(255, 255, 255, 200),
        };
    }
}