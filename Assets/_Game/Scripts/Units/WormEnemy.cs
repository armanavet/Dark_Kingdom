using System;
using System.Collections;
using UnityEngine;
public class WormEnemy : Enemy
{
    [SerializeField] GameObject spawnHole;
    [SerializeField] ParticleSystem effect;

    private GameObject currentHole;
    private ParticleSystem currentEffect;
    private Vector3 startPos;
    private static readonly int IsTaunting = Animator.StringToHash("isTaunting");
    private static readonly int IsDivingIn = Animator.StringToHash("isDivingIn");

    void Awake()
    {
        damage = maxDamage;
        animator = GetComponent<Animator>();
        if (effect == null)
        {
            Debug.LogError($"[WormEnemy] Effect is NULL on {gameObject.name}");
            return;
        }
    }
    void Start()
    {
        SetParameters();
        SpawnHole();
    }
    void SpawnHole()
    {
        currentHole = Instantiate(spawnHole, transform.position, Quaternion.identity, transform);
    }
    public void OnBrackThroughtComplete()
    {
        animator.SetBool(IsTaunting, true);
    }
    public void PlayEffect()
    {
        if (currentEffect != null) currentEffect = null;
        currentEffect = Instantiate(effect, transform.position, Quaternion.identity, transform);
    }
    public void OnTauntFinished()
    {
        animator.SetBool(IsTaunting, false);
        animator.SetBool(IsDivingIn, true);
    }

    public void OnDiveComplete()
    {
        StartCoroutine(MoveYAxis(currentHole.transform, -1f, 2.5f, OnComplete));
    }

    IEnumerator MoveYAxis(Transform target, float targetY, float duration, Action onComplete = null)
    {
        float startY = target.position.y;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float newY = Mathf.Lerp(startY, targetY, timeElapsed / duration);

            target.position = new Vector3(
                target.position.x,
                newY,
                target.position.z
            );

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        target.position = new Vector3(
            target.position.x,
            targetY,
            target.position.z
        );
        if (onComplete != null)
            onComplete?.Invoke();
    }
    void OnComplete()
    {
        Destroy(gameObject);
    }
    public void PlayBreakingThroughSFX()
    {
        Debug.Log(Type);
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.WormBreakingThrough, SoundData, transform);
    }
    public void PlayDiveInSFX()
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.WormDiveIn, 0, SoundData, transform);
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.WormDiveIn, 1, SoundData, transform);
    }
    public void PlayRoarSFX()
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.WormRoar, SoundData, transform);
    }
    protected override void Attack() { }
    //public override int GetTargetPriority() => 0;
}
