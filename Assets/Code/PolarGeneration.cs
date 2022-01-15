using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PolarGeneration : MonoBehaviour {
    private const int CROSS = 4;

    [SerializeField] private GameObject _lineDrawer;
    [SerializeField] private GameObject _lineDrawerCross;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private Transform _parent;

    [Space(10), SerializeField] private Slider _linesSlider;
    [SerializeField] private Slider _rayLinesSlider;
    [SerializeField] private Slider _smoothnessSlider;
    [SerializeField] private Slider _depthValueSlider;
    [SerializeField] private Slider _sphereValueSlider;
    [SerializeField] private Slider _radiusValueSlider;
    [SerializeField] private Slider _radiusOffsetValueSlider;
    [SerializeField] private Slider _widthValueSlider;
    [SerializeField] private Slider _widthRayValueSlider;

    private readonly List<LineRenderer> _circleLines = new List<LineRenderer>();
    private readonly List<LineRenderer> _rayLines = new List<LineRenderer>();
    private readonly List<GameObject> _hitBoxes = new List<GameObject>();
    private int _size;

    //private float anglePerDepth => (90f + 90f * _sphereValueSlider.value) / (_linesSlider.value - 1);

    private void Start() {
        //Clear();
        GameManager.Instance.SetBoard(this);

        _linesSlider.onValueChanged.AddListener(OnSliderChanged);
        _rayLinesSlider.onValueChanged.AddListener(OnSliderChanged);
        _smoothnessSlider.onValueChanged.AddListener(OnSliderChanged);
        _depthValueSlider.onValueChanged.AddListener(OnSliderChanged);
        _sphereValueSlider.onValueChanged.AddListener(OnSliderChanged);
        _radiusValueSlider.onValueChanged.AddListener(OnSliderChanged);
        _radiusOffsetValueSlider.onValueChanged.AddListener(OnSliderChanged);
        _widthValueSlider.onValueChanged.AddListener(OnSliderChanged);
        _widthRayValueSlider.onValueChanged.AddListener(OnSliderChanged);

        OnSliderChanged();
    }

    private void OnSliderChanged(float value = 0) {
        var circles = (int)_linesSlider.value;
        var rays = (int)_rayLinesSlider.value;
        var smootheness = _smoothnessSlider.value;
        var radius = _radiusValueSlider.value;
        var radiusOffset = _radiusOffsetValueSlider.value;
        var depth = _depthValueSlider.value;
        var sphere = _sphereValueSlider.value;
        var circleWidth = _widthValueSlider.value;
        var rayWidth = _widthRayValueSlider.value;
        GameManager.Instance.Clear();
        GameManager.Instance.Set(rays, circles);

        Clear();
        CreateLines(circles, rays);
        Generate(circles,
            smootheness,
            radius,
            radiusOffset,
            circleWidth,
            depth,
            sphere);

        GenerateRays(circles,
            rays,
            smootheness,
            radius,
            radiusOffset,
            rayWidth,
            depth,
            sphere);
    }

    public void Generate() {
        OnSliderChanged();
    }


    void Generate(int circlesValue, float smoothnessValue, float radiusValue, float radiusOffsetValue,
        float widthValue, float depthValue, float sphereValue) {
        var linesCount = (int)(smoothnessValue * circlesValue);
        var anglePerLine = 360f / linesCount;
        var anglePerDepth = (90f + 90f * sphereValue) / (circlesValue - 1f);

        for (int i = 0; i < circlesValue; i++) {
            var line = _circleLines[i];
            line.positionCount = linesCount + 1;
            line.startWidth = widthValue;
            line.endWidth = widthValue;

            var offset = radiusOffsetValue * i;
            var depthTheta = anglePerDepth * i * Mathf.Deg2Rad;

            if (sphereValue != 0 && depthValue != 0) {
                offset = radiusOffsetValue * (circlesValue - 1f) * Mathf.Sin(depthTheta);
            }

            var radius = radiusValue + offset;

            var centeredIndex = (circlesValue - 1f) / 2f - i;
            var depth = sphereValue == 0
                ? centeredIndex * depthValue / 2f
                : depthValue * Mathf.Cos(Mathf.PI * i / (circlesValue - 1f));

            for (int j = 0; j < linesCount + 1; j++) {
                var theta = anglePerLine * j * Mathf.Deg2Rad;

                var x = radius * Mathf.Cos(theta);
                var y = radius * Mathf.Sin(theta);
                line.SetPosition(j, new Vector3(x, y, depth));
            }
        }
    }


    private void GenerateRays(int circlesValue, int rayLinesValue, float smoothnessValue, float radiusValue,
        float radiusOffsetValue, float rayWidthValue, float depthValue, float sphereValue) {
        var countPerSection = (int)(2 * smoothnessValue);
        var linesCount = (circlesValue - 1) * countPerSection;

        var anglePerLine = 360f / rayLinesValue;
        var anglePerDepth = (90f + 90f * sphereValue) / (circlesValue - 1f);
        for (int i = 0; i < rayLinesValue; i++) {
            var line = _rayLines[i];
            line.positionCount = linesCount;
            line.startWidth = rayWidthValue;
            line.endWidth = rayWidthValue;
            float angle = anglePerLine * i * Mathf.Deg2Rad;

            for (int j = 0; j < linesCount; j++) {
                var radiusIndex = j / countPerSection;
                var stepIndex = j % countPerSection;
                var step = radiusIndex + 1f / (countPerSection - 1) * stepIndex;
                var offset = radiusOffsetValue * step;
                var depthTheta = anglePerDepth * step * Mathf.Deg2Rad;

                if (sphereValue != 0 && depthValue != 0) {
                    offset = radiusOffsetValue * (circlesValue - 1f) * Mathf.Sin(depthTheta);
                }

                var centeredIndex = (circlesValue - 1f) / 2f - step;
                var depth = sphereValue == 0
                    ? centeredIndex * depthValue / 2f
                    : depthValue * Mathf.Cos(Mathf.PI * step / (circlesValue - 1f));

                var position = radiusValue + offset;
                var x = position * Mathf.Cos(angle);
                var y = position * Mathf.Sin(angle);

                line.SetPosition(j, new Vector3(x, y, depth));
            }

            GenerateHitBoxes(circlesValue, radiusValue, radiusOffsetValue, depthValue, sphereValue, angle, i);
        }
    }


    private void GenerateHitBoxes(int circlesValue, float radiusValue, float radiusOffsetValue, float depthValue,
        float sphereValue, float angle, int index) {
        var anglePerDepth = (90f + 90f * sphereValue) / (circlesValue - 1f);

        for (int i = 0; i < circlesValue; i++) {
            var offset = radiusOffsetValue * i;
            var theta = anglePerDepth * i * Mathf.Deg2Rad;

            if (circlesValue > 1 && sphereValue != 0 && depthValue > 0) {
                // max offset * arc
                offset = radiusOffsetValue * (circlesValue - 1) * Mathf.Sin(theta);
            }

            var centeredIndex = (circlesValue - 1f) / 2f - i;
            var depth = sphereValue == 0
                ? centeredIndex * depthValue / 2
                : depthValue * Mathf.Cos(Mathf.PI * i / (circlesValue - 1));

            var position = radiusValue + offset;
            var x = position * Mathf.Cos(angle);
            var y = position * Mathf.Sin(angle);

            //instantiate hitbox
            var hitBox = Instantiate(_hitbox, _parent).GetComponent<HitBox>();
            hitBox.name = "hitBox_" + index + "-" + i;
            hitBox.transform.SetPosition(x, y, depth);
            _hitBoxes.Add(hitBox.gameObject);
            GameManager.Instance.AddHitBox(hitBox, index, i);
        }
    }


    private void CreateLines(int circles, int rays) {
        for (int i = 0; i < circles; i++) {
            var line = Instantiate(_lineDrawer, _parent)
                .GetComponent<LineRenderer>();
            line.name = $"CircleLine{i}";
            _circleLines.Add(line);
        }

        _rayLines.Clear();
        for (int i = 0; i < rays; i++) {
            var line = Instantiate(_lineDrawerCross, _parent)
                .GetComponent<LineRenderer>();
            line.name = $"CrossLine{i}";
            _rayLines.Add(line);
        }
    }

    public void Clear() {
        foreach (var hitbox in _hitBoxes) {
            Destroy(hitbox);
        }

        _hitBoxes.Clear();
        foreach (var lineRenderer in _circleLines) {
            DestroyImmediate(lineRenderer);
        }

        _circleLines.Clear();

        foreach (var lineRenderer in _rayLines) {
            DestroyImmediate(lineRenderer);
        }

        _circleLines.Clear();
    }
}