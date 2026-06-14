using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Portal : Enemy
{

    [Header("Portal Parametors")]
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject Projectile;
    [SerializeField] float projectileSpeed;
    //[SerializeField] float attackSpeed;
    [SerializeField, Range(1, 10f)] float attackRange = 2f;


    [Header("Set Portal Layer")]
    [SerializeField] string portalLayerName = "Portal";
    [Header("Set Portal Visual Parameters")]
    [SerializeField] Gradient emissionColor;
    [SerializeField] ParticleSystem orbParticlesL, orbParticlesR, fireParticles;
    [SerializeField] Light gateLight;
    [SerializeField] Renderer gateRenderer, gateEffectRenderer;
    [SerializeField] SoundData gateData;
    [SerializeField] SoundData topFireData;
    [SerializeField] SoundData screamData;

    [HideInInspector] public Tile SpawnTile;
    [HideInInspector] public int id;
    [HideInInspector] public List<GameObject> path;
    [HideInInspector] public int ID => id;
    [HideInInspector] public bool IsDestroyed = false;

    Material gateMaterial, gateEffectMaterial;
    GameObject gateEffectObj;
    float gateLightMaxIntencity = 5f
    //, gateAudioMaxVolume = 0.3f
    , fireAudioMaxVolume = 0.6f;

    private void Start()
    {
        SetParameters();
        attackCooldown = 1 / attackSpeed;
        gameObject.layer = LayerMask.NameToLayer(portalLayerName);

        gateEffectObj = gateEffectRenderer.gameObject;
        gateEffectMaterial = gateEffectRenderer.material;

        gateMaterial = gateRenderer.material;

        gateEffectObj.SetActive(false);
        gateLight.gameObject.SetActive(false);

        gateEffectMaterial.SetFloat("_Alpha", 0);
        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(0));
        gateLight.intensity = 0;

    }
    private void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;
        if (state == EnemyState.Dead) return;

        if (!IsDestroyed && health <= 0) OnDestroyed();
        Attack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 posiion = transform.position;
        Gizmos.DrawWireSphere(posiion, attackRange);
        Gizmos.color = Color.red;
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
    void Shoot()
    {
        AudioManager.Instance.Play(Type, GamePlaySFX_Type.EnemyAttack, SoundData, transform);
        Vector3 point = target.transform.position + new Vector3(0, 1.5f, 0);
        float travelDistance = Vector3.Distance(shootingPoint.position, point);
        float travelTime = travelDistance / projectileSpeed;
        GameObject newProjectile = Instantiate(Projectile, shootingPoint.position, Quaternion.LookRotation(point - shootingPoint.position));
        newProjectile.GetComponent<Mage>()?.Initialize(projectileSpeed);
        StartCoroutine(HitTarget(newProjectile, travelTime));
    }
    protected override bool AcquireTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, towerMask);
        if (targets.Length > 0)
        {
            int ClosestTargetIndex = 0;
            float MinDist = Vector3.Distance(transform.position, targets[ClosestTargetIndex].transform.position);

            for (int i = 1; i < targets.Length; i++)
            {
                if (MinDist <= MinDist + i)
                {
                    float dist = Vector3.Distance(transform.position, targets[i].transform.position);
                    if (dist < MinDist)
                    {
                        MinDist = dist;
                        ClosestTargetIndex = i;
                    }
                }
            }
            target = targets[ClosestTargetIndex].GetComponent<Tower>();

            if (target != null) return true;
            else return false;
        }
        target = null;
        return false;
    }

    IEnumerator HitTarget(GameObject currentProjectile, float arriveTime)
    {
        yield return new WaitForSeconds(arriveTime);

        AudioManager.Instance.Play(Type, GamePlaySFX_Type.ProjectileHit, SoundData, currentProjectile.transform);
        if (target != null)
        {
            target.ApplyDamage(damage);
        }
        Destroy(currentProjectile);
    }
    #region Visual
    public void OnDestroyed()
    {
        if (IsDestroyed) return;
        IsDestroyed = true;
        state = EnemyState.Dead;

        StopAllCoroutines();

        PortalManager.Instance.OnPortalDestroy(this);

        fireParticles.Stop();
        orbParticlesL.Stop();
        orbParticlesR.Stop();

        gateEffectObj.SetActive(false);
        gateLight.gameObject.SetActive(false);

        gameObject.SetActive(false);

    }
    public IEnumerator ActivateVisual()
    {

        float transitionTimer = 0;
        float rand1 = Random.Range(0.2f, 0.8f);
        float rand2 = Random.Range(0.2f, 0.8f);

        while (transitionTimer < 1f)
        {
            transitionTimer += Time.deltaTime * 0.5f;

            if (transitionTimer >= rand1 && !orbParticlesL.isPlaying)
            {
                orbParticlesL.Play();
                //AudioManager.Instance.Play(GamePlaySFX_Type.PortalOrb, SoundData, orbParticlesL.transform);
            }

            if (transitionTimer >= rand2 && !orbParticlesR.isPlaying)
            {
                orbParticlesR.Play();
                //AudioManager.Instance.Play(GamePlaySFX_Type.PortalOrb, SoundData, orbParticlesR.transform);
            }

            gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(transitionTimer));

            yield return null;
        }

        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(1f));

        yield return StartCoroutine(ActivateGate());
    }

    public IEnumerator ActivateGate()
    {
        float transitionTimer = 0;

        gateEffectObj.SetActive(true);
        gateLight.gameObject.SetActive(true);

        fireParticles.Play();
        topFireData.volume = fireAudioMaxVolume;

        //AudioManager.Instance.Play(GamePlaySFX_Type.PortalTopFire, topFireData, orbParticlesR.transform);
        //AudioManager.Instance.Play(GamePlaySFX_Type.PortalScreaming, screamData, orbParticlesR.transform);
        //AudioManager.Instance.Play(GamePlaySFX_Type.PortalGate, gateData, orbParticlesR.transform);

        while (transitionTimer < 1f)
        {
            transitionTimer += Time.deltaTime * 0.21f;

            gateEffectMaterial.SetFloat("_Alpha", 1f - transitionTimer * 0.75f);

            gateLight.intensity = transitionTimer * gateLightMaxIntencity;
            //gateData.volume = transitionTimer * gateAudioMaxVolume;

            yield return null;
        }

        gateEffectMaterial.SetFloat("_Alpha", 0f);
        gateLight.intensity = gateLightMaxIntencity;
        //gateData.volume = gateAudioMaxVolume;
    }

    public IEnumerator DeactivateVisual()
    {
        if (WaveManager.Instance.IsLastWave())
            yield break;

        if (IsDestroyed)
            yield break;

        float transitionTimer = 1;

        orbParticlesL.Stop();
        orbParticlesR.Stop();


        while (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime * 0.3f;

            gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(transitionTimer));

            gateEffectMaterial.SetFloat("_Alpha", 1f - transitionTimer);
            gateLight.intensity = transitionTimer * gateLightMaxIntencity;
            //gateData.volume = transitionTimer * gateAudioMaxVolume;
            topFireData.volume = transitionTimer * fireAudioMaxVolume;
            yield return null;
        }

        fireParticles.Stop();
        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(0f));
        gateEffectObj.SetActive(false);
        gateLight.gameObject.SetActive(false);
        //gateData.clip = null;
        topFireData.clip = null;
        //AudioManager.Instance.Stop();
        yield break;
    }
    #endregion
    protected override void OnDeath() { }
    protected override void Attack()
    {
        if (attackCooldown <= 0)
        {
            if (AcquireTargets())
            {
                Shoot();
            }
            attackCooldown = 1 / attackSpeed;
        }
        attackCooldown -= Time.deltaTime;
    }

}
