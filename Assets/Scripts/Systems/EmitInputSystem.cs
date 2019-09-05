using Entitas;
using UnityEngine;

public class EmitInputSystem : IInitializeSystem, IExecuteSystem
{
    InputContext inputContext;
    InputEntity leftMouseEntity;
    bool valid = false;

    public EmitInputSystem(Contexts contexts)
    {
        inputContext = contexts.input;
    }

    public void Initialize()
    {
        inputContext.isMouseLeft = true;
        leftMouseEntity = inputContext.mouseLeftEntity;

        valid = false;
    }
    public void Execute()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 start = mousePosition;
        start.z = -5;
        Vector3 end = start;
        end.z = 5;
        RaycastHit hit;
        Physics.Raycast(start, end, out hit);

        if (Input.GetMouseButtonUp(0))
        {
            if (valid)
            {
                leftMouseEntity.ReplaceMouseUp(mousePosition);
            }
            valid = false;
        }
        if (Input.GetMouseButton(0))
        {
            if (valid)
            {
                leftMouseEntity.ReplaceMousePressed(mousePosition);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(IsInputArea(mousePosition))
            {
                valid = true;
                leftMouseEntity.ReplaceMouseDown(mousePosition);
            }
        }
    }

    bool IsInputArea(Vector2 mousePosition)
    {
        Vector3 start = mousePosition;
        start.z = -5;
        Vector3 end = start;
        end.z = 5;
        RaycastHit hit;
        Physics.Raycast(start, end, out hit);
        return hit.collider && hit.collider.name == "InputArea";
    }
}
