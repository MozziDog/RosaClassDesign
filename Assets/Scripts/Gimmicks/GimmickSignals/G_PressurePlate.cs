using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_PressurePlate : GimmickSignal
{
    public Sprite active;
    public Sprite inactive;
    public int count = 0;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetSprite()
    {
        if (isActive)
        {
            spriteRenderer.sprite = active;
        }
        else
        {
            spriteRenderer.sprite = inactive;
        }
    }

    bool CheckTag(string tag)
    {
        if(tag == "Player" ||
           tag == "Cube" ||
           tag == "Monster")
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CheckTag(collision.tag))
            return;

        count++;

        if (count > 0)
        {
            isActive = true;
            SetSprite();
            SendSignal();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CheckTag(collision.tag))
            return;

        count--;

        if (count < 1)
        {
            isActive = false;
            SetSprite();
            SendSignal();
        }
    }
}