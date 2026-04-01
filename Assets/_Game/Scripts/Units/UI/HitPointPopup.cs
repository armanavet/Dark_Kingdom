using UnityEngine;
using TMPro;
using DG.Tweening;

public class HitPointPopup : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;
    //[SerializeField] float upDistance = 0.5f;
    //[SerializeField] float sideDistance = 0.6f;
    //[SerializeField] float duration = 0.25f;

    private Vector3 startPos;

    #region Singleton 
    private static HitPointPopup _instance;
    public static HitPointPopup Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<HitPointPopup>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    private const float duration = 0.45f;

    public void HitPointText(float damage)
    {
        textMesh.text = damage.ToString();

        Vector3 startPos = transform.position;

        float upDistance = 0.35f;
        float sideDistance = 0.12f;

        Vector3 side = Random.value > 0.5f ? Vector3.right : Vector3.left;
        Vector3 endPos = startPos + (Vector3.up * upDistance) + (side * sideDistance);
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(endPos, duration)
        .SetEase(Ease.OutCubic))
        .Insert(duration * 0.3f, textMesh.DOFade(0f, duration * 0.7f))
        .OnComplete(() => Destroy(gameObject));
    }
}
