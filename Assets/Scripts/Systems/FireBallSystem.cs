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
        GameEntity[] primaryBalls = gameContext.GetGroup(GameMatcher.PrimaryBall).GetEntities();
        if (primaryBalls.Length == 0)
            return;
        primaryBall = primaryBalls[0];

        foreach (var e in entities)
        {
            Vector2 dir = e.mouseUp.position - primaryBall.position.pos;
            dir.Normalize();

            primaryBall.isPrimaryBall = false;
            primaryBall.isMoving = true;
            primaryBall.isBallCollider = true;

            GameplayManager.Instance.AimLine.positionCount = 0;
        }
    }

}
