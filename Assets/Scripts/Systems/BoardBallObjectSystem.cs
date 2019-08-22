using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using TMPro;
using UnityEngine;

public class BoardBallObjectSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    public BoardBallObjectSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BoardBall);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGameObject;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            if (e.boardBall.boardIdx.x > -1)
            {
                Vector3 newPos = GameUtils.WorldPosForBall(e);
                if (newPos != (Vector3) e.position.pos)
                {
                    e.ReplaceTargetPositions(50, new List<Vector3>() { newPos });
                    e.isMoving = true;
                }
            }

            string valText = Mathf.Pow(2, e.boardBall.value).ToString();
            e.gameObject.gameobject.GetComponentInChildren<TextMeshPro>().SetText(valText);
            e.gameObject.gameobject.GetComponent<SpriteRenderer>().color = GameUtils.GetColor(e.boardBall.value);
        }
    }
}
