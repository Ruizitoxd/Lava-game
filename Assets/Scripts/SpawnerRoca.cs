using UnityEngine;
using System.Collections;

public class SpawnerRoca : MonoBehaviour
{
    public GameObject Roca; 
    public Transform topLeft; 
    public Transform bottomRight; 
    public float spawnInterval = 5f;
    private bool spawningBool = true;

    void Start()
    {
        StartCoroutine(spawning());
    }

    IEnumerator spawning()
    {
        while (spawningBool)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRoca();
        }
    }

    void SpawnRoca()
    {
       
        float randomX = Random.Range(topLeft.position.x, bottomRight.position.x);
        float randomY = Random.Range(topLeft.position.y, bottomRight.position.y);

        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
        Instantiate(Roca, spawnPosition, Quaternion.identity); 
    }
}
