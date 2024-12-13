using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Animator anim;
    private bool isDeath;
    public float disappearTime = 0.5f;
    // private EnemySpawner spawner;
    


    void Start()
        {
            currentHealth = maxHealth;
            anim = GetComponent<Animator>();
            // spawner = FindObjectOfType<EnemySpawner>();
        }
    void Update(){
        if(isDeath){return;}
    }
     public void TakeDamage(int damage){
        if(isDeath){return; }
        currentHealth -= damage;
        anim.SetTrigger("Hurt");
         if (currentHealth <= 0) 
        {
            Dead();
        }
    }

    private void Dead(){
        isDeath = true;
        anim.SetTrigger("Death");

        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null){
            enemyCollider.enabled = false;
        }
    //     if (spawner != null)
    // {
    //     spawner.DecreaseEnemyCount();
    // }


        StartCoroutine(Disappear());
    }
     private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearTime);
        Destroy(gameObject); 
    }

}
