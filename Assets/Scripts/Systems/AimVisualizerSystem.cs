using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class AimVisualizerSystem : ReactiveSystem<InputEntity>
{
    InputContext inputContext;
    GameContext gameContext;
    List<Vector3> linePositions, ballTargets;

    LineRenderer aimLine;
    GameEntity ballEntity;
    IGroup<GameEntity> ballEntitieGroup;

    public AimVisualizerSystem(Contexts contexts) : base(contexts.input)
    {
        inputContext = contexts.input;
        gameContext = contexts.game;

        linePositions = new List<Vector3>();
        ballTargets = new List<Vector3>();
        aimLine = GameplayManager.Instance.AimLine;
        ballEntitieGroup = contexts.game.GetGroup(GameMatcher.PrimaryBall);
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.AllOf(InputMatcher.MousePressed, InputMatcher.MouseLeft));
    }

    protected override bool Filter(InputEntity entity)
    {
        return true;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        if (gameContext.GetGroup(GameMatcher.Moving).GetEntities().Length > 0)
            return;

        Vector3 start, end, dir;
        RaycastHit2D hit;

        linePositions.Clear();
        ballTargets.Clear();
        InputEntity click = entities[0];

        start = GameplayManager.Instance.PrimaryBallPosition.position;
        end = click.mousePressed.position;
        dir = (end - start).normalized;

        linePositions.Add(start);

        hit = Physics2D.Raycast(start, dir);
        if (hit.collider.CompareTag("Border"))
        {
            start += dir * hit.distance*0.98f;
            linePositions.Add(start);
            ballTargets.Add(start);
            dir.x *= -1;
        }

        ballEntity = ballEntitieGroup.GetEntities()[0];
        hit = Physics2D.Raycast(start, dir);
        if (hit.collider.CompareTag("Ball"))
        {
            end = hit.point;
            linePositions.Add(end);

            GameEntity collidedBallEntity = hit.collider.gameObject.GetEntityLink().entity as GameEntity;

            BoardBall tmp = GetEstimatedBoardBall(collidedBallEntity, end);
            ballEntity.boardBall.boardIdx = tmp.boardIdx;
            ballEntity.boardBall.shifted = tmp.shifted;
            
            end = GameUtils.WorldPosForBall(ballEntity);
            linePositions.Add(end);
            ballTargets.Add(end);
        }

        ballEntity.ReplaceTargetPositions(ballTargets);
        UpdateAimLine();
    }

    private BoardBall GetEstimatedBoardBall(GameEntity ballEntity, Vector3 hitPos)
    {
        BoardBall res = new BoardBall();
        res.boardIdx = ballEntity.boardBall.boardIdx;

        bool upHit = hitPos.y > ballEntity.gameObject.gameobject.transform.position.y;
        bool rightHit = hitPos.x > ballEntity.gameObject.gameobject.transform.position.x;

        if(upHit)
        {
            res.boardIdx.x += rightHit ? 1 : -1;
            res.shifted = ballEntity.boardBall.shifted;
        }
        else
        {
            res.shifted = !ballEntity.boardBall.shifted;
            res.boardIdx.y++;
            if(ballEntity.boardBall.shifted)
            {
                res.boardIdx.x += rightHit ? 1 : 0;
            }
            else
            {
                res.boardIdx.x += rightHit ? 0 : -1;
            }
        }

        if (res.boardIdx.x == -1 || res.boardIdx.y == -1 || res.boardIdx.x > 5)
            res.boardIdx = Vector2.one * -1;

        return res;
    }

    void UpdateAimLine()
    {
        aimLine.positionCount = linePositions.Count;
        aimLine.SetPositions(linePositions.ToArray());
    }

}
