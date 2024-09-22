using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;
    public Button SuperAttackButton;
    public Button AttackButton;
    private Text SuperAttackButtonText;
    private Text AttackButtonText;
    public Text WaveText;
    public Text HpText;
    public bool SuperAttackReady = true;
    public GameObject Enemy;

    private int currWave = 0;
    [SerializeField] private LevelConfig Config;

    private void Awake()
    {
        Instance = this;
        SuperAttackButtonText = SuperAttackButton.GetComponentInChildren<Text>();
        AttackButtonText = AttackButton.GetComponentInChildren<Text>();

    }

    private void Start()
    {
        SpawnWave();
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if (Enemies.Count == 0)
        {
            SpawnWave();
            Player.Hp = 50;
        }
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length)
        {
            Win.SetActive(true);
            return;
        }

        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;
        WaveText.text = "Хвиля " + currWave + " з " + Config.Waves.Length;

    }
    public void Attack()
    {
        Player.Attack(Player.EnemieChecker());
        StartCoroutine(AttackCooldownCoroutine());
    }
    public void SuperAttack()
    {
        Player.SuperAttack(Player.EnemieChecker());
        StartCoroutine(SuperAttackCooldownCoroutine());
    }
    private IEnumerator SuperAttackCooldownCoroutine()
    {
        AttackButton.interactable = false;
        SuperAttackReady = false;
        SuperAttackButton.interactable = SuperAttackReady;
        float cooldownRemaining = 2f;
        while (cooldownRemaining > 0)
        {
            SuperAttackButtonText.text = cooldownRemaining.ToString("F1") + "c";
            yield return new WaitForSeconds(0.1f);
            cooldownRemaining -= 0.1f;
            if (Mathf.Approximately(cooldownRemaining, 1.0f))
            {
                AttackButton.interactable = true;
                Debug.Log("Кнопка атаки включена");
            }
        }
        SuperAttackReady = true;
        SuperAttackButtonText.text = "Супер атака\n(Пробіл)";
        SuperAttackButton.interactable = Player.EnemiesClose;
    }
    private IEnumerator AttackCooldownCoroutine()
    {
        AttackButton.interactable = false;
        SuperAttackReady = false;
        float cooldownRemaining = 1f;
        while (cooldownRemaining > 0)
        {
            AttackButtonText.text = cooldownRemaining.ToString("F1") + "c";
            yield return new WaitForSeconds(0.1f);
            cooldownRemaining -= 0.1f;
        }
        AttackButtonText.text = "Атака\n(ПКМ)";
        AttackButton.interactable = true;
        SuperAttackReady = true;
    }
    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void ShowHp()
    {
        HpText.text = "HP: " + Player.Hp;
    }

}
