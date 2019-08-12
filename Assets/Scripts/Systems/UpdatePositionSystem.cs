using System.Collections.Generic;
using Entitas;

public class UpdatePositionSystem : ReactiveSystem<GameEntity>
{
    GameContext context;
    public UpdatePositionSystem(Contexts contexts) : base(contexts.game)
    {
        context = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Position);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGameObject && entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            e.gameObject.gameobject.transform.position = e.position.pos;
        }
    }
}
