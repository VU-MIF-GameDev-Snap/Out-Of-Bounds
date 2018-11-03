using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject[] weapons;
    public Vector3 weaponSpawnValues;
    public float weaponSpawnWait;

    // Use this for initialization
    void Start () {
        StartCoroutine(SpawnWeaponWaves());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnWeaponWaves()
    {
        while (true)
        {
            GameObject weapon = weapons[Random.Range(0, weapons.Length)];

            Vector3 spawnPosition = new Vector3(Random.Range(-weaponSpawnValues.x, weaponSpawnValues.x), weaponSpawnValues.y, weaponSpawnValues.z);
            Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
            Instantiate(weapon, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(weaponSpawnWait);
        }
    }
}
