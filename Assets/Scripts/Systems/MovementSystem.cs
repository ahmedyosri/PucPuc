using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class MovementSystem : IExecuteSystem
{
    GameContext gameContext;
    IGroup<GameEntity> movingEntities;

    float speed = 15.0f;
    float borderLimit = 2.5f;

    public MovementSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        movingEntities = gameContext.GetGroup(GameMatcher.Moving);
    }

    public void Execute()
    {
        foreach(var e in movingEntities.GetEntities())
        {
            e.ReplacePosition(e.position.pos + e.velocity.direction * Time.deltaTime * speed);

            if(e.position.pos.x > borderLimit)
            {
                e.velocity.direction.x *= -1;
                e.position.pos.x = borderLimit;
            }
            else if(e.position.pos.x < -borderLimit)
            {
                e.velocity.direction.x *= -1;
                e.position.pos.x = -borderLimit;
            }
        }
    }

}
