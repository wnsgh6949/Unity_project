using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
	GameObject _player;
	ShootingController _playerShoot;
	HPController _playerHP;
	NavMeshAgent _agent;
	Animator _animator;
	Transform _map;
	GameUI _gameUI;
	
	public HPController enemy;
	public HPController enemy_G;

	// Update()
	Wave currentWave;
	int enemiesRemainingToSpawn;
	float nextSpawnTime;

	// OneEnemyDeath()
	public event System.Action OnWaveOver;
	public event System.Action OnGameClear;
	public Text waveUI;
	int enemiesRemainingAlive;

	// NextWave()
	public Wave[] waves;
	public event System.Action<int> OnNewWave;
	public int currentWaveNumber;

	[System.Serializable]
	public class Wave {
		public int enemyCount;
		public float timeBetweenSpawns;

	}

	void Start() {
		_player = GameObject.FindWithTag("Player");
		_playerShoot = _player.GetComponent<ShootingController>();
		_playerHP = _player.GetComponent<HPController>();
		_agent = _player.GetComponent<NavMeshAgent>();
		_animator = _player.GetComponent<Animator>();
		_map = FindObjectOfType<MapGenerator>().transform;
		_gameUI = FindObjectOfType<GameUI>();
	}

	void Update() {
		if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) {
			enemiesRemainingToSpawn--;
			nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

			if(Random.value < 0.25f)
			{
				HPController spawnedEnemy = Instantiate(enemy_G, transform.position + Vector3.forward * Random.Range(-2.5f, 2.5f), Quaternion.identity) as HPController;
				spawnedEnemy.transform.localScale *= Random.Range(0.9f, 1.1f);
				spawnedEnemy.OnDeath += OnEnemyDeath;
			}
			else
			{
				HPController spawnedEnemy = Instantiate(enemy, transform.position + Vector3.forward * Random.Range(-2.5f, 2.5f), Quaternion.identity) as HPController;
				spawnedEnemy.OnDeath += OnEnemyDeath;
			}
		}
	}

	void OnEnemyDeath() {
		waveUI.text = "" + --enemiesRemainingAlive;

		if(enemiesRemainingAlive == 0 && enemiesRemainingToSpawn == 0)
		{
			if(currentWaveNumber == waves.Length)
			{
				OnGameClear();
			}
			else
			{
				OnWaveOver();
			}
		}
	}

	void ResetPlayerPosition() {
		_agent.nextPosition = _map.position;
	}

	public void NextWave() {
		currentWaveNumber++;
		if(currentWaveNumber - 1 < waves.Length)
		{
			currentWave = waves[currentWaveNumber-1];
		
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;
			waveUI.text = "" + enemiesRemainingAlive;

			if(OnNewWave != null)
			{
				OnNewWave(currentWaveNumber);
			}
			_playerShoot.ResetAmmo();
			_playerHP.ResetHealth();
			_playerHP.isCovered = false;
			_animator.SetBool("IsCovered", false);
			ResetPlayerPosition();
			StartCoroutine(InvincibleTime());
		}
	}

	IEnumerator InvincibleTime()
    {
        _playerHP.isInvincible = true;
        yield return new WaitForSeconds(2f);
        _playerHP.isInvincible = false;
    }
}
