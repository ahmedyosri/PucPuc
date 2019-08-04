using Entitas;
using UnityEngine;

public class EmitInputSystem : IInitializeSystem, IExecuteSystem
{
    InputContext inputContext;
    InputEntity leftMouseEntity;

    public EmitInputSystem(Contexts contexts)
    {
        inputContext = contexts.input;
    }

    public void Initialize()
    {
        inputContext.isMouseLeft = true;
        leftMouseEntity = inputContext.mouseLeftEntity;
    }
    public void Execute()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            leftMouseEntity.ReplaceMouseUp(mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            leftMouseEntity.ReplaceMousePressed(mousePosition);
        }
        if (Input.GetMouseButtonDown(0))
        {
            leftMouseEntity.ReplaceMouseDown(mousePosition);
        }
    }
}
