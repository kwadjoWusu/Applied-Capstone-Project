// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemySpawner : MonoBehaviour
// {
//     [System.Serializable]
//     public class EnemyType
//     {
//         public string enemyName;
//         public GameObject enemyPrefab;
//         public int difficultyRating = 1; // How difficult this enemy is (1-10)
//     }

//     [System.Serializable]
//     public class SpawnWave
//     {
//         public string waveName;
//         public int totalDifficultyBudget = 10;             // Total points for this wave
//         public List<string> allowedEnemyTypes = new List<string>();
//         public float timeBetweenSpawns = 2f;
//         public int maxConcurrentEnemies = 5;
//         public float waveCompletionDelay = 5f;
//     }

//     public static EnemySpawner instance;

//     [Header("Enemy Types")]
//     public List<EnemyType> enemyTypes = new List<EnemyType>();

//     [Header("Spawn Points")]
//     public List<Transform> spawnPoints = new List<Transform>();

//     [Header("Wave Settings")]
//     public List<SpawnWave> spawnWaves = new List<SpawnWave>();
//     public bool startWavesOnProximity = true;
//     public float proximityDistance = 20f;
//     public bool loopWaves = false;

//     [Header("Debug")]
//     public bool debugSpawn = true;

//     // Runtime
//     private int currentWave = 0;
//     private bool waveActive = false;
//     private int remainingEnemies;
//     private List<GameObject> activeEnemies = new List<GameObject>();
//     private Dictionary<string, EnemyType> lookup = new Dictionary<string, EnemyType>();

//     private void Awake()
//     {
//         if (instance == null) instance = this;
//         else Destroy(gameObject);

//         // Build lookup
//         foreach (var et in enemyTypes)
//             if (et != null && et.enemyPrefab != null)
//                 lookup[et.enemyName] = et;
//     }

//     private void Start()
//     {
//         if (startWavesOnProximity)
//             StartCoroutine(ProximityCheck());
//         else
//             StartWave();
//     }

//     private IEnumerator ProximityCheck()
//     {
//         while (!waveActive)
//         {
//             yield return new WaitForSeconds(1f);
//             if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= proximityDistance)
//                 StartWave();
//         }
//     }

//     private void StartWave()
//     {
//         if (waveActive) return;
//         if (spawnWaves.Count == 0) { Debug.LogError("No waves!"); return; }
//         waveActive = true;
//         StartCoroutine(ProcessWaves());
//     }

//     private IEnumerator ProcessWaves()
//     {
//         do
//         {
//             var wave = spawnWaves[currentWave];
//             if (debugSpawn) Debug.Log($"-- Starting Wave: {wave.waveName}");
//             remainingEnemies = PickWaveEnemies(wave).Count;
//             StartCoroutine(SpawnEnemies(wave));

//             yield return new WaitUntil(() => remainingEnemies <= 0);
//             if (debugSpawn) Debug.Log($"-- Wave {wave.waveName} Complete. Waiting {wave.waveCompletionDelay}s");
//             yield return new WaitForSeconds(wave.waveCompletionDelay);

//             currentWave++;
//             if (currentWave >= spawnWaves.Count && loopWaves) currentWave = 0;
//         }
//         while (loopWaves && (currentWave < spawnWaves.Count));

//         if (debugSpawn) Debug.Log("-- All Waves Done");
//     }

//     private List<EnemyType> PickWaveEnemies(SpawnWave wave)
//     {
//         var list = new List<EnemyType>();
//         int budget = wave.totalDifficultyBudget;
//         // build available list
//         var avail = new List<EnemyType>();
//         foreach (var name in wave.allowedEnemyTypes)
//             if (lookup.ContainsKey(name)) avail.Add(lookup[name]);

//         while (budget > 0 && avail.Count > 0)
//         {
//             var elig = avail.FindAll(e => e.difficultyRating <= budget);
//             if (elig.Count == 0) break;
//             var pick = elig[Random.Range(0, elig.Count)];
//             list.Add(pick);
//             budget -= pick.difficultyRating;
//         }
//         return list;
//     }

//     private IEnumerator SpawnEnemies(SpawnWave wave)
//     {
//         var enemies = PickWaveEnemies(wave);
//         foreach (var et in enemies)
//         {
//             // throttle max concurrent
//             while (activeEnemies.Count >= wave.maxConcurrentEnemies)
//             {
//                 activeEnemies.RemoveAll(e => e == null);
//                 yield return new WaitForSeconds(0.5f);
//             }

//             // pick spawn point randomly
//             var pt = spawnPoints[Random.Range(0, spawnPoints.Count)];
//             var go = Instantiate(et.enemyPrefab, pt.position, pt.rotation);
//             activeEnemies.Add(go);

//             // hook up death callback if your AI has one
//             var ai = go.GetComponent<EnemyAIController>();
//             if (ai != null) ai.OnEnemyDeath += () => { remainingEnemies--; };

//             yield return new WaitForSeconds(wave.timeBetweenSpawns);
//         }

//         // if no enemies spawned, end wave immediately
//         if (enemies.Count == 0) remainingEnemies = 0;
//     }

//     // Draw your spawn points & proximity in the editor
//     private void OnDrawGizmos()
//     {
//         if (spawnPoints != null)
//         {
//             Gizmos.color = Color.blue;
//             foreach (var p in spawnPoints)
//                 if (p != null) Gizmos.DrawWireSphere(p.position, 0.5f);
//         }
//         if (startWavesOnProximity)
//         {
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireSphere(transform.position, proximityDistance);
//         }
//     }

//     // helper
//     private void Log(string msg)
//     {
//         if (debugSpawn) Debug.Log("[Spawner] " + msg);
//     }
// }
