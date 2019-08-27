using Entitas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardBalancerSystem : IExecuteSystem
{
    GameContext gameContext;
    bool isAnyAnimating;
    IGroup<GameEntity> movingEntitiesGroup;

    public static int nextRecommendedValue;

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

        int maxDepth = 0;
        int avgValue = 0;
        int entitiesCount = 0;
        GameEntity[,] entities = gameContext.boardManager.entities;
        List<int> possibleValues = new List<int>();

        for(int x=0; x<BoardManager.width; x++)
        {
            for(int y=0; y<BoardManager.length; y++)
            {
                if (entities[x, y] == null)
                    continue;

                maxDepth = Mathf.Max(maxDepth, y);

                entitiesCount++;
                avgValue += entities[x, y].boardBall.value;

                if (GameUtils.GetChildrenFor(entities[x, y].boardBall).Count < 2)
                    possibleValues.Add(entities[x, y].boardBall.value);
            }
        }

        if (entitiesCount < BoardManager.width * 3 && maxDepth <= 5)
        {
            ShiftBoardDown();
        }
        else if (maxDepth < 3)
        {
            ShiftBoardDown();
        }
        else if (maxDepth > 6)
        {
            ShiftBoardUp();
        }

        possibleValues.Sort();

        if (possibleValues.Count == 0)
            possibleValues.Add(1);

        int minVal, maxVal;
        minVal = possibleValues[0];
        maxVal = possibleValues[possibleValues.Count - 1];
        if(minVal > 1)
        {
            possibleValues.Add(minVal - 1);
        }
        if(maxVal < 11)
        {
            possibleValues.Add(maxVal + 1);
        }

        nextRecommendedValue = possibleValues[Random.Range(0, possibleValues.Count)];
    }

    private void ShiftBoardDown()
    {
        GameEntity[,] entities = gameContext.boardManager.entities;

        bool isShifted = false;
        for (int x = 0; x < BoardManager.width; x++)
        {
            if (entities[x, 0] == null)
                continue;
            isShifted = !entities[x, 0].boardBall.shifted;
            break;
        }

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

        for(int x=0; x<BoardManager.width; x++)
        {
            var newEntity = gameContext.CreateEntity();
            newEntity.isBall = true;
            newEntity.isBallCollider = true;
            newEntity.AddBoardBall(
                new Vector2(x, 0),
                Random.Range(1, 5),
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
