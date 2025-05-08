using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public EnemyType enemyType; // Vinculaci�n con el Scriptable Object
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player'. Aseg�rate de asignarlo.");
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false; //  Prevenir NullReferenceException antes de calcular la distancia

        return Vector2.Distance(transform.position, player.position) <= enemyType.detectionRange;
    }
}
