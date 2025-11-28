using System.Collections;
using UnityEngine;
using TMPro;
public class WaveManager : MonoBehaviour
{
    public WaveUIController waveUI;   // gán trong Inspector
    public static WaveManager instance;

    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public GameObject[] enemyPrefabs;   // các loại quái trong wave
        public int enemyCount = 5;          // số quái trong wave
        public float spawnInterval = 0.5f;  // thời gian giữa mỗi con spawn
    }

    [Header("Waves Setup")]
    public Wave[] waves;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;    // chỗ sinh quái
                                       // có thể là 2,3 điểm bên trên màn

    [Header("Boss")]
    public GameObject[] bossPrefab;
    public Transform bossSpawnPoint;
    private int bossWaveIndex = -1;

    [Header("Delay")]
    public float startDelay = 1f;      // đợi 1s rồi mới spawn wave 1
    public float nextWaveDelay = 2f;   // đợi 2s giữa các wave
    public float waveSpawnDelay = 2f;
    public int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private bool spawningWave = false;
    private bool bossSpawned = false;

    [Header("UI Counter")]
    public TextMeshProUGUI enemyCounterText;

    private int enemiesToKillThisWave = 0; 
    private int enemiesKilledThisWave = 0;


    [Header("Audio")]
    public AudioClip startMatchSound;
    private void OnEnable()
    {
        EnemyBase.OnAnyEnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        EnemyBase.OnAnyEnemyDied -= OnEnemyDied;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (AudioManager.instance != null && startMatchSound != null)
        {
            AudioManager.instance.PlaySFX(startMatchSound);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBattleMusic();
        }
        StartCoroutine(StartFirstWave());
    }

    private IEnumerator StartFirstWave()
    {
        yield return new WaitForSeconds(startDelay);
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            return;
        }

        Wave wave = waves[currentWaveIndex];
        Debug.Log($"WaveManager: Start {wave.waveName}");

        enemiesToKillThisWave = wave.enemyCount;
        enemiesKilledThisWave = 0;
        enemiesAlive = 0;
        UpdateEnemyCounterUI();
        if (waveUI != null)
        {
            waveUI.ShowWave($"WAVE {currentWaveIndex + 1}");
        }

        StartCoroutine(SpawnWaveRoutine(wave));
    }


    private IEnumerator SpawnWaveRoutine(Wave wave)
    {
        spawningWave = true;
        enemiesAlive = 0;
        yield return new WaitForSeconds(waveSpawnDelay);

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemyFromWave(wave);
            enemiesAlive++;

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        spawningWave = false;
        CheckWaveStatus();
    }

    private void SpawnEnemyFromWave(Wave wave)
    {
        if (wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("WaveManager: Wave không có enemyPrefabs.");
            return;
        }

        GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];
        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        GameObject newEnemy =  Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        float muitipler = 1f + (currentWaveIndex * 0.2f);
        EnemyBase enemyBase = newEnemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.BuffStats(muitipler);
            Debug.Log($"WaveManager: Spawned enemy {enemyBase.name} with stats multiplier {muitipler}");
        }
    }

    private void OnEnemyDied(EnemyBase enemy)
    {
        if (bossSpawned) return;
        // Giảm số lượng quái sống
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;

        // Tăng số lượng đã giết để hiện UI
        enemiesKilledThisWave++;
        UpdateEnemyCounterUI();

        // Kiểm tra xem đã hết Wave chưa
        CheckWaveStatus();
    }

    private void CheckWaveStatus()
    {
        // 1. Nếu vẫn đang sinh quái (chưa chạy xong vòng for) -> Không làm gì cả
        if (spawningWave) return;

        // 2. Nếu đã sinh xong mà vẫn còn quái sống -> Không làm gì cả
        if (enemiesAlive > 0) return;

        // 3. Nếu đã sinh xong VÀ hết quái sống -> Qua màn
        Debug.Log("Wave Cleared!");

        if (currentWaveIndex >= waves.Length - 1)
        {
            // Nếu đúng là Wave cuối -> Gọi Boss
            if (!bossSpawned)
            {
                StartCoroutine(SpawnBossAfterDelay());
            }
        }
        else
        {
            // Nếu chưa phải Wave cuối -> Đi tiếp Wave sau
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    private void UpdateEnemyCounterUI()
    {
        if (enemyCounterText == null) return;
        enemyCounterText.text = $"{enemiesKilledThisWave}";
    }


    private IEnumerator StartNextWaveAfterDelay()
    {
        Debug.Log("WaveManager: All enemies cleared. Next wave incoming...");
        yield return new WaitForSeconds(nextWaveDelay);
        StartNextWave();
    }

    private IEnumerator SpawnBossAfterDelay()
    {
        Debug.Log("WaveManager: All waves cleared. Boss incoming...");

        if (waveUI != null)
        {
            waveUI.ShowWave("THE BOSS HAS APPEARED");
        }

        yield return new WaitForSeconds(nextWaveDelay);
        SpawnBoss();
    }


    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("WaveManager: BossPrefab chưa gán!");
            return;
        }

        //if (AudioManager.instance != null)
        //{
        //    AudioManager.instance.PlayBossMusic();
        //}

        bossSpawned = true;

        GameObject randomBoss = bossPrefab[Random.Range(0, bossPrefab.Length)];

        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : Vector3.zero;

        Instantiate(randomBoss, spawnPos, Quaternion.identity);

        Debug.Log($"WaveManager: Boss {randomBoss.name} spawned!");
    }
}
