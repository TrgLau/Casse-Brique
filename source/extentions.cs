
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using System.Text.Json;

public static class Vector2fExtensions
{
    public static float Length(this Vector2f v)
        => (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
}

