# Space Jump

Prototipo 3D en primera persona para el **Laboratorio de Construcción de Software** de la Tecnicatura Universitaria en Informática.

El proyecto es un laboratorio interactivo para experimentar con movimiento, física, colisiones y sincronización de jugadores. La experiencia propone recorrer un escenario con plataformas, obstáculos y elementos interactivos hasta alcanzar la meta. El prototipo puede ejecutarse de forma local o con varios jugadores conectados a una misma sala mediante Photon PUN.

## Estado del proyecto

El repositorio contiene actualmente:

- Personaje FPS controlable mediante `CharacterController`.
- Movimiento con aceleración, sprint, agacharse, salto y gravedad.
- Detección de pendientes, plataformas móviles y checkpoints.
- Daño por caída y recuperación mediante reaparición.
- Armas, proyectiles, pickups de salud, munición y jetpack.
- Escenario de experimentación en `PruebaEscenario`.
- Multiplayer con Photon PUN: conexión, sala compartida, instanciación y sincronización de jugadores.
- Condiciones de victoria y derrota mediante `WinScene` y `LoseScene`.
- Panel de diagnóstico de Photon visible con `F1`.

La calidad visual y la cantidad de contenido no son objetivos principales: el escenario está pensado para observar y validar el comportamiento técnico de las mecánicas.

## Tecnologías

- **Unity:** `6000.5.8f1`
- **Renderizado:** Universal Render Pipeline (URP)
- **Input:** Unity Input System `1.20.0`
- **Networking:** Photon Realtime / Photon Unity Networking (PUN)
- **Construcción del escenario:** ProBuilder
- **Navegación:** AI Navigation y NavMesh Components
- **Lenguaje:** C#

Las dependencias de Unity están declaradas en [`Packages/manifest.json`](Packages/manifest.json).

## Requisitos

- Unity Hub.
- Unity `6000.5.8f1` o una versión compatible de Unity 6.
- Acceso a Internet para probar el modo multiplayer.
- Una aplicación configurada en Photon Engine para el modo online.

## Cómo abrir el proyecto

1. Clonar el repositorio.
2. Abrir Unity Hub y seleccionar **Add > Add project from disk**.
3. Elegir la carpeta raíz del repositorio.
4. Abrir el proyecto con Unity `6000.5.8f1`.
5. Abrir la escena [`Assets/FPS/Scenes/MainMenu.unity`](Assets/FPS/Scenes/MainMenu.unity).
6. Ejecutar el proyecto con el botón **Play**.

Las escenas incluidas en la configuración de Build son:

| Escena | Uso |
| --- | --- |
| `MainMenu` | Menú de inicio y acceso al escenario. |
| `PruebaEscenario` | Escenario principal de experimentación y multiplayer. |
| `WinScene` | Resultado de victoria. |
| `LoseScene` | Resultado de derrota. |

## Configuración de Photon

El proyecto utiliza `PhotonNetwork.ConnectUsingSettings()` y carga la configuración desde `PhotonServerSettings`.

1. Crear o seleccionar una aplicación de tipo **Photon Realtime** en el panel de Photon.
2. En Unity, abrir `Window > Photon Unity Networking > Highlight Server Settings`.
3. Pegar el App ID en `AppIdRealtime`.
4. Verificar que `PhotonServerSettings` no quede excluido del proyecto.
5. Guardar los cambios y ejecutar desde `MainMenu`.

No publicar el App ID en documentación pública si el equipo utiliza una credencial propia. Para probar solo el movimiento local, puede ejecutarse la escena de escenario sin depender de una sesión multiplayer, siempre que la escena y el prefab estén configurados para ese modo.

## Cómo jugar

### Controles de teclado y mouse

| Acción | Control |
| --- | --- |
| Moverse | `W`, `A`, `S`, `D` o flechas |
| Mirar | Movimiento del mouse |
| Saltar | `Espacio` |
| Correr | `Shift izquierdo` |
| Agacharse | `C` |
| Interactuar | `E` |
| Disparar | Botón izquierdo del mouse |
| Apuntar | Botón derecho del mouse |
| Recargar | `R` |
| Cambiar arma | `Q` / `E` |
| Mostrar u ocultar diagnóstico Photon | `F1` |

Al iniciar una partida, el jugador debe atravesar el recorrido y alcanzar la meta. En una partida online, el jugador que llega primero recibe la victoria y los demás clientes cargan la escena de derrota.

Los checkpoints son las naves espaciales que estan fijas en el recorrido del escenario. Cuando la vida queda a 0 se devuelve al spawnpoint.

## Flujo multiplayer

El flujo de conexión está implementado en [`MenuLauncher.cs`](Assets/FPS/Scripts/UI/MenuLauncher.cs) y [`Launcher.cs`](Assets/FPS/Scripts/Gameplay/Launcher.cs):

1. El menú conecta el cliente con Photon.
2. El cliente ingresa a una sala aleatoria o crea una nueva.
3. `Launcher` instancia el prefab del jugador mediante `PhotonNetwork.Instantiate`.
4. Cada cliente controla únicamente el jugador cuyo `PhotonView` le pertenece.
5. Las instancias remotas desactivan cámara, audio e input local.
6. El transform de los jugadores se sincroniza mediante `PhotonTransformView` o `PhotonTransformViewClassic`.
7. La meta envía un RPC a todos los clientes para establecer el resultado común de la partida.

Para probar dos jugadores, abrir dos instancias del proyecto o una instancia del editor y otra build, iniciar ambas desde `MainMenu` y conectarlas con el mismo App ID. El diagnóstico de `F1` permite observar conexión, sala, ping, `PhotonView` y propietario de cada objeto.

## Mecánicas principales

La implementación del movimiento se concentra en [`PlayerCharacterController.cs`](Assets/FPS/Scripts/Gameplay/PlayerCharacterController.cs). Entre los comportamientos configurables se incluyen:

- Velocidad máxima en suelo y aire.
- Aceleración y desaceleración.
- Modificador de sprint.
- Salto y fuerza de gravedad.
- Límite de caída fuera del escenario.
- Daño por velocidad de impacto.
- Detección de suelo y límite de pendiente.
- Actualización de checkpoints al atravesar plataformas o zonas configuradas.

El elemento de física más visible es la plataforma móvil implementada por [`FloorMovement.cs`](Assets/FPS/Scripts/Game/FloorMovement.cs), que puede moverse vertical u horizontalmente y pausar al llegar a su punto superior.

La condición de finalización está implementada en [`Goal.cs`](Assets/FPS/Scripts/Gameplay/Goal.cs). En modo online, el resultado se comunica mediante un RPC; en modo local se carga directamente `WinScene`.

## Estructura relevante

```text
Assets/
├── FPS/
│   ├── Scenes/                 Escenas del menú, escenario y resultados
│   ├── Scripts/Gameplay/       Movimiento, objetivos, pickups y red
│   ├── Scripts/Game/           Actores, salud, proyectiles y plataformas
│   ├── Scripts/Networking/     Reconexión automática
│   ├── Scripts/UI/             Menús, HUD y mensajes
│   └── InputSystem_Actions.inputactions
├── Photon/                     Photon Realtime, PUN y Photon Chat
└── NavMeshComponents/          Componentes de navegación
```

## Alcance de la consigna

| Requisito | Implementación actual |
| --- | --- |
| Entorno 3D recorrible | `PruebaEscenario` con plataformas, obstáculos y checkpoints. |
| Movimiento, salto y gravedad | `PlayerCharacterController` y `PlayerInputHandler`. |
| Colisiones y caídas | `CharacterController`, ground check, daño y respawn. |
| Elemento físico | Plataformas móviles con `FloorMovement`. |
| Sala multiplayer | Photon PUN con unión aleatoria o creación de sala. |
| Dos jugadores simultáneos | Prefabs instanciados y estados relevantes sincronizados. |
| Victoria y derrota | `Goal`, `WinScene` y `LoseScene`. |
| Resultado compartido | RPC `GameOver` enviado a todos los clientes. |
| Diagnóstico técnico | `PhotonDiagnostics` con conexión, ping y `PhotonView`. |

## Trabajo académico

El prototipo se desarrolla con foco en:

- Investigación y comparación de alternativas técnicas.
- Validación experimental del movimiento y las colisiones.
- Integración de multiplayer y consistencia del estado.
- Registro de pruebas, problemas y decisiones técnicas.
- Seguimiento de riesgos, backlog y métricas del proceso.

El repositorio contiene el proyecto Unity. El backlog, las evidencias de testing, los ADR y el informe técnico final deben mantenerse en los espacios de documentación acordados por el equipo o agregarse a este repositorio cuando corresponda.

## Créditos y recursos

El proyecto parte del FPS Microgame de Unity y utiliza recursos de terceros incluidos en `Assets`. Consultar [`Assets/FPS/Third-PartyNotice.txt`](Assets/FPS/Third-PartyNotice.txt) y los archivos `README` o avisos de cada paquete para conocer sus condiciones de uso y atribución.
