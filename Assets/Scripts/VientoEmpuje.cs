using UnityEngine;

public class VientoEmpuje : MonoBehaviour
{
    public float fuerzaViento = 10f; // Fuerza que eleva al jugador

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica si el objeto tiene el tag "Player" antes de aplicar la fuerza
        if (other.CompareTag("Player"))
        {
            // Obtén el componente del script que contiene la función JumpBoost
            var playerScript = other.GetComponentInParent<PlayerMovement>();    
            if (playerScript != null)
            {
                playerScript.JumpBoost(fuerzaViento); // Llama a la función JumpBoost
                Debug.Log("JumpBoost activado con fuerza: " + fuerzaViento);
            }
        }
    }
}
