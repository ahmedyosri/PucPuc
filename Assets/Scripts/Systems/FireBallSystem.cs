using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class FireBallSystem : ReactiveSystem<InputEntity>
{
    GameContext gameContext;
    GameEntity primaryBall;

    Transform secondaryPosition;
    Transform entitiesParent;

    public FireBallSystem(Contexts contexts) : base(contexts.input)
    {
        gameContext = contexts.game;

        entitiesParent = GameObject.Find("EntitiesParent").transform;
        secondaryPosition = GameObject.Find("SecondaryPosition").transform;
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

            primaryBall.isPrimaryBall = false;
            primaryBall.isMoving = true;
            primaryBall.isBallCollider = true;

            GameEntity secondaryBall = gameContext.GetGroup(GameMatcher.SecondaryBall).GetEntities()[0];
            secondaryBall.isSecondaryBall = false;
            secondaryBall.isPrimaryBall = true;
            secondaryBall.ReplacePosition(primaryBall.position.pos);

            GameEntity newBallEntity = CreateBall(secondaryPosition);
            newBallEntity.isSecondaryBall = true;

            GameplayManager.Instance.AimLine.positionCount = 0;
        }
    }

    GameEntity CreateBall(Transform newParent)
    {
        GameEntity e = gameContext.CreateEntity();
        e.isBall = true;
        e.AddPosition(newParent.position);
        return e;
    }

}
