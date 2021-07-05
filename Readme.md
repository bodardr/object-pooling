Object Pooling pattern using Scriptable Objects.
Inspired heavily by Aarthificial's video : https://www.youtube.com/watch?v=tWa-3mijV24&ab_channel=aarthificial

There aren't any samples yet, so you will have to figure it out yourself. 
Here's an overview of how it works.

The Pool <T> : Contains references to its recyclable objects. Can be called using Get() and can recycle using Retrieve.
T stands for the pooled object's type

Hierarchy : IObjectPool<T> -> ObjectPool<T> -> ScriptableObjectPool

The Poolable Object : The wrapper around the pooled object. Contains a function to return to its pool (Release()) as long as references to the Content and its Scene.

ScriptableObjectPool, as the name suggests, lets you make prefab pools inside a scriptable object. 
You can then reference this pool inside any script and call Get() on this pool specifically.
It allows for multiple pools to have the same object without confusion.