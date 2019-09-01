using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Start,
        Running,
        Ended
    }

    [SerializeField]
    RectTransform startUi;

    [SerializeField]
    RectTransform gameplayUi;

    [SerializeField]
    GameplayManager gameplayManager;

    public void OnStartGame(float delay)
    {
        StartCoroutine(SwitchToGameplay(delay));
    }

    private IEnumerator SwitchToGameplay(float delay)
    {
        while(delay > 0)
        {
            delay -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        startUi.gameObject.SetActive(false);
        gameplayUi.gameObject.SetActive(true);
        gameplayManager.gameObject.SetActive(true);

        yield break;
    }
}
