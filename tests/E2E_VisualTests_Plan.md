# Plan de Pruebas de Integración Visual (E2E) - F1 Game

## 1. Pruebas de Respuesta al Input
- **Test Inyección de Gas**: Mediante un script de automatización (`InputManagerMock`), establecer `gas = 1.0`.
  - *Resultado Esperado*: En 5 segundos, la `CurrentSpeed` debe pasar de 0 a > 200 km/h y los polígonos deben aumentar su `Scale` rápidamente.
- **Test Inyección de Dirección**: Establecer `steering = -1.0`.
  - *Resultado Esperado*: La propiedad `LateralOffset` del auto debe disminuir y el `TrackRenderer` debe alternar al sprite `car_left`.

## 2. Pruebas de Renderizado (Flicker-Free)
- **Buffer Swapping Check**: Verificar manualmente que el `DrawFrame` se complete antes de la siguiente señal de reloj del sw.
- **Z-Sorting Analysis**: Validar que el `ProjectedPolygon` devuelto por la cámara se procese en orden inverso (lejos a cerca) para evitar artefactos visuales donde la carretera se dibuja sobre el auto.

## 3. Pruebas de Performance
- **Frame Time Delta**: Registrar el tiempo de ejecución de `Update() + Render()`.
  - *Límite*: No debe exceder los 16.6ms (umbral para 60 FPS).
