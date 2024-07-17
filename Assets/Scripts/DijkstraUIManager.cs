using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DijkstraUIManager : MonoBehaviour
{
    public Slider moveSpeedSlider;
    public Slider avoidanceRadiusSlider;
    public Slider avoidanceStrengthSlider;
    public Slider initialPositionOffsetSlider;

    private float moveSpeed;
    private float avoidanceRadius;
    private float avoidanceStrength;
    private float initialPositionOffset;

    void Start()
    {
        moveSpeed = moveSpeedSlider.value;
        avoidanceRadius = avoidanceRadiusSlider.value;
        avoidanceStrength = avoidanceStrengthSlider.value;
        initialPositionOffset = initialPositionOffsetSlider.value;


        moveSpeedSlider.onValueChanged.AddListener(OnMoveSpeedChanged);
        avoidanceRadiusSlider.onValueChanged.AddListener(OnAvoidanceRadiusChanged);
        avoidanceStrengthSlider.onValueChanged.AddListener(OnAvoidanceStrengthChanged);
        initialPositionOffsetSlider.onValueChanged.AddListener(OnInitialPositionOffsetChanged);
    }

    void OnMoveSpeedChanged(float value)
    {
        moveSpeed = value;
    }

    void OnAvoidanceRadiusChanged(float value)
    {
        avoidanceRadius = value;
    }

    void OnAvoidanceStrengthChanged(float value)
    {
        avoidanceStrength = value;
    }

    void OnInitialPositionOffsetChanged(float value)
    {
        initialPositionOffset = value;
    }

    public float GetMoveSpeed() => moveSpeed;
    public float GetAvoidanceRadius() => avoidanceRadius;
    public float GetAvoidanceStrength() => avoidanceStrength;
    public float GetInitialPositionOffset() => initialPositionOffset;
}
