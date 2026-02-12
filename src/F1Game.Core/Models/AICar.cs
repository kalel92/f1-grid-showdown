using F1Game.Core.Models;

namespace F1Game.Core.Models;

public class AICar : Car
{
    public float IdealLateralOffset { get; set; } // La línea de carrera que intenta seguir
    public float Aggression { get; set; } // 0 a 1, qué tanto arriesga en frenado
    public float Skill { get; set; } // 0 a 1, qué tan perfecto es siguiendo la línea
}
