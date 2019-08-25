using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroySystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> autoDestroyEntities;
    List<GameEntity> toDestroy;

    public AutoDestroySystem(Contexts contexts)
    {
        gameContext = contexts.game;
        autoDestroyEntities = gameContext.GetGroup(GameMatcher.AutoDestroy);
         toDestroy = new List<GameEntity>();
    }

    public void Execute()
    {
        toDestroy.Clear();
        foreach(GameEntity e in autoDestroyEntities.GetEntities())
        {
            e.autoDestroy.afterSeconds -= Time.deltaTime;
            if(e.autoDestroy.afterSeconds <= 0)
            {
                toDestroy.Add(e);
            }
        }

        foreach(GameEntity e in toDestroy)
        {
            if(e.isBall)
                GameplayManager.Instance.DeleteBall(e.gameObject.gameobject);
            e.Destroy();
        }
    }

}
