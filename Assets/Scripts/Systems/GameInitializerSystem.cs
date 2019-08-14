using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameInitializerSystem : IInitializeSystem
{
    GameContext gameContext;

    public GameInitializerSystem(Contexts contexts)
    {
        gameContext = contexts.game;
    }

    public void Initialize()
    {
        gameContext.ReplaceBoardManager(new GameEntity[10, 6]);
        CreateShootingBalls();
        CreateBoard();
    }

    private void CreateBoard()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<6; j++)
            {
                var newEntity = gameContext.CreateEntity();
                newEntity.isBall = true;
                newEntity.isBallCollider = true;
                newEntity.AddBoardBall(
                    new Vector2(j, i),
                    (int)Mathf.Pow(2, Random.Range(1, 5)),
                    i % 2 == 0 ? false : true
                );

                gameContext.boardManager.entities[i, j] = newEntity;
            }
        }
    }

    private void CreateShootingBalls()
    {
        GameEntity tmpEntity = CreateBall(GameplayManager.Instance.PrimaryBallPosition);
        tmpEntity.isPrimaryBall = true;

        tmpEntity = CreateBall(GameplayManager.Instance.SecondaryBallPosition);
        tmpEntity.isSecondaryBall = true;
    }

    GameEntity CreateBall(Transform newParent)
    {
        GameEntity e = gameContext.CreateEntity();
        e.isBall = true;
        e.AddPosition(newParent.position);
        return e;
    }

}
