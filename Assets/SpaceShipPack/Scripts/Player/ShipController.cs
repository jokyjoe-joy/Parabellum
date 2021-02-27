using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ShipController : MonoBehaviour
{
    private float defaultFOV;
    private float currentFOV;
    private Ship ship;
    private WeaponSystem weaponSystem;
    private PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Vignette vignette;
    private float vignetteDefaultValue;
    private CameraShake cameraShake;
    private HealthData healthData;
    [HideInInspector] public GameObject currentTarget = null;
    

    private void Awake() {
        weaponSystem = GetComponent<WeaponSystem>();
        healthData = GetComponent<HealthData>();
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
        postProcessVolume.profile.TryGetSettings(out lensDistortion);
        postProcessVolume.profile.TryGetSettings(out vignette);
        vignetteDefaultValue = vignette.intensity.value;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        healthData.onDamage.AddListener(ShakeCamera);
    }

    void Start()
    {
        // Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        ship = GetComponent<Ship>();
        defaultFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        // Create Shoot event on Left Click
        if (Input.GetMouseButtonDown(0))
        {
            weaponSystem.ShootGuns(ship.currentVelocity);
        }
        if (Input.GetKeyDown("t")) {

            // Try to target enemy
            // Check enemies nearby
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject target = jokyUtilities.GetClosestGameObject(enemies, transform);
            
            if (target != null) {
                // FIXME: isOffScreen doesn't work properly! Sometimes when object is offScreen
                // it still returns false
                bool isOffScreen = jokyUtilities.checkIfObjectIsOffScreen(target.transform.position);
                // In case Player doesn't have a target, try to target the closest enemy
                // which is on-screen.
                if (currentTarget == null && !isOffScreen) {
                    // Invoke onTargeted event on the targeted ship
                    // Below parent is needed, as enemyTag is its child gameobject
                    currentTarget = target.transform.parent.gameObject;
                    Ship enemyShip = target.transform.parent.GetComponent<Ship>();
                    if (enemyShip != null) enemyShip.onTargeted.Invoke();
                } else {
                    // If there is only one enemy nearby, then turn off targeting
                    if (enemies.Length == 1) {
                        currentTarget = null;
                    } else {
                        // TODO: This should not select randomly, but selecting in order of
                        // distance (first the ones close, then the ones far)
                        currentTarget = enemies[Random.Range(0,enemies.Length)].transform.parent.gameObject;
                    }
                }
            }

        }
    }

    private void FixedUpdate() {
        CheckMovementControls();
        AdjustFOVOnSpeed();
        AdjustPostProcessing();
    }

    private void CheckMovementControls()
    {
        ship.LookUpDown(Input.GetAxis("Mouse Y"));
        ship.LookLeftRight(Input.GetAxis("Mouse X"));

        if (Input.GetKey("w"))
        {
            ship.ThrustForward();
        }
        if (Input.GetKey("s"))
        {
            ship.ThrustBackwards();
        }
        if (Input.GetKey("a"))
        {
            ship.RollLeft();
        }
        if (Input.GetKey("d"))
        {
            ship.RollRight();
        }
        if (Input.GetKey("space"))
        {
            ship.Stabilise();
        }
    }

    private void AdjustFOVOnSpeed()
    {
        // Dynamic field of view based on speed
        if (ship.currentVelocity.magnitude > 10)
        {
            currentFOV = defaultFOV + ship.currentVelocity.magnitude / 10;
        }
        else
        {
            currentFOV = 60;
        }

        // If right-clicking, zoom
        if (Input.GetMouseButton(1))
        {
            currentFOV /= 2;
        }

        // setting fov
        Camera.main.fieldOfView = currentFOV;
    }

    private void AdjustPostProcessing() {
        float aberrationIntensity = ship.currentVelocity.magnitude / 150f;
        if (chromaticAberration != null) chromaticAberration.intensity.Override(Mathf.Clamp(aberrationIntensity, 0f, 1f));

        float lensDistortionIntensity = -(ship.currentVelocity.magnitude / 10);
        if (lensDistortion != null) lensDistortion.intensity.Override(Mathf.Clamp(lensDistortionIntensity, -20f, 0f));

        if (healthData.health < healthData.healthMax && vignette != null && chromaticAberration != null) {
            vignette.color.Override(Color.red);
            float vignetteIntensityValue = (healthData.healthMax - healthData.health) / healthData.healthMax;
            vignette.intensity.Override(Mathf.Clamp(vignetteIntensityValue,vignetteDefaultValue,0.65f));
            
            chromaticAberration.intensity.Override(1f);
        } else {
            if (vignette != null) vignette.intensity.Override(vignetteDefaultValue);
        }
    }

    
    private void ShakeCamera() {
        cameraShake.shakeDuration = 0.4f;
    }
}
