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
            ent.gameObject.gameobject.GetComponent<TrailRenderer>().enabled = false;
            GameplayManager.Instance.DeleteBall(ent.gameObject.gameobject);
            ent.Destroy();
            gameContext.boardManager.mergingEntitiesCount--;
        }

        if (gameContext.boardManager.mergingEntitiesCount > 0)
            return;

        SoundManager.Instance.collisionSound.Play();

        // 2- Start merging process
        GameEntity[] addToBoardEntities = gameContext.GetGroup(GameMatcher.AddToBoard).GetEntities();
        if (addToBoardEntities.Length == 0)
            return; // It wasn't properly selected, hence was deleted

        GameEntity e = addToBoardEntities[0];
        e.isAddToBoard = false;
        e.isReachedTarget = false;

        GameUtils.MergeNeighborsOf(e);
    }

}