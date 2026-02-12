using F1Game.Core.Models;

namespace F1Game.Core.Rendering;

public struct ScreenPoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Scale { get; set; }
}

public class ProjectedPolygon
{
    public ScreenPoint LowerLeft { get; set; }
    public ScreenPoint LowerRight { get; set; }
    public ScreenPoint UpperLeft { get; set; }
    public ScreenPoint UpperRight { get; set; }
    public int SegmentIndex { get; set; }
}

public class Pseudo3DCamera
{
    private const float TrackWidth = 20.0f; // Ancho constante de la pista en unidades de mundo

    public List<ProjectedPolygon> Project(
        Track track, 
        Car car, 
        int screenWidth, 
        int screenHeight, 
        float cameraHeight, 
        float cameraDepth,
        int drawDistance = 300)
    {
        var projectedTrack = new List<ProjectedPolygon>();
        float playerProgress = car.Position.Progress;
        float playerX = car.Position.LateralOffset;

        // Encontrar el índice del segmento actual basado en el progreso
        int startSegmentIndex = FindSegmentIndexAtProgress(track, playerProgress);
        
        float currentZ = 0; // Z relativo a la cámara (distancia al frente)
        float currentXOffset = 0; // Acumulador de curvas para el efecto visual
        float currentYOffset = 0; // Acumulador de inclinación (colinas)

        for (int i = 0; i < drawDistance; i++)
        {
            int idx = (startSegmentIndex + i) % track.Segments.Count;
            var segment = track.Segments[idx];

            // Propiedades del segmento actual (Z relativo)
            float z1 = currentZ;
            float z2 = currentZ + segment.Length;

            // Evitar división por cero o segmentos detrás de la cámara
            if (z1 <= 0) z1 = 0.1f;

            // Proyección básica estilo OutRun
            // Scale = CameraDepth / Z
            float scale1 = cameraDepth / z1;
            float scale2 = cameraDepth / z2;

            // Proyectar puntos
            var poly = new ProjectedPolygon
            {
                SegmentIndex = idx,
                LowerLeft = ProjectPoint(z1, scale1, -1, currentXOffset, currentYOffset, playerX, cameraHeight, screenWidth, screenHeight),
                LowerRight = ProjectPoint(z1, scale1, 1, currentXOffset, currentYOffset, playerX, cameraHeight, screenWidth, screenHeight),
                UpperLeft = ProjectPoint(z2, scale2, -1, currentXOffset, currentYOffset, playerX, cameraHeight, screenWidth, screenHeight),
                UpperRight = ProjectPoint(z2, scale2, 1, currentXOffset, currentYOffset, playerX, cameraHeight, screenWidth, screenHeight)
            };

            projectedTrack.Add(poly);

            // Avanzar acumuladores para el siguiente segmento
            currentZ = z2;
            // Aquí simularíamos la curvatura basándonos en la geometría 2D del Paso 2
            // Para simplificar el efecto visual, usamos la diferencia de ángulo/posición
            currentXOffset += CalculateCurvature(segment); 
            currentYOffset += segment.Inclination * segment.Length;

            if (currentZ > 2000) break; // Límite de visión lejana
        }

        // Ordenamos de atrás hacia adelante (Painter's Algorithm)
        projectedTrack.Reverse();
        return projectedTrack;
    }

    private ScreenPoint ProjectPoint(
        float z, float scale, float side, 
        float worldXOffset, float worldYOffset, 
        float playerX, float cameraHeight,
        int width, int height)
    {
        float centerX = width / 2.0f;
        float centerY = height / 2.0f;

        // Posición X en pantalla: centro + (escala * (desplazamiento lateral - posición jugador))
        float x = centerX + (scale * (side * TrackWidth + worldXOffset - playerX) * centerX);
        
        // Posición Y en pantalla: centro - (escala * (elevación - altura cámara))
        // En OutRun, Y crece hacia arriba visualmente, pero en coordenadas de pantalla suele ser hacia abajo
        float y = centerY - (scale * (worldYOffset - cameraHeight) * centerY);

        return new ScreenPoint { X = x, Y = y, Scale = scale };
    }

    public ScreenPoint ProjectSprite(
        float worldProgress, 
        float worldX, 
        Car player, 
        int screenWidth, 
        int screenHeight, 
        float cameraHeight, 
        float cameraDepth)
    {
        float relativeZ = worldProgress - player.Position.Progress;
        if (relativeZ <= 0) return new ScreenPoint { Scale = -1 }; // Detrás de la cámara

        float scale = cameraDepth / relativeZ;
        float centerX = screenWidth / 2.0f;
        float centerY = screenHeight / 2.0f;

        float x = centerX + (scale * (worldX - player.Position.LateralOffset) * centerX);
        float y = centerY - (scale * (0 - cameraHeight) * centerY); // Asumiendo altura mundo 0

        return new ScreenPoint { X = x, Y = y, Scale = scale };
    }

    private int FindSegmentIndexAtProgress(Track track, float progress)
    {
        float accumulated = 0;
        for (int i = 0; i < track.Segments.Count; i++)
        {
            accumulated += track.Segments[i].Length;
            if (accumulated >= progress) return i;
        }
        return 0;
    }

    private float CalculateCurvature(TrackSegment segment)
    {
        // Deriva la curvatura de la dirección del vector en el espacio 2D
        var dir = segment.EndPoint - segment.StartPoint;
        return dir.X * 0.1f; // Factor de exageración visual
    }
}
