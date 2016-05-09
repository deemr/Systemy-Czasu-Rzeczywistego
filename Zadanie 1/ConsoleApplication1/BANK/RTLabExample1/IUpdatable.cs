using System;

interface IUpdatable
{
    void Update(float time);

    IEnumerable<float> CoroutineUpdate(float time);
}