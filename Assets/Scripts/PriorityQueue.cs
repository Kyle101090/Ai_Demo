using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<Tuple<T, float>> elements = new List<Tuple<T, float>>();

    public int Count { get { return elements.Count; } }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new Tuple<T, float>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item2 < elements[bestIndex].Item2)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public void UpdatePriority(T item, float priority)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item1.Equals(item))
            {
                elements[i] = new Tuple<T, float>(item, priority);
                return;
            }
        }

        throw new ArgumentException("Item not found in priority queue.");
    }
}


