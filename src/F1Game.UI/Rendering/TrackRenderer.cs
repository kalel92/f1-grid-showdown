using F1Game.Core.Rendering;
using F1Game.Core.Models;

namespace F1Game.UI.Rendering;

public class TrackRenderer
{
    // Colores tipo OutRun (Hexadecimal ficticio para representación)
    private const string ColorGrassLight = "#348C31";
    private const string ColorGrassDark = "#2A7028";
    private const string ColorRoadLight = "#424242";
    private const string ColorRoadDark = "#333333";
    private const string ColorRumbleRed = "#FF0000";
    private const string ColorRumbleWhite = "#FFFFFF";

    public void DrawFrame(List<ProjectedPolygon> polygons)
    {
        foreach (var poly in polygons)
        {
            // Lógica de alternancia: cada 3 segmentos cambiamos el color
            bool isAlternate = (poly.SegmentIndex / 3) % 2 == 0;
            
            string grassColor = isAlternate ? ColorGrassLight : ColorGrassDark;
            string roadColor = isAlternate ? ColorRoadLight : ColorRoadDark;
            string rumbleColor = isAlternate ? ColorRumbleRed : ColorRumbleWhite;

            DrawTrapezoid(poly.LowerLeft, poly.LowerRight, poly.UpperLeft, poly.UpperRight, roadColor);
            
            // Aquí se dibujarían los bordes de la pista (Rumble strips)
            DrawRumbleStrips(poly, rumbleColor);
        }

        DrawPlayerCar();
    }

    private void DrawTrapezoid(ScreenPoint bl, ScreenPoint br, ScreenPoint ul, ScreenPoint ur, string color)
    {
        // En un motor real (GDI+, SFML, Raylib):
        // Graphics.FillPolygon(new Point[] { bl, br, ur, ul }, color);
    }

    private void DrawRumbleStrips(ProjectedPolygon poly, string color)
    {
        // El ancho del rumble strip es proporcional al scale
        float width = 20.0f * poly.LowerLeft.Scale;
        // Dibuja rectángulos proyectados a los lados de la pista
    }

    private void DrawPlayerCar()
    {
        // Posición: Inferior Centro
        // Sprite: f1_car_straight.png, f1_car_left.png o f1_car_right.png basado en el input
        /*
        if (inputSteering < -0.1f) RenderSprite("car_left");
        else if (inputSteering > 0.1f) RenderSprite("car_right");
        else RenderSprite("car_straight");
        */
    }
}
