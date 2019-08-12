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
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            if(!e.hasGameObject)
            {
                GameObject newObj = GameplayManager.Instance.CreateBall();
                e.AddGameObject(newObj);
                e.AddPosition(WorldPosForBall(e));
                newObj.Link(e);
            }
            else
            {
                e.ReplacePosition(WorldPosForBall(e));
            }
            e.gameObject.gameobject.GetComponentInChildren<TextMeshPro>().SetText(e.boardBall.value.ToString());
            e.gameObject.gameobject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    Vector3 WorldPosForBall(GameEntity e)
    {
        Vector3 res = WorldPosForIdxPos(e.boardBall.boardIdx);
        res.x += 0.5f * (e.boardBall.shifted ? GameplayManager.Instance.cellWidth : 0.0f);
        return res;
    }

    Vector2 WorldPosForIdxPos(Vector2 idxPos)
    {
        idxPos.x *= GameplayManager.Instance.cellWidth;
        idxPos.y *= GameplayManager.Instance.cellLength * -1;
        return GameplayManager.Instance.ZeroPosition + idxPos;
    }
}
