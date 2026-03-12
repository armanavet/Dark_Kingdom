using AudioSystem;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [Header("Audio Parameters")]
    [SerializeField] protected SoundData soundData;
    [SerializeField] GameObject[] effects;
    [SerializeField] LayerMask EnemyMask;
    [SerializeField] LayerMask PortalMask;
    GameObject effect;
    List<Debuff> debuffs;
    Vector3 launchPoint, targetPoint, launchVelocity;
    float age, blastRadius, damage;
    HitPointPopup hitPointPopup;
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
    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float blastRadius, float damage, List<Debuff> debuffs, HitPointPopup hitPointPopup, TowerType towerType)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
        this.debuffs = debuffs;
        this.hitPointPopup = hitPointPopup;
        this.towerType = towerType;
    }
    void Explode()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, blastRadius, PortalMask);
        if (targets.Length == 0) targets = Physics.OverlapSphere(transform.position, blastRadius, EnemyMask);

        if (targets.Length > 0)
        {
            foreach (var target in targets)
            {
                Enemy enemy = target.GetComponent<Enemy>();
                UIManager.Instance.ShowDamage(hitPointPopup, enemy, damage);
                enemy.ApplyDamage(damage);
                foreach (var debuff in debuffs)
                {
                    DebuffManager.Instance.ApplyDebuff(enemy, debuff);
                }
            }
        }
        AudioManager.Instance.Play(towerType, GamePlaySFX_Type.EnemyAttack, soundData, transform);
        if (effects.Length != 0)
        {
            effect = Instantiate(effects[0], new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.Euler(-90f, transform.rotation.y, transform.rotation.z));
            Destroy(effect, 2f);
        }
        Destroy(gameObject);

    }

}
