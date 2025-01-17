using System.Collections;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] float changeFactor = 0.01f;
    [SerializeField] float moveFactor = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("UpLava");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(playerObject.transform.position.x, transform.position.y);
    }
        
    private IEnumerator UpLava()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + moveFactor);
        yield return new WaitForSeconds(Mathf.Pow(Mathf.Epsilon, changeFactor*Time.fixedDeltaTime));
        StartCoroutine("UpLava");
    }
}
