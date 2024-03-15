public interface IEnemy
{
    string Name {get; set;}
    int Health {get; set;}
    void TakeDamage(int damageAmount);
}