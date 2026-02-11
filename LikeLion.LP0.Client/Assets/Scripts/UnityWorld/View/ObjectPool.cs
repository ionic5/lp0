using UnityEngine;

namespace LikeLion.LP0.Client.UnityWorld.View
{
    public class ObjectPool : MonoBehaviour
    {
        private UnityEngine.Pool.ObjectPool<GameObject> _pool;

        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private GameObject _root;

        void Awake()
        {
            _pool = new UnityEngine.Pool.ObjectPool<GameObject>(
                () => Instantiate(_prefab, _root == null ? null : _root.transform),
                (obj) => obj.SetActive(true),
                (obj) => obj.SetActive(false));
        }

        public GameObject Get()
        {
            return _pool.Get();
        }

        public void Return(GameObject gameObject)
        {
            _pool.Release(gameObject);
        }
    }
}
