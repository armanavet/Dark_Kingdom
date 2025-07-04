using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] Mage mage;
    [SerializeField] float height;
    [SerializeField] float rotationSpeed;
    [SerializeField] float flyUpSpeed;
    [SerializeField] float distanceToAttack;
    Vector3 targetPoint;
    Quaternion targetRotation;
    float rotationProgress;
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        target = TowerManager.Instance.Towers[0];
        targetPoint = target.transform.position + new Vector3(0, height, 0);
        targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
        currentSpeed = maxSpeed;
        help = maxHP;
        damage = maxDamage;
        attackSpeed = maxAttackSpeed;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (state == EnemyState.Dead) 
        {
            Fall(); 
            return; 
        } 
        if (FlyUp()) return;
        else if (FaceTarget()) return;
        else if (Vector3.Distance(transform.position, targetPoint) > distanceToAttack) Move();
        else Attack();
        
    }
    protected override void Move()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isAttacking", false);
        transform.Translate(Vector3.forward * currentSpeed*Time.deltaTime);
    }
    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
        attackCooldown -= Time.deltaTime;
        if (target != null && attackCooldown <= 0)
        {
            Mage arrow = Instantiate(mage, transform.position, Quaternion.LookRotation(targetPoint - transform.position));
            arrow.Initialize(attackSpeed, transform.position, targetPoint, target, damage);
            attackCooldown = 1 / attackSpeed;
        }
    }
    bool FaceTarget()
    {
        float rotationDifference = targetRotation.eulerAngles.y - transform.rotation.y;
        float rotationTime=rotationDifference/rotationSpeed;
        rotationProgress += Time.deltaTime / rotationTime;
        if(rotationProgress>=1)return false;
        float yRotation=Mathf.LerpAngle(transform.rotation.y,targetRotation.eulerAngles.y,rotationProgress);
        transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);
        return true;
    }
    bool FlyUp()
    {
        if (transform.position.y>=height) return false;
        transform.Translate(transform.up * flyUpSpeed*Time.deltaTime);
        return true;
    }

    void Fall()
    {
        if (transform.position.y <= 0) return;
        transform.Translate(-transform.up * flyUpSpeed * Time.deltaTime);
    }
    //public void PlayAttackSound()
    //{
    //    audioSource.clip = attackSound;
    //    audioSource.PlayOneShot(attackSound);
    //}
    //public void PlayFlyingSound()
    //{
    //    int randomSound = Random.Range(0, movingSounds.Length);
    //    audioSource.clip = movingSounds[randomSound];
    //    audioSource.PlayOneShot(movingSounds[randomSound]);
    //}
}
