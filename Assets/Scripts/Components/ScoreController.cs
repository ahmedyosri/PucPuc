using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup scoreGroup;
    [SerializeField]
    Text scoreText;

    [SerializeField]
    int animationSpeed;

    float currentValue;
    float targetValue;

    // Start is called before the first frame update
    void Start()
    {
        currentValue = targetValue = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(scoreGroup.alpha < 1)
            scoreGroup.alpha += 2 * Time.fixedDeltaTime;

        if (currentValue == targetValue)
            return;

        currentValue = Mathf.Clamp(currentValue+ Time.deltaTime * animationSpeed, 0.0f, targetValue);
        scoreText.text = ((int)currentValue).ToString();
    }

    public void UpdateScore(float newTargetScore)
    {
        targetValue = newTargetScore;
    }
}
