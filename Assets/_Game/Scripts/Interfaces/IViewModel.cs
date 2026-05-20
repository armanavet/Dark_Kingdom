using System;
using UnityEngine;

public interface IViewModel
{
    bool HasChanges { get; }
    void Apply();
    void RevertToSaved();
    void ResetToDefault();
    event Action OnChanged;
}
