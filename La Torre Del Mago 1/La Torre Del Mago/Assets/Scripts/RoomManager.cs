using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public enum Difficulty { Easy, Medium, Hard, Finished }

    [Header("Listas originales (editar en Inspector)")]
    [Tooltip("Nombres de escenas fáciles (en Build Settings)")]
    public List<string> easyScenes = new List<string>();
    [Tooltip("Nombres de escenas medias (en Build Settings)")]
    public List<string> mediumScenes = new List<string>();
    [Tooltip("Nombres de escenas difíciles (en Build Settings)")]
    public List<string> hardScenes = new List<string>();

    [Header("Opciones")]
    [Tooltip("Cargar automáticamente la primera sala al iniciar el juego")]
    public bool autoStart = true;

    [Tooltip("Mostrar logs en consola")]
    public bool verboseLogs = true;

    // Copias trabajables (pools) que se barajan cada corrida
    private readonly List<string> _easyPool = new();
    private readonly List<string> _mediumPool = new();
    private readonly List<string> _hardPool = new();

    // Para depurar: guardamos los ya usados (no necesario para lógica, pero útil)
    private readonly List<string> _usedEasy = new();
    private readonly List<string> _usedMedium = new();
    private readonly List<string> _usedHard = new();

    private Difficulty _current = Difficulty.Easy;
    private bool _isLoading = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Preparar pools barajados
        ResetRun();
    }

    private void Start()
    {
        if (autoStart)
            LoadNextRoom();
    }

    private void Update()
    {
        // Forzar siguiente sala con P (test)
        if (Input.GetKeyDown(KeyCode.P))
            LoadNextRoom();
    }

    /// <summary>
    /// Reinicia la corrida: clona y baraja los pools, resetea estado y usados.
    /// </summary>
    public void ResetRun()
    {
        _easyPool.Clear(); _mediumPool.Clear(); _hardPool.Clear();
        _usedEasy.Clear(); _usedMedium.Clear(); _usedHard.Clear();

        _easyPool.AddRange(easyScenes);
        _mediumPool.AddRange(mediumScenes);
        _hardPool.AddRange(hardScenes);

        Shuffle(_easyPool);
        Shuffle(_mediumPool);
        Shuffle(_hardPool);

        _current = Difficulty.Easy;

        if (verboseLogs)
            Debug.Log($"[RoomManager] Run reseteada. Pools: E({_easyPool.Count}) M({_mediumPool.Count}) H({_hardPool.Count}).");
    }

    /// <summary>
    /// Carga la próxima sala según el pool actual; si se agota, avanza de dificultad.
    /// </summary>
    public void LoadNextRoom()
    {
        if (_isLoading) return;
        if (_current == Difficulty.Finished)
        {
            if (verboseLogs) Debug.Log("[RoomManager] No quedan más salas (Finished).");
            return;
        }

        string next = DequeueNextSceneName();
        if (string.IsNullOrEmpty(next))
        {
            // No hay en este nivel; se habrá avanzado de dificultad. Intentar de nuevo si no está Finished.
            if (_current != Difficulty.Finished)
                LoadNextRoom();
            else if (verboseLogs)
                Debug.Log("[RoomManager] Se completaron todas las salas.");
            return;
        }

        if (verboseLogs) Debug.Log($"[RoomManager] Cargando escena: {next} ({_current})");

        _isLoading = true;
        var op = SceneManager.LoadSceneAsync(next, LoadSceneMode.Single);
        op.completed += _ =>
        {
            _isLoading = false;
        };
    }

    /// <summary>
    /// Toma el siguiente nombre de escena del pool actual. Si el pool está vacío, avanza de dificultad.
    /// </summary>
    private string DequeueNextSceneName()
    {
        switch (_current)
        {
            case Difficulty.Easy:
                if (_easyPool.Count > 0)
                    return PopFromPool(_easyPool, _usedEasy);
                _current = Difficulty.Medium;
                if (verboseLogs) Debug.Log("[RoomManager] Pasando a MEDIAS.");
                return DequeueNextSceneName();

            case Difficulty.Medium:
                if (_mediumPool.Count > 0)
                    return PopFromPool(_mediumPool, _usedMedium);
                _current = Difficulty.Hard;
                if (verboseLogs) Debug.Log("[RoomManager] Pasando a DIFICILES.");
                return DequeueNextSceneName();

            case Difficulty.Hard:
                if (_hardPool.Count > 0)
                    return PopFromPool(_hardPool, _usedHard);
                _current = Difficulty.Finished;
                if (verboseLogs) Debug.Log("[RoomManager] ¡Todas las salas completadas!");
                return null;

            default:
                return null;
        }
    }

    /// <summary>
    /// Toma y remueve el primer elemento del pool (ya barajado) y lo agrega a la lista de usados.
    /// </summary>
    private static string PopFromPool(List<string> pool, List<string> used)
    {
        // Podés elegir tomar del final si preferís (no cambia porque ya está barajado)
        string scene = pool[0];
        pool.RemoveAt(0);
        used.Add(scene);
        return scene;
    }

    /// <summary>
    /// Baraja en sitio (Fisher–Yates).
    /// </summary>
    private static void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // --- Utilidades opcionales ---
    public Difficulty GetCurrentDifficulty() => _current;

    public (IReadOnlyList<string> remaining, IReadOnlyList<string> used) GetEasyDebug()
        => (_easyPool.AsReadOnly(), _usedEasy.AsReadOnly());
    public (IReadOnlyList<string> remaining, IReadOnlyList<string> used) GetMediumDebug()
        => (_mediumPool.AsReadOnly(), _usedMedium.AsReadOnly());
    public (IReadOnlyList<string> remaining, IReadOnlyList<string> used) GetHardDebug()
        => (_hardPool.AsReadOnly(), _usedHard.AsReadOnly());
}
