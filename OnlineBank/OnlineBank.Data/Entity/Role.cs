﻿namespace OnlineBank.Data.Entity;

public class Role:BaseEntity
{
    public string Name { get; set; }
    public int Level { get; set; }
}