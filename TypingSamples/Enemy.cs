using System;

public class Enemy : IEnemy
{
    public string Name { get; set; }
    public int Health { get; set; }

    public Enemy(string name, int health)
    {
        Name = name;
        Health = health;
    }
    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Console.WriteLine($"{Name} has been defeated!");
        }
    }
}