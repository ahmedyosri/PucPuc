using Entitas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardBalancerSystem : IExecuteSystem
{
    GameContext gameContext;
    bool isAnyAnimating;
    IGroup<GameEntity> movingEntitiesGroup;

    public static int nextRecommendedValue;
    private int boardAvgValue;
    private int boardMeanValue;

    //Board size control
    readonly int maxBoardDepth = 6;
    readonly int minBoardDepth = 3;

    // row generation props
    readonly float fixedNegativeRad = 2.0f;
    readonly float scoreNegativeImpactRad = 3.0f;
    readonly float avgMinLimit = 6.0f;

    readonly float fixedPositiveRad = 2.0f;
    readonly float scorePositiveImpactRad = 4.0f;
    readonly float avgMaxLimit = 9.0f;

    readonly float effectiveScoreScale = 10000.0f;
    readonly float chanceToGetTwoSimilarBalls = 0.1f;

    public BoardBalancerSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        movingEntitiesGroup = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Moving));
    }

    public void Execute()
    {
        GameEntity[] movingEntities = movingEntitiesGroup.GetEntities();

        bool newIsAnyAnimating = (movingEntities.Length > 0);

        if (newIsAnyAnimating == isAnyAnimating)
            return;

        isAnyAnimating = newIsAnyAnimating;

        if (isAnyAnimating)
            return;

        int currDepth = 0;
        int entitiesCount = 0;
        GameEntity[,] entities = gameContext.boardManager.entities;
        List<int> possibleValues = new List<int>();
        HashSet<int> boardUniqueValues = new HashSet<int>();
        boardAvgValue = 0;

        for (int x=0; x<BoardManager.width; x++)
        {
            for(int y=0; y<BoardManager.length; y++)
            {
                if (entities[x, y] == null)
                    continue;

                currDepth = Mathf.Max(currDepth, y);

                entitiesCount++;
                boardAvgValue += entities[x, y].boardBall.value;
                boardUniqueValues.Add(entities[x, y].boardBall.value);

                if (GameUtils.GetChildrenFor(entities[x, y].boardBall).Count < 2)
                    possibleValues.Add(entities[x, y].boardBall.value);
            }
        }

        if (entitiesCount == 0)
        {
            GameplayManager.Instance.OnBoardCleared();
            boardAvgValue = 1;
        }
        else
        {
            boardAvgValue /= entitiesCount;
            List<int> uniqueList = boardUniqueValues.ToList();
            boardMeanValue = uniqueList[uniqueList.Count / 2];
        }

        if (possibleValues.Count == 0)
        {
            possibleValues.Add(1);
        }
        possibleValues.Sort();

        int minVal, maxVal;
        minVal = possibleValues[0];
        maxVal = possibleValues[possibleValues.Count - 1];
        if (minVal > 1)
        {
            possibleValues.Add(minVal - 1);
        }
        nextRecommendedValue = possibleValues[Random.Range(0, possibleValues.Count)];
        nextRecommendedValue = Mathf.Clamp(nextRecommendedValue, 1, 10);

        if (entitiesCount < BoardManager.width * 3 && currDepth <= 5)
        {
            ShiftBoardDown();
        }
        else if (currDepth < minBoardDepth)
        {
            ShiftBoardDown();
        }
        else if (currDepth > maxBoardDepth)
        {
            ShiftBoardUp();
        }
        else
        {
            gameContext.isReady = true;
        }
    }

    private void ShiftBoardDown()
    {
        GameEntity[,] entities = gameContext.boardManager.entities;

        bool isShifted = !GameUtils.IsRowShifted(0);

        for(int y=BoardManager.length-1; y>=1 ; y--)
        {
            for(int x=0; x<BoardManager.width; x++)
            {
                entities[x, y] = entities[x, y - 1];
                entities[x, y - 1] = null;

                if (entities[x, y] == null)
                    continue;

                entities[x, y].ReplaceBoardBall(new Vector2(x, y), entities[x, y].boardBall.value, entities[x, y].boardBall.shifted);
                entities[x, y].isMoving = true;
                entities[x, y].isReachedTarget = false;
            }
        }

        // How aggressive each new row is
        float scoreMeter = Mathf.Clamp(GameplayManager.Instance.CurrentScore, 0, effectiveScoreScale) / effectiveScoreScale;
        int avgMin = (int)Mathf.Lerp(1, avgMinLimit, scoreMeter * 0.5f + (boardMeanValue / 20.0f));
        int avgMax = (int)Mathf.Lerp(avgMinLimit, avgMaxLimit, scoreMeter * 0.5f + (boardMeanValue / 20.0f));

        int lastValue = Random.Range(avgMin, avgMax);
        int newVal = lastValue;
        Debug.Log(boardMeanValue + " | " + avgMin.ToString() + " | " + avgMax.ToString());

        for(int x=0; x<BoardManager.width; x++)
        {
            while (newVal == lastValue && Random.Range(0.0f, 1.0f) > chanceToGetTwoSimilarBalls)
            {
                newVal = Random.Range(avgMin, avgMax);
            }
            lastValue = newVal;

            var newEntity = gameContext.CreateEntity();
            newEntity.isBall = true;
            newEntity.isBallCollider = true;
            newEntity.AddBoardBall(
                new Vector2(x, 0),
                newVal,
                isShifted
            );
            var newPos = GameUtils.WorldPosForBall(newEntity) + Vector3.up;
            newEntity.AddPosition(newPos);
            entities[x, 0] = newEntity;
        }
    }

    private void ShiftBoardUp()
    {
        GameEntity[,] entities = gameContext.boardManager.entities;

        for (int x = 0; x < BoardManager.width; x++)
        {
            if (entities[x, 0] == null)
                continue;

            entities[x, 0].RemoveBoardBall();
            entities[x, 0].AddTargetPositions(
                GameplayManager.Instance.ballMergeSpeed,
                new List<Vector3>() { entities[x, 0].position.pos + Vector2.up }
            ) ;
            entities[x, 0].isMoving = true;
            entities[x, 0].isReachedTarget = false;
            entities[x, 0].AddAutoDestroy(0.25f);
            entities[x, 0] = null;
        }

        for(int x=0; x<BoardManager.width; x++)
        {
            for(int y=1; y<BoardManager.length; y++)
            {
                GameEntity e = entities[x, y];
                if (e == null)
                    continue;
                e.ReplaceBoardBall(e.boardBall.boardIdx - Vector2.up, e.boardBall.value, e.boardBall.shifted);
                e.isMoving = true;
                e.isReachedTarget = false;
                entities[x, y - 1] = entities[x, y];
                entities[x, y] = null;
            }
        }
    }
}
