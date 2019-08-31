using Entitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineModifierSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;
    Material lineMaterial;

    public LineModifierSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
        lineMaterial = GameplayManager.Instance.AimLine.sharedMaterial;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        BoardBall primaryBall = entities[0].boardBall;
        lineMaterial.SetColor("_Color", GameplayManager.Instance.ColorsDic.colors[primaryBall.value]);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasBoardBall;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.PrimaryBall));
    }
}
