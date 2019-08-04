using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class BallCreatorSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    Transform secondaryPosition;
    GameObject ballPrefab;

    public BallCreatorSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
        secondaryPosition = GameObject.Find("SecondaryPosition").transform;
        ballPrefab = Resources.Load("Ball") as GameObject;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Ball);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasBall && !entity.hasObject;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            GameObject newObj = GameObject.Instantiate(ballPrefab) as GameObject;
            newObj.transform.SetParent(secondaryPosition);

            e.AddObject(newObj);
            newObj.Link(e);
        }
    }
}
