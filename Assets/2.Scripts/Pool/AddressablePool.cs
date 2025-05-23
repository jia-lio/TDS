using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Pool
{
    public class AddressablePool<T> : IDisposable where T : Component
    {
        private readonly string _key;
        private readonly Transform _parent;
        private readonly Queue<T> _components = new();

        public AddressablePool(string key, Transform parent)
        {
            _key = key;
            _parent = parent;
        }

        public void Init(int capacity)
        {
            InitAsync(capacity).Forget();
        }

        private async UniTask InitAsync(int capacity)
        {
            while (_components.Count < capacity)
            {
                _components.Enqueue(await Create());
            }    
        }

        private async UniTask<T> Create()
        {
            var instance = await Addressables.InstantiateAsync(_key, _parent);
            instance.SetActive(false);

            var component = instance.GetComponent<T>();

            return component;
        }
        
        public async UniTask<T> Rent(Transform parent)
        {
            var component = _components.Any() ? _components.Dequeue() : await Create();
            if (component == null)
                return null;
            
            component.gameObject.SetActive(true);
            component.transform.SetParent(parent);
            component.transform.localPosition = Vector3.zero;

            return component;
        }

        public void Return(T component)
        {
            if (component == null || _components.Contains(component))
                return;
            
            component.gameObject.SetActive(false);
            component.transform.SetParent(_parent);
            _components.Enqueue(component);
            component.gameObject.OnDestroyAsync();
        }

        public void Dispose()
        {
            try
            {
                while (_components.Any())
                {
                    var gameObject = _components.Dequeue().gameObject;
                    if (Addressables.ReleaseInstance(gameObject) == false)
                    {
                        Object.Destroy(gameObject);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}