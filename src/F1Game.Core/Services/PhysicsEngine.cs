using F1Game.Core.Models;

namespace F1Game.Core.Services;

public class PhysicsEngine
{
    private const float FrictionCoefficient = 0.05f;
    private const float AirResistanceBase = 0.0002f;
    private const float BrakingForce = 80.0f;

    public void Update(Car car, float deltaTime, float inputGas, float inputSteering, float inputBrake)
    {
        if (deltaTime <= 0) return;

        // 1. Calcular aceleración/frenado
        float accelerationApplied = 0;

        if (inputBrake > 0)
        {
            accelerationApplied -= BrakingForce * inputBrake;
        }
        else
        {
            accelerationApplied = car.BaseAcceleration * inputGas;
        }

        // 2. Aplicar resistencia al aire (drag) y fricción (fuerzas opuestas al movimiento)
        float drag = AirResistanceBase * car.CurrentSpeed * car.CurrentSpeed;
        float friction = car.CurrentSpeed * FrictionCoefficient;
        
        float resistance = drag + friction;
        if (car.CurrentSpeed < 0.1f && inputGas <= 0) resistance = 0; // Evitar que retroceda por fricción

        // 3. Actualizar velocidad
        car.CurrentSpeed += (accelerationApplied - (car.CurrentSpeed > 0 ? resistance : -resistance)) * deltaTime;

        // Limitar velocidad máxima
        if (car.CurrentSpeed > car.MaxSpeed) 
            car.CurrentSpeed = car.MaxSpeed;
        
        if (car.CurrentSpeed < 0) 
            car.CurrentSpeed = 0;

        // 4. Actualizar posición longitudinal (Progreso)
        // Convertimos km/h a m/s si fuera necesario, pero aquí asumimos unidades consistentes o m/s para simplicidad
        car.Position = car.Position with 
        { 
            Progress = car.Position.Progress + (car.CurrentSpeed * deltaTime) 
        };

        // 5. Actualizar posición lateral (Handling)
        // El giro depende de la velocidad (a muy alta velocidad es más difícil girar bruscamente)
        float steeringSensitivity = car.BaseHandling * (1.0f - (car.CurrentSpeed / (car.MaxSpeed * 1.2f)));
        float lateralMovement = inputSteering * steeringSensitivity * 10.0f * deltaTime;
        
        car.Position = car.Position with 
        { 
            LateralOffset = car.Position.LateralOffset + lateralMovement 
        };
    }
}
