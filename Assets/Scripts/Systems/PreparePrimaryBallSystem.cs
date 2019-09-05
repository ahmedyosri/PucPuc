using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePrimaryBallSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    public PreparePrimaryBallSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Ready));
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        GameEntity[] primaryBalls = gameContext.GetGroup(GameMatcher.PrimaryBall).GetEntities();
        if (primaryBalls.Length > 0)
            return;


        GameEntity secondaryBall = gameContext.GetGroup(GameMatcher.SecondaryBall).GetEntities()[0];
        secondaryBall.isSecondaryBall = false;
        secondaryBall.isPrimaryBall = true;

        
        secondaryBall.ReplaceTargetPositions(30, new List<Vector3>() { GameplayManager.Instance.PrimaryBallPosition.position });
        secondaryBall.isMoving = true;

        GameEntity newBallEntity = CreateBall(GameplayManager.Instance.ballCreationPosition.position);
        newBallEntity.isSecondaryBall = true;
        newBallEntity.ReplaceTargetPositions(30, new List<Vector3>() { GameplayManager.Instance.SecondaryBallPosition.position });
        newBallEntity.isMoving = true;
    }

    GameEntity CreateBall(Vector3 newPosition)
    {
        GameEntity e = gameContext.CreateEntity();
        e.AddBoardBall(Vector2.one * -10, BoardBalancerSystem.nextRecommendedValue, false);
        e.isBall = true;
        e.AddPosition(newPosition);
        return e;
    }
}

