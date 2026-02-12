using F1Game.Core.Rendering;
using F1Game.Core.Models;
using Raylib_cs;
using System.Numerics;

namespace F1Game.UI.Rendering;

public class TrackRenderer
{
    private readonly Color ColorGrassLight = new(52, 140, 49, 255);
    private readonly Color ColorGrassDark = new(42, 112, 40, 255);
    private readonly Color ColorRoadLight = new(66, 66, 66, 255);
    private readonly Color ColorRoadDark = new(51, 51, 51, 255);
    private readonly Color ColorRumbleRed = new(255, 0, 0, 255);
    private readonly Color ColorRumbleWhite = new(255, 255, 255, 255);

    public void DrawFrame(List<ProjectedPolygon> polygons, float steeringInput)
    {
        // 1. Dibujar el fondo (Cielo)
        Raylib.ClearBackground(Color.SKYBLUE);

        foreach (var poly in polygons)
        {
            bool isAlternate = (poly.SegmentIndex / 3) % 2 == 0;
            
            Color grassColor = isAlternate ? ColorGrassLight : ColorGrassDark;
            Color roadColor = isAlternate ? ColorRoadLight : ColorRoadDark;
            Color rumbleColor = isAlternate ? ColorRumbleRed : ColorRumbleWhite;

            // Dibujar Pasto (Fondo completo del segmento)
            Raylib.DrawRectangle(0, (int)poly.UpperLeft.Y, Raylib.GetScreenWidth(), (int)(poly.LowerLeft.Y - poly.UpperLeft.Y) + 1, grassColor);

            // Dibujar Asfalto (Trapecio)
            DrawTrapezoid(poly.LowerLeft, poly.LowerRight, poly.UpperLeft, poly.UpperRight, roadColor);
            
            // Bordes (Rumble Strips)
            DrawRumbleStrips(poly, rumbleColor);
        }

        DrawPlayerCar(steeringInput);
    }

    private void DrawTrapezoid(ScreenPoint bl, ScreenPoint br, ScreenPoint ul, ScreenPoint ur, Color color)
    {
        Vector2[] vertices = {
            new(bl.X, bl.Y),
            new(br.X, br.Y),
            new(ur.X, ur.Y),
            new(ul.X, ul.Y)
        };
        Raylib.DrawTriangleFan(vertices, vertices.Length, color);
    }

    private void DrawRumbleStrips(ProjectedPolygon poly, Color color)
    {
        float rumbleWidth1 = 40.0f * poly.LowerLeft.Scale;
        float rumbleWidth2 = 40.0f * poly.UpperLeft.Scale;

        // Rumble Izquierdo
        Vector2[] leftRumble = {
            new(poly.LowerLeft.X - rumbleWidth1, poly.LowerLeft.Y),
            new(poly.LowerLeft.X, poly.LowerLeft.Y),
            new(poly.UpperLeft.X, poly.UpperLeft.Y),
            new(poly.UpperLeft.X - rumbleWidth2, poly.UpperLeft.Y)
        };
        Raylib.DrawTriangleFan(leftRumble, leftRumble.Length, color);

        // Rumble Derecho
        Vector2[] rightRumble = {
            new(poly.LowerRight.X, poly.LowerRight.Y),
            new(poly.LowerRight.X + rumbleWidth1, poly.LowerRight.Y),
            new(poly.UpperRight.X + rumbleWidth2, poly.UpperRight.Y),
            new(poly.UpperRight.X, poly.UpperRight.Y)
        };
        Raylib.DrawTriangleFan(rightRumble, rightRumble.Length, color);
    }

    private void DrawPlayerCar(float steering)
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        // En un prototipo, usamos un rectángulo para el auto
        int carWidth = 80;
        int carHeight = 40;
        int carX = (screenWidth / 2) - (carWidth / 2);
        int carY = screenHeight - 100;

        // Inclinación visual basada en steering
        int tilt = (int)(steering * 20);
        
        Raylib.DrawRectangle(carX + tilt, carY, carWidth, carHeight, Color.RED);
        Raylib.DrawRectangle(carX + tilt + 5, carY - 15, carWidth - 10, 20, Color.BLACK); // Alerón/Cabina
        
        Raylib.DrawText("PLAYER 1", carX + tilt, carY + 45, 10, Color.WHITE);
    }
}
