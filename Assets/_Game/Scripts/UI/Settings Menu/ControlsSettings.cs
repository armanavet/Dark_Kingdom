using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ControlsSettings : MonoBehaviour
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
        B_default;
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
        listeningText;
    [SerializeField] private GameObject listeningOverlay;

    private SettingsViewModel viewModel;

    public const int DEFAULT_BINDING_INDEX = 0;

    //const int UP = 1;
    //const int DOWN = 2;
    //const int LEFT = 3;
    //const int RIGHT = 4;

    public void InitializeData(SettingsInstaller installer)
    {
        viewModel = installer.ViewModel;

        B_rebindForward.onClick.AddListener(() => TryStartRebind(InputActionId.MoveForward, forwardLabel));
        B_rebindBackward.onClick.AddListener(() => TryStartRebind(InputActionId.MoveBackward, backwardLabel));
        B_rebindLeft.onClick.AddListener(() => TryStartRebind(InputActionId.MoveLeft, leftLabel));
        B_rebindRight.onClick.AddListener(() => TryStartRebind(InputActionId.MoveRight, rightLabel));
        B_default.onClick.AddListener(viewModel.CancelRebind);

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
        listeningOverlay?.SetActive(false);

        viewModel.OnChanged += Refresh;

        Refresh();
    }
    private void TryStartRebind(InputActionId id, TextMeshProUGUI targetLabel)
    {
        if (viewModel.IsRebinding) return;

        ShowListening(targetLabel);

        viewModel.StartRebind(id, DEFAULT_BINDING_INDEX);
    }
    void ShowListening(TextMeshProUGUI target)
    {
        listeningOverlay.SetActive(true);
        listeningText.text = $"Waiting for press ... {target.name} (ESC to cancel)";
    }

    void HideListening()
    {
        listeningOverlay.SetActive(false);
    }

    // --- UI refresh ---
    void Refresh()
    {
        // Bindings
        forwardLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveForward, DEFAULT_BINDING_INDEX);
        backwardLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveBackward, DEFAULT_BINDING_INDEX);
        leftLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveLeft, DEFAULT_BINDING_INDEX);
        rightLabel.text = viewModel.GetBindingDisplay(InputActionId.MoveRight, DEFAULT_BINDING_INDEX);

        // Speeds (Pending)
        movement.SetValueWithoutNotify(viewModel.PendingControlsData.MovementSpeed);
        rotation.SetValueWithoutNotify(viewModel.PendingControlsData.RotationSpeed);
        zoom.SetValueWithoutNotify(viewModel.PendingControlsData.ZoomSpeed);
        mouseDrag.SetValueWithoutNotify(viewModel.PendingControlsData.DragSpeed);

        UpdateLabel(moveSpeedText, viewModel.PendingControlsData.MovementSpeed);
        UpdateLabel(rotationSpeedText, viewModel.PendingControlsData.RotationSpeed);
        UpdateLabel(zoomSpeedText, viewModel.PendingControlsData.ZoomSpeed);
        UpdateLabel(dragSpeedText, viewModel.PendingControlsData.DragSpeed);

        // Rebind state
        if (!viewModel.IsRebinding)
            HideListening();

        // Optional: disable buttons during rebind
        SetButtonsInteractable(!viewModel.IsRebinding);
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
}
