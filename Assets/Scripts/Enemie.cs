using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    public bool MegaEnemy;

    public Animator AnimatorController;
    public NavMeshAgent Agent;
    public float GiveHp;

    private float lastAttackTime = 0;
    private bool isDead = false;


    private void OnEnable()
    {
        SceneManager.Instance.AddEnemie(this);
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            if (MegaEnemy)
            {
                SpawnRegularEnemies();

            }
            Die();
            Agent.isStopped = true;
            return;
        }

        if (SceneManager.Instance.Player.isDead)
        {
            Agent.isStopped = true;
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);

        if (distance <= AttackRange)
        {
            Agent.isStopped = true;
            if (Time.time - lastAttackTime > AtackSpeed && !SceneManager.Instance.Player.isDead)
            {
                lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            if (Agent.isOnNavMesh)
            {
                Agent.isStopped = false;
                Agent.SetDestination(SceneManager.Instance.Player.transform.position);
            }
            else
            {
                Debug.LogError("Agent is not on NavMesh");
            }
        }

        if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
        {
            if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
            {
                Agent.SetDestination(SceneManager.Instance.Player.transform.position);
            }
        }

        AnimatorController.SetFloat("Speed", Agent.velocity.magnitude);
    }
    public void Die()
    {
        if (SceneManager.Instance.Player.Hp < 50)
        {
            SceneManager.Instance.Player.Hp = Mathf.Min(SceneManager.Instance.Player.Hp + GiveHp, 50);
        }
        isDead = true;
        SceneManager.Instance.RemoveEnemie(this);
        AnimatorController.SetTrigger("Die");
    }
    private void SpawnRegularEnemies()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 offset1 = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            Instantiate(SceneManager.Instance.Enemy, transform.position + offset1, Quaternion.identity);
        }
    }

}
