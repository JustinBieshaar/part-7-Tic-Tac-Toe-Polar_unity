using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
    private Slider _slider;
    private TextMeshProUGUI _valueText;

    private void Start() {
        _slider = GetComponent<Slider>();
        _valueText = GetComponentInChildren<TextMeshProUGUI>();

        if (_valueText != null) {
            _valueText.text = _slider.value.ToString();
            _slider.onValueChanged.AddListener(value => _valueText.text = value.ToString());
        }
    }
}