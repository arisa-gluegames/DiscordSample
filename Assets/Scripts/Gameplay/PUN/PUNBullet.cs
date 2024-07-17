using Photon.Realtime;

public class PUNBullet : Bullet
{
    public Player Owner { get; private set; }

    public void InitializeValues(float damage, Player owner)
    {
        this.Damage = damage;
        this.Owner = owner;
    }
}
