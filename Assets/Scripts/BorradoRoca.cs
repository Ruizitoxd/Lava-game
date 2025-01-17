using UnityEngine;

public class BorradoRoca : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void OnTriggerEnter2D(Collider2D other) {
        /*if (other.CompareTag("Player")){
            Player.SetActive(false);
        }*/

        if(other.CompareTag("Lava")){
            Destroy(gameObject);
        }


        
    }
}
