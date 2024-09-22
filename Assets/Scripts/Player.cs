using UnityEditor.SearchService;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    public bool EnemiesClose = false;
    public float MoveSpeed = 3.0f;
    public float rotationSpeed = 5f;
    private float lastSuperAttackTime = -Mathf.Infinity;
    public float superAttackCooldown = 2f;

    private float lastAttackTime = 0;
    public bool isDead = false;
    public Animator AnimatorController;

    private void Update()
    {
        SceneManager.Instance.ShowHp();
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }
        // атака
        if (Input.GetMouseButtonDown(1) && SceneManager.Instance.AttackButton.interactable)
        {
            SceneManager.Instance.AttackButton.onClick.Invoke();
        }
        if (SceneManager.Instance.SuperAttackReady != false)
        {
            ScanForEnemies();
            SceneManager.Instance.SuperAttackButton.interactable = EnemiesClose;
        }

        // рух
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * MoveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // анімація руху
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            AnimatorController.SetFloat("Speed", 1f);
        }
        else
        {
            AnimatorController.SetFloat("Speed", 0f);
        }
        // повороти граівця в напрямку руху
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.Space) && SceneManager.Instance.SuperAttackReady && EnemiesClose)
        {
            SceneManager.Instance.SuperAttackButton.onClick.Invoke();
            lastSuperAttackTime = Time.time;
        }
    }
    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }
    private Enemie goblin;
    private float distance;
    public void Attack(Enemie closestEnemie)
    {
        if (closestEnemie != null)
        {
            goblin = closestEnemie;
            distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                AnimatorController.SetTrigger("Attack");
                if (distance <= AttackRange)
                {
                    transform.rotation = Quaternion.LookRotation(goblin.transform.position - transform.position);
                }

            }
        }
        else
        {
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                AnimatorController.SetTrigger("Attack");

            }
        }
    }
    public void SuperAttack(Enemie closestEnemie)
    {
        if (closestEnemie != null)
        {
            goblin = closestEnemie;
            distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                AnimatorController.SetTrigger("Super_Attack");
                if (distance <= AttackRange)
                {
                    transform.rotation = Quaternion.LookRotation(goblin.transform.position - transform.position);
                }
            }
        }
    }
    public Enemie EnemieChecker()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }
        return closestEnemie;
    }
    private void ScanForEnemies()
    {
        var enemies = SceneManager.Instance.Enemies;
        EnemiesClose = false;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            if (distance <= AttackRange)
            {
                EnemiesClose = true;
                break;
            }
        }
    }
    public void Hit()
    {
        if (goblin != null)
        {
            if (distance <= AttackRange)
            {
                goblin.Hp -= Damage;
                goblin = null;
            }
        }
    }

}
