using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class BallCreatorSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    public BallCreatorSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Ball);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isBall && !entity.hasGameObject;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            GameObject newObj = GameplayManager.Instance.CreateBall();

            e.AddGameObject(newObj);

            if (!e.hasPosition)
                e.AddPosition(Vector2.one * -10);

            if (!e.hasBoardBall)
                e.AddBoardBall(Vector2.one * -10,Random.Range(1, 5), false);

            newObj.Link(e);
        }
    }
}
