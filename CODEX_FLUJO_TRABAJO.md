# Contexto de trabajo para Codex

Ultima actualizacion: 2026-05-07

Este archivo es la memoria de trabajo del proyecto. Cada vez que David diga
"soy la Persona 1" o "soy David", Codex debe revisar este documento antes de
proponer cambios, tocar scripts o decidir responsabilidades.

## Equipo

| Persona | Nombre | Sistema principal | Que hace |
| --- | --- | --- | --- |
| Persona 1 | David | Jugadores | Movimiento, salto, gravedad, vida, dano, muerte, controles WASD/IJKL, direccion de lanzamiento y animaciones del jugador. |
| Persona 2 | Por definir | Bombas | Lanzamiento, carga de fuerza, bomba parabolica, bomba pegajosa, bomba resorte/oscilante, bomba rebotadora, explosion, dano y empuje. |
| Persona 3 | Por definir | Pickups, UI y flujo del juego | Pickups que cambian bomba, mapa, spawn, barra de vida, pantalla de inicio, victoria, derrota y reinicio. |

## Regla principal para David

David es responsable del sistema del jugador. Cuando trabaje como Persona 1, el
codigo debe mantenerse centrado en jugador/movimiento/vida/controles y solo
conectar con Bombas o UI mediante contratos simples.

## Flujo mejorado entre los tres sistemas

1. Persona 3 detecta el pickup.
2. Persona 3 cambia el tipo de bomba disponible del jugador.
3. Persona 1 guarda el tipo de bomba actual en el jugador o en un componente del jugador.
4. Persona 1 calcula direccion de lanzamiento desde WASD/IJKL u otro esquema definido.
5. Persona 1 calcula carga/fuerza de lanzamiento.
6. Persona 1 llama al sistema de Persona 2 con tipo de bomba, direccion, fuerza y origen.
7. Persona 2 crea y controla la bomba.
8. Persona 2 detecta explosion, dano y empuje.
9. Persona 2 avisa a Persona 1 que un jugador recibio dano/empuje.
10. Persona 1 aplica vida, dano, muerte e invulnerabilidad si existe.
11. Persona 1 notifica cambios de vida/muerte.
12. Persona 3 actualiza UI, victoria/derrota y reinicio.

## Contratos entre sistemas

### De Persona 1 a Persona 2

- `Transform` o posicion de origen del lanzamiento.
- Direccion normalizada.
- Fuerza de lanzamiento.
- Tipo de bomba actual.
- Referencia del jugador que lanza.

### De Persona 2 a Persona 1

- Cantidad de dano.
- Vector de empuje.
- Punto de explosion.
- Referencia del jugador afectado.

### De Persona 3 a Persona 1

- Tipo de bomba conseguido por pickup.
- Ordenes globales como iniciar, pausar, reiniciar o bloquear controles.

### De Persona 1 a Persona 3

- Vida actual.
- Vida maxima.
- Evento de dano recibido.
- Evento de muerte.
- Estado de jugador vivo/muerto.

## Limites de responsabilidad

- Persona 1 no decide como se comporta internamente cada bomba.
- Persona 2 no decide si el jugador gana, pierde o cambia de pantalla.
- Persona 3 no calcula fisica del jugador ni fisica de la bomba.
- El jugador puede pedir lanzar una bomba, pero el comportamiento de esa bomba pertenece a Persona 2.
- La UI puede mostrar vida, pero la vida real pertenece al jugador.

## Proceso cuando David vaya a trabajar

Cuando David diga que es la Persona 1:

1. Leer este archivo.
2. Revisar scripts de jugador, movimiento, vida, dano, controles o lanzamiento.
3. Confirmar si la tarea toca solo Persona 1 o si necesita contrato con Persona 2/3.
4. Hacer cambios pequenos y verificables.
5. Evitar modificar Bombas, Pickups, UI o GameManager salvo que sea necesario para conectar el flujo.
6. Registrar aqui cualquier proceso nuevo que David quiera repetir.

## Checklist antes de modificar scripts de jugador

- Confirmar que el cambio pertenece al sistema Jugadores.
- Revisar si ya existe un script de movimiento, vida, dano o lanzamiento.
- No duplicar una responsabilidad que ya tenga Bombas, Pickup o GameManager.
- Mantener separados los controles de cada jugador.
- Usar nombres claros para variables de velocidad, salto, vida, dano y direccion.
- Evitar valores magicos si el valor debe configurarse desde Unity Inspector.

## Checklist despues de modificar scripts de jugador

- El jugador se mueve correctamente.
- El salto y la gravedad siguen funcionando.
- La direccion de lanzamiento coincide con los controles.
- El dano baja vida una sola vez por impacto esperado.
- La muerte no se ejecuta varias veces.
- Los eventos de vida/muerte pueden ser escuchados por UI o GameManager.
- No se rompio el contrato con Bombas ni Pickups.

## Nombres recomendados

- `Movimiento` o `PlayerMovement`: movimiento, salto y gravedad.
- `PlayerHealth`: vida, dano y muerte.
- `PlayerBombInventory`: tipo de bomba actual del jugador.
- `PlayerBombAim`: direccion y carga del lanzamiento.
- `BombLauncher`: punto de conexion con el sistema de bombas.
- `BombPickup`: pickup que cambia la bomba disponible.
- `GameManager`: victoria, derrota, reinicio y estado global.
- `HealthBar`: representacion visual de la vida.

## Procesos registrados

### 2026-05-07 - Contexto inicial de David

- David queda registrado como Persona 1.
- David trabaja en el sistema Jugadores.
- El flujo se divide en contratos entre Jugadores, Bombas, Pickups/UI y GameManager.
- Este archivo se usara como memoria de trabajo en futuras tareas.

### 2026-05-07 - Movimiento local para dos jugadores

- El movimiento debe estar en un componente propio de Persona 1: `Movimiento`.
- Jugador 1 usa WASD: A izquierda, D derecha, W salto/subir, S abajo/caida rapida.
- Jugador 2 usa flechas: izquierda, derecha, arriba salto/subir, abajo caida rapida.
- `PlayerTest` queda solo como inventario temporal de bombas para no duplicar movimiento.
- Cada jugador debe tener `Rigidbody2D`, `Collider2D` y un `Movimiento` con su esquema correcto.
