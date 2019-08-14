using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallColliderSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    public BallColliderSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BoardBall);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGameObject && (entity.gameObject.gameobject != null);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            e.gameObject.gameobject.GetComponent<Collider2D>().enabled = e.isBallCollider;
        }
    }
}
