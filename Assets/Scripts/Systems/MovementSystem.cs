using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class MovementSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    float speed = 2.0f;
    float borderLimit = 2.5f;

    public MovementSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Velocity);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasVelocity;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            e.ReplacePosition(e.position.pos + e.velocity.direction * Time.deltaTime * speed);

            if(e.position.pos.x > borderLimit)
            {
                e.velocity.direction.x *= -1;
                e.position.pos.x = borderLimit;
            }
            else if(e.position.pos.x < -borderLimit)
            {
                e.velocity.direction.x *= -1;
                e.position.pos.x = -borderLimit;
            }
        }
    }

}
