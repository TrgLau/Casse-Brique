using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;

public class Brick
{
    public ExplosionEffect explosionEffect;
    public RectangleShape Shape { get; private set; }
    public int Health { get; private set; }

    public string Type { get; private set; } = "normal";
    
    public bool CanBeRemoved()
    {
        return IsDestroyed && (explosionEffect == null || explosionEffect.IsFinished);
    }
    public void Draw(RenderWindow window)
    {
        if (!IsDestroyed)
            window.Draw(Shape);

        if (explosionEffect != null && !explosionEffect.IsFinished)
            explosionEffect.Draw(window);
    }
    public Brick(Vector2f position, Vector2f size, int health, string type = "normal")
    {
        Type = type;
        Health = health;
        Shape = new RectangleShape(size)
        {
            Position = position,
            FillColor = GetColorByHealth(health),
            OutlineThickness = 1f,
            OutlineColor = Color.Black
        };
    }


    public bool Hit()
    {
        Health--;
        UpdateColor();

        if (Health <= 0 && explosionEffect == null)
        {
            explosionEffect = new ExplosionEffect(Shape.Position, Shape.Size.X / 2);
            return true;
        }
        else return false;
    }

    public bool IsDestroyed => Health <= 0;

    public void Update(float deltaTime)
    {
        explosionEffect?.Update(deltaTime);
    }


    private void UpdateColor()

    {
        Shape.FillColor = GetColorByHealth(Health);
        if (Type == "warp")
        {
            Shape.FillColor = new Color(100, 100, 255);
            return;
        }
    }

    private Color GetColorByHealth(int health)
    {
        return health switch
        {
            >= 3 => Color.Blue,
            2 => Color.Green,
            1 => Color.Yellow,
            _ => new Color(30, 30, 30, 100)
        };
    }
}
public class LevelData
{
    public List<Level> levels { get; set; }
}
public class Level
{
    public List<BrickData> bricks { get; set; }
}
public class BrickData
{
    public float x { get; set; }
    public float y { get; set; }
    public float width { get; set; }
    public float height { get; set; }
    public int health { get; set; }

    public string type { get; set; }
}
public static class LevelLoader
{
    public static List<Brick> LoadLevel(string jsonPath, int levelIndex)
    {
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException("Fichier de niveau introuvable");

        string json = File.ReadAllText(jsonPath);
        var levelData = JsonSerializer.Deserialize<LevelData>(json);

        if (levelData == null || levelIndex >= levelData.levels.Count)
            throw new ArgumentException("Niveau inexistant");

        var bricks = new List<Brick>();

        foreach (var brickInfo in levelData.levels[levelIndex].bricks)
        {
            var pos = new Vector2f(brickInfo.x, brickInfo.y);
            var size = new Vector2f(brickInfo.width, brickInfo.height);
            var health = brickInfo.health;
            string type = brickInfo.type ?? "normal";
            bricks.Add(new Brick(pos, size, health, type));
        }

        return bricks;
    }
}