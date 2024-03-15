using System;

public class Hero
{
    public string Name { get; set; }
    public int Health { get; set; }

    public Hero(string name, int health)
    {
        Name = name;
        Health = health;
    }
    public void Attack()
    {
        Console.WriteLine($"{Name} attacks the enemy!");
    }
    public void Defend()
    {
        Console.WriteLine($"{Name} raises the shield to defend against the enemy's attack!");
    }
}