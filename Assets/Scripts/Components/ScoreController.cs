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
    Text topScoreText;

    [SerializeField]
    int animationSpeed;

    float currentValue;
    float targetValue;
    float topScore;

    public int CurrentScore
    {
        get
        {
            return (int) targetValue;
        }
     }

    // Start is called before the first frame update
    void Start()
    {
        currentValue = targetValue = 0;
        topScore = PlayerPrefs.GetFloat("TopScore");
        topScoreText.text = topScore.ToString();
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

    public void AddScore(float addedScore)
    {
        targetValue += addedScore;
        if(targetValue > topScore)
        {
            topScore = targetValue;
            topScoreText.text = topScore.ToString();
            PlayerPrefs.SetFloat("TopScore", topScore);
        }
    }
}
