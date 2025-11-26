using System.Collections;
using UnityEngine;
using TMPro;
public class WaveManager : MonoBehaviour
{
    public WaveUIController waveUI;   // gán trong Inspector


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
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    [Header("Delay")]
    public float startDelay = 1f;      // đợi 1s rồi mới spawn wave 1
    public float nextWaveDelay = 2f;   // đợi 2s giữa các wave

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private bool spawningWave = false;
    private bool bossSpawned = false;

    [Header("UI Counter")]
    public TextMeshProUGUI enemyCounterText;

    private int enemiesToKillThisWave = 0; 
    private int enemiesKilledThisWave = 0;

    private void OnEnable()
    {
        EnemyBase.OnAnyEnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        EnemyBase.OnAnyEnemyDied -= OnEnemyDied;
    }

    private void Start()
    {
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
            if (!bossSpawned)
            {
                SpawnBoss();
            }
            return;
        }

        Wave wave = waves[currentWaveIndex];
        Debug.Log($"WaveManager: Start {wave.waveName}");

        enemiesToKillThisWave = wave.enemyCount;
        enemiesKilledThisWave = 0;
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

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemyFromWave(wave);
            enemiesAlive++;

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        spawningWave = false;
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

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private void OnEnemyDied(EnemyBase enemy)
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        enemiesKilledThisWave = Mathf.Clamp(enemiesKilledThisWave + 1, 0, enemiesToKillThisWave);
        UpdateEnemyCounterUI();

        if (!spawningWave && enemiesAlive <= 0)
        {
            if (currentWaveIndex == waves.Length - 1)
            {
                if (!bossSpawned)
                {
                    StartCoroutine(SpawnBossAfterDelay());
                }
            }
            else
            {
                StartCoroutine(StartNextWaveAfterDelay());
            }
        }
    }

    private void UpdateEnemyCounterUI()
    {
        if (enemyCounterText == null) return;
        enemyCounterText.text = $"{enemiesKilledThisWave} / {enemiesToKillThisWave}";
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

        bossSpawned = true;

        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : Vector3.zero;
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        Debug.Log("WaveManager: Boss spawned!");
    }
}
