using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ControlsView : MonoBehaviour
{
    [SerializeField]
    private Slider
        movement,
        rotation,
        zoom,
        mouseDrag;
    [SerializeField]
    private Button
        B_rebindForward,
        B_rebindBackward,
        B_rebindLeft,
        B_rebindRight,
        B_defaultForward,
        B_defaultBackward,
        B_defaultLeft,
        B_defaultRight;
    [SerializeField]
    private TextMeshProUGUI
        moveSpeedText,
        rotationSpeedText,
        zoomSpeedText,
        dragSpeedText,

        forwardLabel,
        backwardLabel,
        leftLabel,
        rightLabel,

        forwardKeyLabel,
        backwardKeyLabel,
        leftKeyLabel,
        rightKeyLabel;
    [Header("Listening Panel")]
    [SerializeField]
    private TextMeshProUGUI
        listeningText;
    [SerializeField] private GameObject P_listeningOverlay;

    private ControlsViewModel viewModel;

    public const int DEFAULT_BINDING_INDEX = 0;

    private void OnDestroy()
    {
        if (viewModel == null) return;
        viewModel.OnChanged -= Refresh;
    }
    //const int UP = 1;
    //const int DOWN = 2;
    //const int LEFT = 3;
    //const int RIGHT = 4;

    public void InitializeData(SettingsInstaller installer)
    {
        viewModel = installer.ControlsVM;

        SetTexts();
        SetListeners();
        P_listeningOverlay?.SetActive(false);

        viewModel.OnChanged += Refresh;

        Refresh();
    }
    void RemoveAllListeners()
    {
        B_rebindForward.onClick.RemoveAllListeners();
        B_rebindBackward.onClick.RemoveAllListeners();
        B_rebindLeft.onClick.RemoveAllListeners();
        B_rebindRight.onClick.RemoveAllListeners();

        B_defaultForward.onClick.RemoveAllListeners();
        B_defaultBackward.onClick.RemoveAllListeners();
        B_defaultLeft.onClick.RemoveAllListeners();
        B_defaultRight.onClick.RemoveAllListeners();

        movement.onValueChanged.RemoveAllListeners();
        rotation.onValueChanged.RemoveAllListeners();
        zoom.onValueChanged.RemoveAllListeners();
        mouseDrag.onValueChanged.RemoveAllListeners();
    }
    void SetListeners()
    {
        RemoveAllListeners();

        B_rebindForward.onClick.AddListener(() => TryStartRebind(InputActionId.MoveForward, forwardLabel));
        B_rebindBackward.onClick.AddListener(() => TryStartRebind(InputActionId.MoveBackward, backwardLabel));
        B_rebindLeft.onClick.AddListener(() => TryStartRebind(InputActionId.MoveLeft, leftLabel));
        B_rebindRight.onClick.AddListener(() => TryStartRebind(InputActionId.MoveRight, rightLabel));

        B_defaultForward.onClick.AddListener(() => viewModel.ResetBindingToDefault(InputActionId.MoveForward, DEFAULT_BINDING_INDEX));
        B_defaultBackward.onClick.AddListener(() => viewModel.ResetBindingToDefault(InputActionId.MoveBackward, DEFAULT_BINDING_INDEX));
        B_defaultLeft.onClick.AddListener(() => viewModel.ResetBindingToDefault(InputActionId.MoveLeft, DEFAULT_BINDING_INDEX));
        B_defaultRight.onClick.AddListener(() => viewModel.ResetBindingToDefault(InputActionId.MoveRight, DEFAULT_BINDING_INDEX));

        movement.onValueChanged.AddListener(v =>
        {
            viewModel.SetMoveSpeed(v);
            UpdateLabel(moveSpeedText, v);
        });

        rotation.onValueChanged.AddListener(v =>
        {
            viewModel.SetRotationSpeed(v);
            UpdateLabel(rotationSpeedText, v);
        });

        zoom.onValueChanged.AddListener(v =>
        {
            viewModel.SetZoomSpeed(v);
            UpdateLabel(zoomSpeedText, v);
        });

        mouseDrag.onValueChanged.AddListener(v =>
        {
            viewModel.SetDragSpeed(v);
            UpdateLabel(dragSpeedText, v);
        });

    }
    void SetTexts()
    {
        forwardLabel.text = "Forward";
        backwardLabel.text = "Backward";
        leftLabel.text = "Left";
        rightLabel.text = "Right";

        forwardKeyLabel.text = "W";
        backwardKeyLabel.text = "S";
        leftKeyLabel.text = "A";
        rightKeyLabel.text = "D";

        moveSpeedText.text = "Movement";
        rotationSpeedText.text = "Rotation";
        zoomSpeedText.text = "Zoom";
        dragSpeedText.text = "Drag Speed";

    }
    private void TryStartRebind(InputActionId id, TextMeshProUGUI targetLabel)
    {
        if (viewModel.IsRebinding) return;

        ShowListening(targetLabel);

        viewModel.StartRebind(id, DEFAULT_BINDING_INDEX);
    }
    void ShowListening(TextMeshProUGUI target)
    {
        P_listeningOverlay.SetActive(true);
        listeningText.text = $"Waiting for press ... {target.name} (ESC to cancel)";
    }

    void HideListening()
    {
        P_listeningOverlay.SetActive(false);
    }


    void SetButtonsInteractable(bool value)
    {
        B_rebindForward.interactable = value;
        B_rebindBackward.interactable = value;
        B_rebindLeft.interactable = value;
        B_rebindRight.interactable = value;
    }

    void UpdateLabel(TextMeshProUGUI label, float value)
        => label.text = Mathf.RoundToInt(value * 100).ToString();

    void Refresh()
    {
        // Bindings
        forwardKeyLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveForward, DEFAULT_BINDING_INDEX);
        backwardKeyLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveBackward, DEFAULT_BINDING_INDEX);
        leftKeyLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveLeft, DEFAULT_BINDING_INDEX);
        rightKeyLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveRight, DEFAULT_BINDING_INDEX);

        // Speeds (Pending)
        movement.SetValueWithoutNotify(viewModel.PendingData.MovementSpeed);
        rotation.SetValueWithoutNotify(viewModel.PendingData.RotationSpeed);
        zoom.SetValueWithoutNotify(viewModel.PendingData.ZoomSpeed);
        mouseDrag.SetValueWithoutNotify(viewModel.PendingData.DragSpeed);

        UpdateLabel(moveSpeedText, viewModel.PendingData.MovementSpeed);
        UpdateLabel(rotationSpeedText, viewModel.PendingData.RotationSpeed);
        UpdateLabel(zoomSpeedText, viewModel.PendingData.ZoomSpeed);
        UpdateLabel(dragSpeedText, viewModel.PendingData.DragSpeed);

        // Rebind state
        if (!viewModel.IsRebinding)
            HideListening();

        // Optional: disable buttons during rebind
        SetButtonsInteractable(!viewModel.IsRebinding);
    }
}
