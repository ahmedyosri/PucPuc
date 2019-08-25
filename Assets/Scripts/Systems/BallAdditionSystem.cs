using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallAdditionSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    public BallAdditionSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.AddToBoard, GameMatcher.ReachedTarget));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isAddToBoard;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (GameEntity e in entities)
        {
            if(e.isReachedTarget)
                ApplyImpact(e);

            e.isMoving = false;
            e.isReachedTarget = false;
            e.isBallCollider = true;

            gameContext.boardManager.entities[(int)e.boardBall.boardIdx.x, (int)e.boardBall.boardIdx.y] = e;
        }
    }

    void ApplyImpact(GameEntity e)
    {
        List<Vector2> nodeneighbors = GameUtils.GetNeighborsFor(e);

        foreach (Vector2 nodeShift in nodeneighbors)
        {
            Vector2 nodeIdx = e.boardBall.boardIdx + nodeShift;
            if (nodeIdx.x < 0 || nodeIdx.y < 0 || nodeIdx.x >= BoardManager.width || nodeIdx.y >= BoardManager.length)
            {
                continue;
            }

            GameEntity neighbor = gameContext.boardManager.entities[(int)nodeIdx.x, (int)nodeIdx.y];
            if (neighbor == null)
                continue;

            Vector3 pos = neighbor.position.pos;
            Vector3 newPos = pos - (Vector3)e.position.pos;
            newPos = newPos.normalized * 0.1f;

            neighbor.AddTargetPositions(1.0f, new List<Vector3>() { pos + newPos, pos });
            neighbor.isMoving = true;
            neighbor.isReachedTarget = false;
            neighbor.isImpacting = true;

            gameContext.boardManager.impactingEntitesCount++;
        }
    }

}