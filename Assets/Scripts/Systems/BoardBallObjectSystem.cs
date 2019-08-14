﻿using System.Collections.Generic;
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
            if (!e.hasPosition)
                e.AddPosition(Vector2.one * -1);

            if (e.boardBall.boardIdx.x > -1)
                e.ReplacePosition(GameUtils.WorldPosForBall(e));

            e.gameObject.gameobject.GetComponentInChildren<TextMeshPro>().SetText(e.boardBall.value.ToString());
            e.gameObject.gameobject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }
}
