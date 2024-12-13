using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensValText;

    private const string MouseSensitivityKey = "MouseSensitivity";
    // Start is called before the first frame update
    void Start()
    {
        float savedSens = PlayerPrefs.GetFloat(MouseSensitivityKey, 1.0f);
        sensitivitySlider.value = savedSens;
        UpdateValueText(savedSens);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value) {
        UpdateValueText(value);

        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        PlayerPrefs.Save();
    }

    private void UpdateValueText(float value) {
        sensValText.text = "Sensitivity: " + value.ToString("F2");
    }
}
