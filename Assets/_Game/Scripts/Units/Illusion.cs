using UnityEngine;


public class Illusion : Enemy
{
    [Header("Illusion Parameters")]
    [SerializeField] private float duration;
    private float timer;
    public Transform Model => model;

    private void OnEnable()
    {
        StateManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        StateManager.OnGameStateChanged -= OnGameStateChanged;
    }

    void Start()
    {
        ApplyStats();
        CacheComponents();
        InitializeAudio();
    }

    void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;
        if (state == EnemyState.Dead) return;

        timer += Time.deltaTime;

        if (timer >= duration)
        {
            OnDeath();
        }
    }

    protected override void Attack() { }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Passive)
        {
            OnDeath();
        }
    }
}
