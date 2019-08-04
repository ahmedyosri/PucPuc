using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class FireBallSystem : ReactiveSystem<InputEntity>
{
    GameContext gameContext;
    GameEntity primaryBall;
    
    public FireBallSystem(Contexts contexts) : base(contexts.input)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.MouseLeft, InputMatcher.MouseUp));
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasMouseUp;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach(var e in entities)
        {
            primaryBall = gameContext.GetGroup(GameMatcher.PrimaryBall).GetEntities()[0];

            Vector2 dir = e.mouseUp.position - primaryBall.position.pos;
            dir.Normalize();

            primaryBall.AddVelocity(dir);
            primaryBall.isPrimaryBall = false;
            primaryBall.isMoving = true;
        }
    }

}
