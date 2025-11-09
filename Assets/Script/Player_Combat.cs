using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = 2f;
    private float timer;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        // ตรวจปุ่ม K
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetTrigger("Attack"); // Animator ต้องมี Trigger ชื่อ "Attack"
            timer = cooldown;
            Debug.Log("Attack triggered!");
        }
    }

    public void FinishAttacking()
    {
        Debug.Log("Attack finished.");
    }
}
