using System;
using UnityEngine.InputSystem;

public class RebindOperation : IDisposable
{
    public enum Result { Completed, Cancelled }

    public event Action<Result> OnFinished;

    private readonly InputAction _action;
    private InputActionRebindingExtensions.RebindingOperation _op;

    public RebindOperation(InputAction action, int bindingIndex)
    {
        _action = action;
        _action.Disable();

        _op = _action.
            PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTimeout(10f)
            .OnComplete(_ => Finish(Result.Completed))
            .OnCancel(_ => Finish(Result.Cancelled));

        _op.Start();
    }

    public void Cancel () => _op?.Cancel();

    private void Finish(Result result)
    {
        _action.Enable();
        OnFinished?.Invoke(result);
        Dispose();
    }
    public void Dispose()
    {
        _op?.Dispose();
        _op = null;
    }
}
