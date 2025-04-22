using System.Collections;
public interface IDamageable
{
    public void DealDamage(float damaged);
}
public interface IsAttackble
{
    public float Damaged { get; }

    public void OnCollider();
    public IEnumerator ReEnabled();
}