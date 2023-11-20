using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocksFallLogic : MonoBehaviour
{
    private Rigidbody2D rb;
    float impactPos;

    // Prefabs
    [SerializeField] private GameObject rockImpactPrefab;
    [SerializeField] private GameObject rockExplosionPrefab;

    // For the prefab animations
    [SerializeField] private AnimationClip[] animLength;

    // Start is called before the first frame update
    void Start()
    {
        Camera mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * 15f;

        float mainCameraPositionY = mainCamera.transform.position.y;
        float frustumHeight = 2.0f * mainCamera.orthographicSize;

        // When dividing frustumHeight by a number, the closer to 1 or less (0.9, 0.5, 0.2, 0.01, etc...), the higher/lower down it goes.
        // The further away from 1 (2, 5, 10), the less higher/lower it goes.
        impactPos = Random.Range(mainCameraPositionY + (-frustumHeight / 2f), 
                                 mainCameraPositionY + (frustumHeight / 2.5f)
                                );

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 720 * Time.deltaTime); // rotate the rock 2x per second

        // Trigger rock impact & explosion animations when the rock's y position dips below the randomly generated y-value from 'impactPos' variable
        if (transform.position.y <= impactPos)
        {
            GameObject rockImpact = Instantiate(rockImpactPrefab, transform.position + new Vector3(0, -0.55f, 0), Quaternion.identity);
            GameObject rockExplosion = Instantiate(rockExplosionPrefab, transform.position + new Vector3(-0.49f, 0.67f, 0), Quaternion.identity);

            // Trigger screen shake when the rock lands
            Camera.main.GetComponent<ScreenShake>().Shake();

            Destroy(gameObject);
            Destroy(rockImpact, animLength[0].length);
            Destroy(rockExplosion, animLength[1].length);
        }
    }

}
