using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Rocket : MonoBehaviour
{
    private enum State {
        Startup,
        GoingForward,
        ChaseTarget,
        Explosion
    }
    private State state;
    public float shootSFXMinDistance = 2.0f;
    public float explosionSFXMinDistance = 10.0f;
    public float moveSpeed = 100f;
    public float distanceToExplode = 1;
    public float explosionRadius = 1;
    public GameObject explosionVFX;
    public float explosionScale = 2;
    private Vector3 shootDir;
    private Vector3 initialVelocity;
    private new Rigidbody rigidbody;
    private bool isLookingForEnemy = true;
    public float timeBeforeChasing = 0.4f;
    private int damageAmount;
    private AudioSource audioSource;
    public AudioClip audioOnShoot;
    public AudioClip audioOnExplosion;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = audioOnShoot;
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = shootSFXMinDistance;
        audioSource.maxDistance = 1000.0f;
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 shootDir, Vector3 initialVelocity, int damageAmount, bool isLookingForEnemy = true)
    {
        this.shootDir = shootDir;
        transform.localRotation = Quaternion.Euler(90,180,90);
        this.isLookingForEnemy = isLookingForEnemy;
        rigidbody.velocity = initialVelocity;
        Destroy(gameObject, 10f);
        this.damageAmount = damageAmount;
        audioSource.Play();
    }

    private void FixedUpdate()
    {
        if (rigidbody == null) return;
        switch (state) {
            default:
            case State.Startup:
                // Wait for timeBeforeChasing time, until then, move forward
                StartCoroutine(ChangeToChasingInSeconds(timeBeforeChasing));
                state = State.GoingForward;
                break;
            case State.GoingForward:
                rigidbody.AddForce(shootDir * Time.deltaTime * moveSpeed * 200);
                break;
            case State.ChaseTarget:
                GameObject[] enemies;
                // Check enemies nearby
                if (this.isLookingForEnemy) enemies = GameObject.FindGameObjectsWithTag("Enemy");
                else enemies = GameObject.FindGameObjectsWithTag("PlayerTag");
                // Find nearest one
                GameObject target = jokyUtilities.GetClosestGameObject(enemies, transform);

                if (target != null)
                {
                    Vector3 vector = target.transform.position - transform.position;
                    rigidbody.AddForce(vector * Time.deltaTime * moveSpeed);
                    if (vector.magnitude < distanceToExplode) {
                        Explode();
                    }
                } 
                else 
                {
                    rigidbody.AddForce(transform.forward * Time.deltaTime * moveSpeed * 200);
                    Destroy(gameObject, 3f);
                }
                break;
        }
    }

    IEnumerator ChangeToChasingInSeconds(float delay) {
        yield return new WaitForSeconds(delay);
        state = State.ChaseTarget;
    }

    private void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders) {
            HealthData healthData = collider.gameObject.GetComponent<HealthData>();
            if (healthData == null && collider.gameObject.transform.parent != null) {
                healthData = collider.gameObject.transform.parent.GetComponent<HealthData>();
            }
            if (healthData != null)
            {
                // target hit
                healthData.Damage(damageAmount);
                // create VFX
                GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
                explosion.transform.localScale *= explosionScale * collider.gameObject.transform.localScale.x;
                // SFX
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.minDistance = explosionSFXMinDistance;
                audioSource.maxDistance = 750.0f;
                audioSource.PlayOneShot(audioOnExplosion);
                Destroy(explosion, 2f);
                if (rigidbody != null) Destroy(rigidbody); // stopping movement
                gameObject.transform.localScale = new Vector3(0,0,0); // hiding object, so SFX plays well
                Destroy(gameObject, 5f);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Explode();
    }
}
