using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [Header("Audio Parameters")]
    [SerializeField] protected SoundData soundData;
    [SerializeField] GameObject[] effects;
    [SerializeField] LayerMask EnemyMask;
    [SerializeField] LayerMask PortalMask;
    List<Debuff> debuffs;
    Vector3 launchPoint, targetPoint, launchVelocity;
    float age, blastRadius, damage;
    TowerType towerType;
    private void Start()
    {
        //soundData = AudioManager.Instance.SetData(SoundDataType.Gameplay);
    }
    void Update()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.position = p;
        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        if (transform.position.y < 0f)
        {
            Explode();
        }

    }
    public void Initialize(
        Vector3 launchPoint,
        Vector3 targetPoint,
        Vector3 launchVelocity,
        float blastRadius,
        float damage,
        List<Debuff> debuffs,
        TowerType towerType)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
        this.debuffs = debuffs;
        this.towerType = towerType;
    }
    void Explode()
    {
        Collider[] Targets = Physics.OverlapSphere(transform.position, blastRadius, PortalMask);
        if (Targets.Length == 0) Targets = Physics.OverlapSphere(transform.position, blastRadius, EnemyMask);

        if (Targets.Length > 0)
        {
            foreach (var Target in Targets)
            {
                ITargetable target = Target.GetComponent<ITargetable>();
                if (target is Enemy enemyTarget)
                {
                    UIManager.Instance.ShowDamage(enemyTarget, damage);
                    foreach (var debuff in debuffs)
                    {
                        DebuffManager.Instance.ApplyDebuff(enemyTarget, debuff);
                    }
                }
                target.ApplyDamage(damage);
            }
        }
        DestroyObject(effects);
    }
    IEnumerator DestroyObject(GameObject[] effects)
    {
        AudioManager.Instance.Play(towerType, GamePlaySFX_Type.TowerShoot, soundData, transform);
        GameObject effect;
        if (effects.Length != 0)
        {
            effect = Instantiate(effects[0], new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.Euler(-90f, transform.rotation.y, transform.rotation.z));
            Destroy(effect, 2f);
        }
        gameObject.SetActive(false);
        yield return new WaitForSeconds(soundData.clip.length);
        Destroy(gameObject);
        yield break;
    }
}
