using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    public List<Level> levels;
    public PlayerInfo player;
    public int curLevel;
    public int curExpNeeded;

    public void Awake()
    {
        curExpNeeded = levels[curLevel].expNeeded;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if(curExpNeeded <= 0)
            {
                for (int i = 0; i < levels[curLevel].statIncrease.Count; i++)
                {
                    Stats s = levels[curLevel].statIncrease[i].stat;

                    switch (s)
                    {
                        case Stats.Health:
                            player.health += levels[curLevel].statIncrease[i].increaseBy;
                            break;

                        case Stats.Speed:
                            player.speed += levels[curLevel].statIncrease[i].increaseBy;
                            break;

                        case Stats.Strength:
                            player.strength += levels[curLevel].statIncrease[i].increaseBy;
                            break;

                    }
                }

                curLevel++;
                curExpNeeded = levels[curLevel].expNeeded;
            }
            else
            {
                curExpNeeded -= 5;
            }

        }
    }
}
