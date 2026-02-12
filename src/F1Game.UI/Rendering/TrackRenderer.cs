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

    public void DrawFrame(List<ProjectedPolygon> polygons, float steeringInput, IEnumerable<AICar> opponents, Pseudo3DCamera camera, Car player)
    {
        // 1. Dibujar el fondo (Cielo)
        Raylib.ClearBackground(Color.SKYBLUE);

        foreach (var poly in polygons)
        {
            bool isAlternate = (poly.SegmentIndex / 3) % 2 == 0;
            
            Color grassColor = isAlternate ? ColorGrassLight : ColorGrassDark;
            Color roadColor = isAlternate ? ColorRoadLight : ColorRoadDark;
            Color rumbleColor = isAlternate ? ColorRumbleRed : ColorRumbleWhite;

            Raylib.DrawRectangle(0, (int)poly.UpperLeft.Y, Raylib.GetScreenWidth(), (int)(poly.LowerLeft.Y - poly.UpperLeft.Y) + 1, grassColor);
            DrawTrapezoid(poly.LowerLeft, poly.LowerRight, poly.UpperLeft, poly.UpperRight, roadColor);
            DrawRumbleStrips(poly, rumbleColor);
        }

        // 2. Dibujar Oponentes (AI)
        foreach (var ai in opponents)
        {
            var sp = camera.ProjectSprite(ai.Position.Progress, ai.Position.LateralOffset * 20.0f, player, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), 15, 0.7f);
            if (sp.Scale > 0 && sp.Scale < 1.0f) // Solo si está frente a nosotros y no demasiado cerca
            {
                DrawOpponent(sp, ai);
            }
        }

        DrawPlayerCar(steeringInput);
    }

    private void DrawOpponent(ScreenPoint sp, AICar ai)
    {
        int w = (int)(80 * sp.Scale);
        int h = (int)(40 * sp.Scale);
        Raylib.DrawRectangle((int)sp.X - w / 2, (int)sp.Y - h, w, h, Color.BLUE);
        Raylib.DrawRectangle((int)sp.X - (w / 2) + 2, (int)sp.Y - h - (h / 2), w - 4, h / 2, Color.DARKBLUE);
        Raylib.DrawText("AI", (int)sp.X - 5, (int)sp.Y - h - 15, (int)(10 * sp.Scale + 5), Color.WHITE);
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

        int carWidth = 100;
        int carHeight = 50;
        int carX = (screenWidth / 2) - (carWidth / 2);
        int carY = screenHeight - 120;

        int tilt = (int)(steering * 30);
        
        // Sombra
        Raylib.DrawEllipse(carX + carWidth / 2 + tilt, carY + carHeight - 5, 50, 15, Raylib.Fade(Color.BLACK, 0.4f));

        // Cuerpo Principal (F1)
        Raylib.DrawRectangle(carX + tilt, carY, carWidth, carHeight, Color.MAROON);
        Raylib.DrawRectangle(carX + tilt + 10, carY - 20, carWidth - 20, 25, Color.BLACK); // Cabina
        
        // Alerón trasero
        Raylib.DrawRectangle(carX + tilt - 5, carY + 5, 10, 30, Color.BLACK);
        Raylib.DrawRectangle(carX + tilt + carWidth - 5, carY + 5, 10, 30, Color.BLACK);
        
        Raylib.DrawText("CHEKO PEREZ", carX + tilt + 10, carY + 60, 12, Color.GOLD);
    }
}
}
