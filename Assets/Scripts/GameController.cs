using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public List<GameObject> weapons = new List<GameObject>();
    public Vector3 weaponSpawnValues;
    public float weaponSpawnWait;

    // Use this for initialization
    void Start ()
    {
        SpawnPlayers();
        StartCoroutine(SpawnWeaponWaves());
    }

	// Update is called once per frame
	void Update ()
    {

	}

    IEnumerator SpawnWeaponWaves()
    {
        if (weapons.Count == 0)
            yield return new WaitForSeconds(weaponSpawnWait);
        else
        {
            while (true)
            {
                GameObject weapon = weapons[Random.Range(0, weapons.Count)];

                Vector3 spawnPosition = new Vector3(Random.Range(-weaponSpawnValues.x, weaponSpawnValues.x), weaponSpawnValues.y, weaponSpawnValues.z);
                Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
                Instantiate(weapon, spawnPosition, spawnRotation);

                yield return new WaitForSeconds(weaponSpawnWait);
            }
        }
    }

    private void SpawnPlayers ()
    {
        var playerSpawnpoints = GameObject.FindGameObjectWithTag("PlayerSpawnpoints");
        var spawnpointsList = new List<Vector3>();
        for (int i = 0; i < playerSpawnpoints.transform.childCount; i++)
        {
            spawnpointsList.Add(playerSpawnpoints.transform.GetChild(i).transform.position);
        }

        for (int i = 0; i < Globals.Players.Count; i++)
        {
            var targetSpawnpointId = Random.Range(0, spawnpointsList.Count);
            var spawnpoint = spawnpointsList[targetSpawnpointId];

            var obj = Instantiate(CharactersManager.CharactersList[Globals.Players[i].CharacterId].CharacterPrefab, spawnpoint, new Quaternion());
            obj.GetComponent<PlayerInputManager>().PlayerId = Globals.Players[i].ControlsId;

            spawnpointsList.RemoveAt(targetSpawnpointId);
        }
    }
}
