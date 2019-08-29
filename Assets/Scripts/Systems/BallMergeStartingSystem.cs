using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallMergeStartingSystem : IExecuteSystem
{

    GameContext gameContext;
    bool startMerge, oldStartMerge;
    IGroup<GameEntity> impactingEntities;

    public BallMergeStartingSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        startMerge = oldStartMerge = false;
    }

    public void Execute()
    {
        impactingEntities = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Impacting, GameMatcher.ReachedTarget));

        if (impactingEntities.count == 0)
        {
            return;
        }

        if (impactingEntities.count != gameContext.boardManager.impactingEntitesCount)
        {
            return;
        }

        // 1- Cleanup impacting data
        List<GameEntity> eList = impactingEntities.GetEntities().ToList();
        foreach(var entity in eList)
        {
            entity.isImpacting = false;
            entity.isReachedTarget = false;
        }
        gameContext.boardManager.impactingEntitesCount = 0;

        // 2- Start merging process
        GameEntity e = gameContext.GetGroup(GameMatcher.AddToBoard).GetEntities()[0];
        e.isAddToBoard = false;
        e.isMoving = false;
        e.isReachedTarget = false;

        gameContext.boardManager.scoreMultiplier = 1;
        GameUtils.MergeNeighborsOf(e);
    }
}
