using UnityEngine;

public class GameUtils
{
    public static Vector3 WorldPosForBall(GameEntity e)
    {
        Vector3 res = WorldPosForIdxPos(e.boardBall.boardIdx);
        res.x += 0.5f * (e.boardBall.shifted ? GameplayManager.Instance.cellWidth : 0.0f);
        return res;
    }

    public static Vector2 WorldPosForIdxPos(Vector2 idxPos)
    {
        idxPos.x *= GameplayManager.Instance.cellWidth;
        idxPos.y *= GameplayManager.Instance.cellLength * -1;
        return GameplayManager.Instance.ZeroPosition + idxPos;
    }

    public static Color GetColor(int val)
    {
        return GameplayManager.Instance.ColorsDic.colors[val];
    }
}
