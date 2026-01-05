/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  对象池实现
 *  负责管理创建与销毁对象
 *  避免频繁的创建与销毁操作
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;

namespace ObjPool
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public sealed class ObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> _pool = new();
        private readonly Action<T> _reset;  // 默认reset方法

        public ObjectPool(Action<T> reset, int initialSize = 0)
        {
            _reset = reset;
            for (int i = 0; i < initialSize; i++)
                _pool.Push(new T());
        }

        public T GetObject()
        {
            return _pool.Count > 0 ? _pool.Pop() : new T();
        }

        public void ReturnObject(T obj)
        {
            _reset?.Invoke(obj);
            _pool.Push(obj);
        }
    }
}