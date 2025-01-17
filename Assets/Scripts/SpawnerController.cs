using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y+15f);
    }
}
