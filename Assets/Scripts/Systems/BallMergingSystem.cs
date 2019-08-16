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
        mergingEntities = gameContext.GetGroup(GameMatcher.MergeTo);
    }

    public void Execute()
    {
        GameEntity[] entities = mergingEntities.GetEntities();
        foreach(GameEntity e in entities)
        {
            if(!e.mergeTo.mergeTo.hasGameObject)
            {
                GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
                e.Destroy();
                continue;
            }

            e.ReplacePosition(Vector2.Lerp(e.position.pos, e.mergeTo.mergeTo.position.pos, Time.deltaTime * 10));
            if (Vector2.Distance(e.position.pos, e.mergeTo.mergeTo.position.pos) < 0.1f)
            {
                GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
                e.Destroy();
            }
        }
    }

}