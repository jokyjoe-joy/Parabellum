using System;
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
    }

    private void FixedUpdate() {
        CheckMovementControls();
        AdjustFOVOnSpeed();
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

        // TODO: Also finish these stuff! And use Override!!!!
        float aberrationIntensity = ship.currentVelocity.magnitude / 150f;
        chromaticAberration.intensity.value = Mathf.Clamp(aberrationIntensity, 0f, 1f);

        float lensDistortionIntensity = -(ship.currentVelocity.magnitude / 10);
        lensDistortion.intensity.value = Mathf.Clamp(lensDistortionIntensity, -20f, 0f);

        if (healthData.health < healthData.healthMax) {
            vignette.color.Override(Color.red);
            float vignetteIntensityValue = (healthData.healthMax - healthData.health) / healthData.healthMax;
            vignette.intensity.Override(Mathf.Clamp(vignetteIntensityValue,vignetteDefaultValue,0.65f));
            
            chromaticAberration.intensity.Override(1f);
        } else {
            vignette.intensity.Override(vignetteDefaultValue);
        }

    }

    
    private void ShakeCamera() {
        cameraShake.shakeDuration = 0.4f;
    }
}
