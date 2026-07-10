using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win condition (balance levers)")]
    [SerializeField] private int targetMoney = 100;
    [SerializeField] private float timeLimitSeconds = 120f;

    public int Money { get; private set; }
    public float TimeRemaining { get; private set; }

    private bool _over;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        TimeRemaining = timeLimitSeconds;
    }

    private void Update()
    {
        if (_over) return;

        TimeRemaining -= Time.deltaTime;
        if (TimeRemaining <= 0f) EndGame(Money >= targetMoney);
    }

    public void AddMoney(int amount)
    {
        if (_over) return;

        Money += amount;
        if (Money >= targetMoney) EndGame(true);
    }

    private void EndGame(bool won)
    {
        _over = true;
        Debug.Log(won ? $"WIN — ${Money}" : $"LOSE — ${Money} / ${targetMoney}");
        // TODO: UI end screen + Firebase high score hook here
    }
}
