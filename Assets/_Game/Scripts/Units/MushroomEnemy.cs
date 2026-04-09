using UnityEngine;

public class MushroomEnemy : Enemy
{
    // How it works:
    //Operates within a radius of one tile.
    // Behavior:
    //When it detects one or more towers, it stops moving and begins the capture process.
    //Once a tower is captured, it converts it to its side (making it hostile to other towers).
    //If there are no towers nearby, it destroys the captured ones and continues moving until it finds new targets.
    //After it is destroyed, all captured towers are returned to the player.
    // Captured tower behavior:
    //Captured towers attack normal (player-owned) towers.
    // Normal tower behavior:
    //Captured towers become the highest-priority targets for normal towers.
    protected override void Attack()
    {
    }
}
