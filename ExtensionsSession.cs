/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace System
{
    public static class ExtensionsSession
    {
        public static IDictionary<string, object> ToDictionary(this HttpSessionState httpSessionState)
        {
            return new SessionDictionary(httpSessionState);
        }

        public static IDictionary ToDictionaryNotGeneric(this HttpSessionState httpSessionState)
        {
            return new SessionDictionary(httpSessionState);
        }

        private class SessionDictionary : IDictionary<string, object>, IDictionary
        {
            readonly HttpSessionState _sessionState;

            public SessionDictionary(HttpSessionState sessionState)
            {
                this._sessionState = sessionState;
            }

            public bool ContainsKey(string key)
            {
                return _sessionState[key] != null;
            }

            public void Add(string name, object value)
            {
                _sessionState.Add(name, value);
            }

            public bool Remove(string name)
            {
                if (!ContainsKey(name)) return false;
                _sessionState.Remove(name);
                return true;
            }

            public bool TryGetValue(string key, out object value)
            {
                if (ContainsKey(key))
                {
                    value = this[key];
                    return true;
                }

                value = null;
                return false;
            }

            public void Add(KeyValuePair<string, object> item)
            {
                Add(item.Key, item.Value);
            }

            public bool Contains(object key)
            {
                return ContainsKey((string)key);
            }

            public void Add(object key, object value)
            {
                Add((string)key, value);
            }

            public void Clear()
            {
                _sessionState.Clear();
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                Remove((string)key);
            }

            object IDictionary.this[object key]
            {
                get { return this[(string)key]; }
                set { this[(string)key] = value; }
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                if (!ContainsKey(item.Key)) return false;
                return Equals(this[item.Key], item.Value);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                CopyTo((Array)array, arrayIndex);
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                if (!Contains(item)) return false;
                return Remove(item.Key);
            }

            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                return KeyValueEnumerable().GetEnumerator();
            }

            IEnumerable<KeyValuePair<string, object>> KeyValueEnumerable()
            {
                return Keys.Select(key => new KeyValuePair<string, object>(key, this[key]));
            }

            public IEnumerator GetEnumerator()
            {
                return _sessionState.GetEnumerator();
            }

            public object this[string name]
            {
                get { return _sessionState[name]; }
                set { _sessionState[name] = value; }
            }

            public void CopyTo(Array array, int index)
            {
                _sessionState.CopyTo(array, index);
            }

            public int Count
            {
                get { return _sessionState.Count; }
            }

            public object SyncRoot
            {
                get { return _sessionState.SyncRoot; }
            }

            public bool IsSynchronized
            {
                get { return _sessionState.IsSynchronized; }
            }

            public ICollection<string> Keys
            {
                get { throw new NotImplementedException(); }
            }

            ICollection IDictionary.Values
            {
                get { throw new NotImplementedException(); }
            }

            ICollection IDictionary.Keys
            {
                get { throw new NotImplementedException(); }
            }

            public ICollection<object> Values
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { return _sessionState.IsReadOnly; }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }
        }

    }


}
*/