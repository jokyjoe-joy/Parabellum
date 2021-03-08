using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CrosshairController : MonoBehaviour
{
    public WeaponSystem weaponSystem;
    private Image crosshair;
    private Color initialColor;

    private void Awake()
    {
        crosshair = GetComponent<Image>();
        initialColor = crosshair.color;
    }

    private void Update()
    {
        if (weaponSystem.hit.transform != null && weaponSystem.hit.transform.Find("EnemyTag") != null)
            crosshair.color = Color.red;
        else
            crosshair.color = initialColor;
    }
}
