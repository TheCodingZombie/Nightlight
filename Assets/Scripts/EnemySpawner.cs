using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private bool allow = true;
    public GameObject enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if ((collision.gameObject.tag == "Player") && allow){
            var pos = new Vector2(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y + 2.0f);
            Instantiate(enemy, pos, transform.rotation);
            allow = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        allow = true;
    }
}
