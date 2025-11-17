# Maze Escape — Unity Project

## 📌 Descripción general
Maze Escape es un juego desarrollado en Unity donde el jugador debe escapar de un laberinto generado proceduralmente, evitando enemigos que lo persiguen mediante NavMesh y gestionando su vida dentro de un tiempo límite.  
Las variables del nivel (tamaño, enemigos y tiempo) dependen de la dificultad seleccionada desde el menú principal.

---

## 🚀 Características principales

### ✔️ Generación procedural de laberintos
- Algoritmo DFS (Depth-First Search)
- Laberintos perfectos sin loops
- Vista previa aérea antes de empezar
- Lógica separada en `MazeGenerator` y renderizado en `Maze`

### ✔️ Sistema de dificultad
Tres niveles:
| Dificultad | Tamaño | Tiempo | Enemigos |
|------------|--------|--------|----------|
| Fácil      | 5x5    | 60 s   | Bajo     |
| Medio      | 10x10  | 40 s   | Medio    |
| Difícil    | 15x15  | 30 s   | Alto     |

### ✔️ Enemigos con IA de persecución
- Implementados con `NavMeshAgent`
- Ajuste dinámico del NavMesh según paredes del laberinto
- Daño por proximidad al jugador

### ✔️ Jugador en primera persona
- Movimiento con `CharacterController`
- Correr, saltar, cámara flotante
- Sistema modular de vida con eventos

### ✔️ UI completa
- Barra de vida
- Temporizador por nivel
- Menú de pausa (TAB)
- Pantalla de victoria o derrota
- Créditos inferiores (Desarrollo de Videojuegos II — UNIR)

---

## 📁 Estructura del Proyecto

```

Assets/
│
├─ Scripts/
│   ├─ MazeScripts/
│   │   ├─ Maze.cs
│   │   ├─ MazeGenerator.cs
│   │   ├─ MazeCellData.cs
│   │   └─ Cell.cs
│   │
│   ├─ EnemyScripts/
│   │   └─ SimpleChaser.cs
│   │
│   ├─ PlayerController/
│   │   ├─ Health.cs
│   │   └─ Otros scripts
│   │
│   ├─ UI/
│       ├─ HealthBarUI.cs
│       ├─ TimerUI.cs
│       ├─ InGameMenuController.cs
│       └─ DifficultySelector.cs
│
└─ Prefabs/
├─ CellPrefab
├─ EnemyPrefab
├─ ExitPoint
└─ PlayerPrefab

```

---

## 🎮 Flujo de ejecución

1. El usuario selecciona dificultad en el menú principal.
2. `MazeGenerator` crea un laberinto único según la dificultad.
3. `Maze.cs` instancia físicamente el laberinto y reconstruye el NavMesh.
4. Se generan enemigos y se muestra una vista previa aérea.
5. Se instancia el jugador y comienza el temporizador.
6. El jugador debe:
   - Llegar a la meta → **Victoria**
   - Perder toda la vida → **Derrota**
   - Agotar el tiempo → **Derrota**

---

## 🔧 Scripts clave

### `Maze.cs`
Control principal:
- Instancia celdas
- Construye NavMesh dinámico
- Controla enemigos, jugador y meta
- Administra victoria/derrota/pausa

### `MazeGenerator.cs`
Lógica procedural:
- DFS para generar laberintos
- Datos independientes del renderizado

### `SimpleChaser.cs`
IA básica:
- Persecución mediante NavMeshAgent
- Daño por proximidad

### `Health.cs`
- Gestión de vida
- Eventos para UI
- Manejo de muerte

### `TimerUI.cs`
- Cuenta regresiva
- Llamada a derrota por tiempo

---

## 🛠️ Instalación y requisitos

### Requisitos
- Unity 6 o superior (ideal 6000.0.43f1)
- Soporta:
  - Windows
  - WebGL
  - MacOS (si se recompila)

### Configuración
1. Clonar el repositorio.
2. Abrir el proyecto en Unity.
3. Ejecutar la escena **MainMenu**.
4. Para builds WebGL:
   - Activar compresión Gzip o Brotli.
   - Ajustar resolución escalable.

---

## 🎮 Controles
- **WASD** → Moverse  
- **Space** → Saltar  
- **Shift** → Correr  
- **Tab** → Menú de pausa  

---

## 📈 Extensiones posibles
- Añadir combate (disparos)
- Enemigos con diferentes patrones de IA
- Objetos coleccionables
- Sonidos y música reactiva
- Mazmorras más grandes o multicapas

---

## 👨‍🎓 Créditos
Proyecto desarrollado como parte del curso  
**Desarrollo de Videojuegos II — UNIR**.
