using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class QualityView: MonoBehaviour
{
    [SerializeField] private TMP_Dropdown qualityLevels;
    private List<string> options = new List<string>()
    {"Low","Medium","High","Ultra"};
    private QualityViewModel viewModel;

    public void InitializeData(SettingsInstaller installer)
    {
        viewModel = installer.QualityVM;
        
        qualityLevels.ClearOptions();

        qualityLevels.AddOptions(options);
        qualityLevels.RefreshShownValue();

        qualityLevels.onValueChanged.AddListener(viewModel.SetQualityLevel);

        viewModel.OnChanged += Refresh;
        Refresh();
    }
    void Refresh()
    {
        qualityLevels.SetValueWithoutNotify(viewModel.PendingData.QualityIndex);

        qualityLevels.RefreshShownValue();
    }
}
