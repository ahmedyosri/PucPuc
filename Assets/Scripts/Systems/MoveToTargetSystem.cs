using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetSystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> movingEntities;
    Vector3 pos;

    public MoveToTargetSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        movingEntities = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.TargetPositions, GameMatcher.Moving));
    }

    public void Execute()
    {
        foreach (var e in movingEntities.GetEntities())
        {
            pos = Vector3.MoveTowards((Vector3)e.position.pos, e.targetPositions.positions[0], Time.deltaTime * e.targetPositions.speed);
            e.ReplacePosition(pos);

            if (pos == e.targetPositions.positions[0])
            {
                e.targetPositions.positions.RemoveAt(0);
            }

            if (e.targetPositions.positions.Count == 0)
            {
                e.isMoving = false;
                e.isReachedTarget = true;
                e.RemoveTargetPositions();
            }
        }
    }
}

