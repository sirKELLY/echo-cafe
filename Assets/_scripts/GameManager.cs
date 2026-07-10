using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win condition (balance levers)")]
    [SerializeField] private int targetMoney = 100;
    [SerializeField] private float timeLimitSeconds = 120f;

    [Header("HUD")]
    [SerializeField] private TMP_Text moneyText;   // accepts a UI (TextMeshProUGUI) or world-space TextMeshPro
    [SerializeField] private TMP_Text timeText;

    public int Money { get; private set; }
    public float TimeRemaining { get; private set; }

    public bool IsOver => _over;
    public int TargetMoney => targetMoney;
    public float MoneyProgress01 => Mathf.Clamp01((float)Money / targetMoney);
    public float TimeProgress01 => Mathf.Clamp01(TimeRemaining / timeLimitSeconds);

    public event System.Action<int, int> OnMoneyChanged;   // (newTotal, delta) — "+$n" popups
    public event System.Action<bool> OnGameEnded;          // true = won

    private bool _over;
    private int _shownSeconds = -1;   // so the clock string is only rebuilt when the visible second changes

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        TimeRemaining = timeLimitSeconds;
        RefreshMoney();
        RefreshTime();
    }

    private void Update()
    {
        if (_over) return;

        TimeRemaining -= Time.deltaTime;
        RefreshTime();
        if (TimeRemaining <= 0f) EndGame(Money >= targetMoney);
    }

    public void AddMoney(int amount)
    {
        if (_over) return;

        Money += amount;
        RefreshMoney();
        OnMoneyChanged?.Invoke(Money, amount);
        if (Money >= targetMoney) EndGame(true);
    }

    private void EndGame(bool won)
    {
        _over = true;
        OnGameEnded?.Invoke(won);
        Debug.Log(won ? $"WIN — ${Money}" : $"LOSE — ${Money} / ${targetMoney}");
        // TODO: UI end screen + Firebase high score hook here
    }

    private void RefreshMoney()
    {
        if (moneyText != null) moneyText.text = $"${Money}";
    }

    private void RefreshTime()
    {
        if (timeText == null) return;

        int secs = Mathf.Max(0, Mathf.CeilToInt(TimeRemaining));
        if (secs == _shownSeconds) return;   // skip the string rebuild when nothing visibly changed

        _shownSeconds = secs;
        timeText.text = $"{secs / 60}:{secs % 60:00}";
    }
}
