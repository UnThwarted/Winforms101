using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeSandbox
{
    // IndexerGeneric - This program demonstrates the use of the index operator to provide
    //    access to an array using a string as an index. This version is generic,
    //    permitting storage of any type, still keyed by string. This makes the
    //    KeyedArray<T> similar to a Dictionary, but where Dictionary<TKey, TValue> is
    //    unordered, unsortable, and unindexable, KeyedArray<T> is all of those.
        public class KeyedArray<T>
        {
            // The following string provides the "key" into the array -
            // the key is the string used to identify an element.
            private List<string> _keys;

            // The T object is the actual data associated with that key.
            private List<T> _storedItems;

            // KeyedArray - Create a flexible KeyedArray.
            public KeyedArray()
            {
                // Each key corresponds to a storedItem.
                _keys = new List<string>();
                _storedItems = new List<T>();
            }

            // Find - Find the index of the record corresponding to the string
            //    targetKey (return negative if can't be found).
            // NOTE: Since this method is only called in two places, you could
            // just "inline" it there and eliminate this method. However,
            // I find the method, and its name, clearer than the IndexOf call.
            private int Find(string targetKey)
            {
                return _keys.IndexOf(targetKey);
                // Here's what this line replaces from the nongeneric version:
                //for(int i = 0; i < keys.Length; i++)
                //{
                //  if (String.Compare(keys[i], targetKey) == 0)
                //  {
                //    return i;
                //  }
                //}
                //return -1;
            }

            // No longer needed.
            // FindEmpty - Find room in the array for a new entry.
            //private int FindEmpty()
            //{
            //  for (int i = 0; i < keys.Length; i++)
            //  {
            //    if (keys[i] == null)
            //    {
            //      return i;
            //    }
            //  }

            //  throw new Exception("Array is full");
            //}

            // Indexer- Look up contents by string key - this is the indexer.
            //    Note that it screens out duplicate keys, but you could choose
            //    to implement it without that.
            public T this[string key]
            {
                set
                {
                    // See if the string isn't already there.
                    int index = Find(key);  // Here's where you could inline Find().
                    if (index < 0)
                    {
                        // Not found, so add it.
                        _keys.Add(key);
                        _storedItems.Add(value);
                    }
                    else
                    {
                        // Just replace the item already at that key.
                        _storedItems[index] = value;
                    }
                    // Here's what the code above replaced:
                    //if (index < 0)
                    //{
                    //  // It isn't -  find a new spot.
                    //  index = FindEmpty();
                    //  keys[index] = key;
                    //}

                    //// Save the object in the corresponding spot.
                    //arrayElements[index] = value;
                }

                get
                {
                    int index = Find(key);    // Other place you could inline Find().
                    if (index < 0)
                    {
                        // We have to return something, so ...
                        return default(T);  // Whatever 'null' is for T.
                    }
                    return _storedItems[index];
                }
            }
        }
    }
