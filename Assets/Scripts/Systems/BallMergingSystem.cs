using System.Collections.Generic;
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
        foreach (GameEntity e in entities)
        {
            GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
            e.Destroy();
        }
    }

}