using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;
    public TemplateSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        throw new System.NotImplementedException();
    }

    protected override bool Filter(GameEntity entity)
    {
        throw new System.NotImplementedException();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        throw new System.NotImplementedException();
    }

}

