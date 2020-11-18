using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Ce composant gere le spawn des ennemis et les vagues

public class EnemiesSpawnArea : MonoBehaviour
{
    public Vector2 size; // Taille de la zone de spawn
    public GameObject warningObject; // Point d'exclamation a créer avant le spawn de l'ennemi
    [Space]
    public List<WaveStats> waveSteps = new List<WaveStats>(); // Description des vagues (nombre d'ennemis, vitesse, vie de ces derniers...)
    
    private Transform player;
    private Text waveCountText, timerText; // References au elements d'interface en haut a droite
    private int currentWave = -1; // Index de la vague actuelle

    private float timeBetweenwaves = 20;
    private float waitingTime; // Temps d'attente actuel
    [HideInInspector] public bool inWave = false, spawningNewWave = false;
    private int offsetWaves = 0;

    private void Start()
    {
        player = PlayerBehavior.instance.transform;
        waveCountText = GameObject.FindGameObjectWithTag("WaveCountText").GetComponent<Text>();
        timerText = GameObject.FindGameObjectWithTag("NextWaveTimer").GetComponent<Text>();
    }

    private void Update()
    {
        // Si il n'y a plus d'ennemis dans la scene (et que je ne suis pas deja en train de spawner de nouveau ennemis) alors commencer une nouvelle vague
        List<GameObject> ennemisInScene = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        if(ennemisInScene.Count <= 0 && !spawningNewWave)
        {
            // Si in wave est encore a vrai alors cela veut dire que nous sortons juste d'une vague
            if (inWave)
            {
                waitingTime = 0f;
                inWave = false;
            }else
            {
                // Sinon j'attends 20 secondes (variable timeBetweenwaves) avant la prochaine vague
                waitingTime += Time.deltaTime;
                timerText.gameObject.SetActive(true);
                timerText.text = "Prochaine dans " + Mathf.RoundToInt(timeBetweenwaves - waitingTime); // Mise a jour du compteur de temps restant

                if (waitingTime >= timeBetweenwaves)
                {
                    // Une fois les 20 secondes passées je commence la vague
                    timerText.gameObject.SetActive(false);
                    StartCoroutine(NextWave());
                    spawningNewWave = true;
                    inWave = true;
                }
            }
        }

    }

    // Spawn de la vague suivante (en fonction de waveSteps)
    private IEnumerator NextWave ()
    {
        // Mettre a jour l'index de vague actuel
        // Si le joueur n'a pas depassé les vague prévu dans waveSteps alors passer a la suivante
        if (currentWave < waveSteps.Count - 1)
        {
            currentWave++;
            waveCountText.text = "Vague " + (currentWave + 1).ToString(); // Mettre a jour le compteur de vague
        }
        // Si non, si il est plus loin que les vagues prévu par les devs dans waveSteps alors on spawn la meme vague que precedent (derniere vague de waveSteps)
        else
        {
            offsetWaves++; // offsetWaves est le nombres de vagues passées au de la des vagues prévus par les devs dans waveSteps
            currentWave = waveSteps.Count - 1; // Joueur la derniere vague de waveSteps
            waveCountText.text = "Vague " + (currentWave + 1 + offsetWaves).ToString();
        }

        // Spawn ennemies
        int count = waveSteps[currentWave].enemiesCount + (offsetWaves * 2); // Nombre d'ennemis a spawner (ajoute deux ennemis suplementaires a chaque nouvelles vagues depassant de waveSteps)
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(SpawnRandomEnemy(waveSteps[currentWave].enemisHealth, waveSteps[currentWave].enemisSpeed, waveSteps[currentWave].ennemisPrefabs));
            yield return new WaitForSeconds(Random.Range(0.2f, 1f)); // Petit temps d'attente entre deux spawn
        }

        spawningNewWave = false;
    }

    // Spawn un ennemi a une position aleatoire
    private IEnumerator SpawnRandomEnemy (float enemyHealth, float enemySpeed, List<GameObject> enemiesPrefabs)
    {
        // Position aleatoire loin du joueur
        Vector2 spawnPos = new Vector2(transform.position.x + Random.Range(-size.x / 2, size.x / 2), transform.position.y + Random.Range(-size.y / 2, size.y / 2));
        while (Vector2.Distance(player.position, spawnPos) < 15f)
        {
            spawnPos = new Vector2(transform.position.x + Random.Range(-size.x / 2, size.x / 2), transform.position.y + Random.Range(-size.y / 2, size.y / 2));
        }

        // Spawn du point d'exclamation avec le spawn de l'ennemi
        Instantiate(warningObject, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        // Spawn de l'ennemi et assignation de ces variables
        GameObject instance = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], spawnPos, Quaternion.identity);
        Enemy script = instance.GetComponent<Enemy>();

        if(instance.GetComponent<DistanceEnemy>() != null)
        {
            script.health = enemyHealth / 2;
        }else
        {
            script.health = enemyHealth;
        }
        script.moveSpeed = enemySpeed;
    }

    // Dessine la zone dans la quelle les ennemis peuvent spawner (pour l'éditeur)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}

// Description des vagues
[System.Serializable]
public class WaveStats
{
    // Cette classe est la description d'une vague (nombre d'ennemis, leurs vie, leurs vitesse...)
    public int enemiesCount = 1;
    public float enemisHealth;
    public float enemisSpeed;
    public List<GameObject> ennemisPrefabs = new List<GameObject>();
}
