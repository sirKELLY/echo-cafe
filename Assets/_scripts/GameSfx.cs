using _scripts;
using UnityEngine;

// Global game moments. Goes on the Systems object; drag in the managers it should listen to.
// Any unassigned manager or clip is simply skipped.
public class GameSfx : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private EchoManager echoManager;
    [SerializeField] private CustomerSpawner customerSpawner;

    [SerializeField] private AudioClip moneyChanged;      // coin clink on every sale
    [SerializeField] private AudioClip gameWon;
    [SerializeField] private AudioClip gameLost;
    [SerializeField] private AudioClip echoSpawned;       // the ghost clocks in
    [SerializeField] private AudioClip customerArrived;   // door chime (also see CustomerSfx.arrived — assign one, not both)

    private void Start()   // Start: all managers have finished Awake by now
    {
        if (gameManager != null)
        {
            gameManager.OnMoneyChanged += PlayMoney;
            gameManager.OnGameEnded += PlayEnded;
        }
        if (echoManager != null) echoManager.OnEchoSpawned += PlayEchoSpawned;
        if (customerSpawner != null) customerSpawner.OnCustomerArrived += PlayCustomerArrived;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnMoneyChanged -= PlayMoney;
            gameManager.OnGameEnded -= PlayEnded;
        }
        if (echoManager != null) echoManager.OnEchoSpawned -= PlayEchoSpawned;
        if (customerSpawner != null) customerSpawner.OnCustomerArrived -= PlayCustomerArrived;
    }

    private void PlayMoney(int total, int delta) => Sfx.Play(moneyChanged, volume);
    private void PlayEnded(bool won) => Sfx.Play(won ? gameWon : gameLost, volume);
    private void PlayEchoSpawned(ReplaySource _) => Sfx.Play(echoSpawned, volume);
    private void PlayCustomerArrived(Customer _) => Sfx.Play(customerArrived, volume);
}
