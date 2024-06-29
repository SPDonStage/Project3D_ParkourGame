using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setting")]
    public Color HealthColor;
    public Color HealthBarBackGroundColor;
    public Sprite HealthBarBackGroundSprite;
    [Range(1f, 100f)]
    public int Alert = 20;
    public Color BarAlertColor;

    [Header("Image")]
    public Image Healthbar, HealthbarBackground;
    public float barValue;
    public float BarValue
    {
        get { return barValue; }

        set
        {
            value = Mathf.Clamp(value, 0, 100);
            barValue = value;
            UpdateValue(barValue);

        }
    }



    private void Awake()
    {
        Healthbar = transform.Find("Health").GetComponent<Image>();
        HealthbarBackground = GetComponent<Image>();
        HealthbarBackground = transform.Find("HealthBar").GetComponent<Image>();
    }

    private void Start()
    {
        Healthbar.color = HealthColor;
        HealthbarBackground.color = HealthBarBackGroundColor;
        HealthbarBackground.sprite = HealthBarBackGroundSprite;

        UpdateValue(barValue);


    }

    void UpdateValue(float val)
    {
        Healthbar.fillAmount = val / 100;

        if (Alert >= val)
        {
            Healthbar.color = BarAlertColor;
        }
        else
        {
            Healthbar.color = HealthColor;
        }

    }


    private void Update()
    {
            UpdateValue(50);
            Healthbar.color = HealthColor;

            Healthbar.color = HealthColor;
            HealthbarBackground.color = HealthBarBackGroundColor;

            HealthbarBackground.sprite = HealthBarBackGroundSprite;
    }
}
