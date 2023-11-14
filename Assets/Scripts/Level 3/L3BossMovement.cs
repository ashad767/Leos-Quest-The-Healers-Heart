using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class L3BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator a;
    [SerializeField] private AnimationClip[] animLength;
    [SerializeField] private Transform MC;

    // Prefabs
    public GameObject lightningPrefab;
    public GameObject miniZombiePrefab;
    public GameObject miniSkeletonPrefab;
    [SerializeField] private GameObject startingDarknessPrefab;
    [SerializeField] private GameObject boneShieldPrefab;

    private MiniEnemiesSpawnManager spawnManager;
    [SerializeField] private darknessManager darknessManager; // the script
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject MC_PointLight;

    // Audio
    [SerializeField] AudioSource swingAudio;
    [SerializeField] AudioSource maceDragAudio;
    [SerializeField] AudioSource startingDarknessAudio;
    [SerializeField] AudioSource insideDarknessAudio;
    [SerializeField] AudioSource boneShieldAudio;
    [SerializeField] AudioSource deathAudio;

    // Animation states
    private enum States { idle, walk, attack };

    public bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool dead = false;
    
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        
        spawnManager = MiniEnemiesSpawnManager.Instance;
        directionalLight.SetActive(false);
        MC_PointLight.SetActive(false);

        StartCoroutine(follow_MC());
        StartCoroutine(spawnMiniEnemies());
        StartCoroutine(Darken());
        StartCoroutine(dummyBossHitTester());
    }

    // Update is called once per frame
    void Update()
    {
        // Flip boss sprite on its X axis depending on if the MC is left or right of the boss
        sr.flipX = MC.position.x < transform.position.x;

        if (idle && !attack)
        {
            a.SetInteger("state", (int)States.idle);
        }

        // Make the boss move towards MC when MC is NOT idle
        if (!idle && !dead)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 2.7f * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            idle = false;

            walk = true;
            a.SetInteger("state", (int)States.walk);
            maceDragAudio.Play();
            yield return new WaitForSeconds(3f);
            maceDragAudio.Stop();
            walk = false;

            idle = true;
            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator spawnMiniEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f)); // Wait 3-10 seconds before getting the chance to spawn mini-enemies

            // 95% chance of mini-enemies getting spawned (if not the first iteration, only spawns if all 3 mini-enemies are killed from previous wave)
            if (Random.Range(0f, 1f) <= 0.95f)
            {
                spawnManager.StartSpawning();
            }
        }
    }

    private IEnumerator Darken()
    {
        float chanceOfDarkness = 0.65f;

        while (true)
        {
            // Because I don't want the chance of darkening the screen at the start of the game, I check if 0.1 seconds have passed since the beginning of the game, then wait 8 sec before starting the chance to darken the screen or activate bone shield.
            if(Time.time <= 0.1f)
            {
                yield return new WaitForSeconds(8f);
            }
            
            // 65% chance of the screen getting dark on every other iteration (starts from 65%, then if dark, stay at 65%. If shield gets activated instead, chance of darkness is 25% on next iteration)
            if (!dead && (Random.Range(0f, 1f) <= chanceOfDarkness))
            {
                startingDarknessAudio.Play();
                GameObject startingDarkness = Instantiate(startingDarknessPrefab, transform.position, Quaternion.identity);
                startingDarkness.transform.SetParent(transform); // if the boss moves, the animation moves with it
                Destroy(startingDarkness, animLength[3].length * 2); // playing the animation twice before destroying it

                darknessManager.activateDarkness();
            }

            // if the darkness effect gets activated, I want to wait 37 seconds before deactivating it. And since I don't want to darken the screen right away again, I wait another 6 sec.
            if (darknessManager.isDark)
            {
                StartCoroutine(PlayInsideDarknessAudio());
                yield return new WaitForSeconds(35f);
                darknessManager.de_activateDarkness();

                chanceOfDarkness = 0.25f;
                yield return new WaitForSeconds(5f);
            }
            
            // if the darkness effect does NOT get activated, just wait 2 sec before giving the chance to darken the screen.
            else
            {
                StartCoroutine(activateBoneShield());
                yield return new WaitForSeconds(8f); // control comes back here after first yield call in 'activateBoneShield()'. Bone shield effect lasts 8 sec, so I wait 8 seconds here as well to let the bone shield effect finish properly.

                chanceOfDarkness = 0.65f;
                yield return new WaitForSeconds(2f); // Wait another 2 seconds just for the sake of it
            }
        }
    }

    private IEnumerator PlayInsideDarknessAudio()
    {
        yield return new WaitForSeconds(startingDarknessAudio.clip.length - 5.5f);
        insideDarknessAudio.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
            maceDragAudio.Stop();
            a.SetInteger("state", (int)States.attack);
            StartCoroutine(attackFunc());
        }
    }
    private IEnumerator attackFunc()
    {
        swingAudio.Play();
        yield return new WaitForSeconds(animLength[0].length);
        attack = false;

        // because walking is a looped animation, if boss attacks midway of the walking animation, I want to return back to the walking animation
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
            maceDragAudio.Play();
        }
    }

    private IEnumerator activateBoneShield()
    {
        boneShieldAudio.Play();
        GameObject boneShield = Instantiate(boneShieldPrefab, transform.position, Quaternion.identity);
        boneShield.transform.SetParent(transform);
        
        yield return new WaitForSeconds(8f); // let the bone shield effect last 8 seconds
        Destroy(boneShield);
    }

    private IEnumerator dummyBossHitTester()
    {
        while (true)
        {
            Color originalColor = sr.color;
            Color hitEffect = sr.color;

            yield return new WaitForSeconds(2f);
            currentHealth -= 10f;

            // When boss gets hit, I want to momentarily make the boss go slighlty transparent, then back to its original/angry color
            hitEffect.a = 0.2f;
            sr.color = hitEffect;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;

            if (currentHealth <= 0f)
            {
                dead = true;
                GetComponent<CircleCollider2D>().enabled = false;
                rb.bodyType = RigidbodyType2D.Static;
                a.SetTrigger("death"); // show death animation
                deathAudio.Play();
                
                yield return new WaitForSeconds(deathAudio.clip.length);
                Destroy(gameObject); // Destroys boss gameobject
            }
        }
    }

    private void OnDestroy()
    {
        // Find all active shadow clone prefabs in the scene and destroy them
        GameObject[] miniZombiesToDestroy = GameObject.FindGameObjectsWithTag("miniZombie");
        GameObject[] miniSkeletonsToDestroy = GameObject.FindGameObjectsWithTag("miniSkeleton");
        GameObject[] arrowsToDestroy = GameObject.FindGameObjectsWithTag("arrow");

        foreach (GameObject miniZombie in miniZombiesToDestroy)
        {
            Destroy(miniZombie);
        }

        foreach (GameObject miniSkeleton in miniSkeletonsToDestroy)
        {
            Destroy(miniSkeleton);
        }

        foreach (GameObject arrow in arrowsToDestroy)
        {
            Destroy(arrow);
        }
    }
}
