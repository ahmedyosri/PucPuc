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
        gameContext.ReplaceBoardManager(new GameEntity[BoardManager.width, BoardManager.length], 0, 0, 0);
        gameContext.isReady = false;
        CreateShootingBalls();
        CreateBoard();
    }

    private void CreateBoard()
    {
        for(int y=0; y<4; y++)
        {
            for(int x=0; x<BoardManager.width; x++)
            {
                var newEntity = gameContext.CreateEntity();
                newEntity.isBall = true;
                newEntity.isBallCollider = true;
                newEntity.AddBoardBall(
                    new Vector2(x, y),
                    Random.Range(1, 5),
                    y % 2 == 0 ? false : true
                );
                newEntity.AddPosition(Vector3.up * 8);
                gameContext.boardManager.entities[x, y] = newEntity;
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
