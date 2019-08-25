using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

public class BallMergingSystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> mergingEntities;

    public BallMergingSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        mergingEntities = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.MergeTo, GameMatcher.ReachedTarget));
    }

    public void Execute()
    {
        GameEntity[] entities = mergingEntities.GetEntities();

        if (entities.Length == 0)
        {
            return;
        }

        foreach (GameEntity ent in entities)
        {
            GameplayManager.Instance.DeleteBall(ent.gameObject.gameobject);
            ent.Destroy();
            gameContext.boardManager.mergingEntitiesCount--;
        }

        if (gameContext.boardManager.mergingEntitiesCount > 0)
            return;

        // 2- Start merging process
        GameEntity e = gameContext.GetGroup(GameMatcher.AddToBoard).GetEntities()[0];
        e.isAddToBoard = false;
        e.isReachedTarget = false;

        GameUtils.MergeNeighborsOf(e);
    }

}