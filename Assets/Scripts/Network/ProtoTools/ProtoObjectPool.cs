using ObjPool;

namespace Network.ProtoTools
{
    /// <summary>
    /// 通用 Protobuf 对象池封装
    /// </summary>
    internal sealed class ProtoObjectPool<T> where T : class, IPoolable, new()
    {
        private readonly ObjectPool<T> _pool;

        public ProtoObjectPool(int capacity)
        {
            _pool = new ObjectPool<T>(obj => obj.Reset(), capacity);
        }

        public T Get() => _pool.GetObject();

        public void Return(T obj)
        {
            if (obj != null)
                _pool.ReturnObject(obj);
        }
    }
}