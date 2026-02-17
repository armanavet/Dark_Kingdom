using UnityEngine;
using TMPro;
using DG.Tweening;

public class HitPointPopup : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;
    [SerializeField] float upDistance;
    [SerializeField] float sideDistance;
    [SerializeField] float duration;

    private Vector3 startPos;

    #region Singleton 
    private static HitPointPopup _instance;
    public static HitPointPopup Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HitPointPopup>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    public void HitPointText(float damage)
    {
        textMesh.text = damage.ToString();

        startPos = transform.position;

        Sequence seq = DOTween.Sequence();

        Vector3 upPos = startPos + Vector3.up * upDistance;
        Vector3 side = Random.value > 0.5f ? Vector3.right : Vector3.left;
        Vector3 endPos = upPos + side * sideDistance + Vector3.down * 0.5f;

        seq.Append(transform.DOMove(upPos, 0.4f).SetEase(Ease.OutQuad))
           .Append(transform.DOMove(endPos, 0.8f).SetEase(Ease.InQuad))
           .Join(textMesh.DOFade(0f, 0.8f))
           .OnComplete(() => Destroy(gameObject));
    }
}
