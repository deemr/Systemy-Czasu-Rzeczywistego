using System;


public class Agent : IUpdatable
{
    public int Id { get; private set; }

    public Agent(int id)
	{
        Id = id;
	}

    
}
