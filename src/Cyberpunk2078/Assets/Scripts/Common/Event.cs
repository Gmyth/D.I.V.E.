using UnityEngine.Events;


public class Event<T> : UnityEvent<T>
{
}

public class Event2<T> : UnityEvent<T, T>
{
}
