public class QualitySettingsModel
{
    public int QualityIndex;

    public static QualitySettingsModel CreateDefault(int qualitySettings) => new QualitySettingsModel
    {
        QualityIndex = qualitySettings,
    };

    public QualitySettingsModel Clone() => new QualitySettingsModel { QualityIndex = QualityIndex };

}
