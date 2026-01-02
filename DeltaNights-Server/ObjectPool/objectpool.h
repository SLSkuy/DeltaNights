#pragma once

#include <QMutex>
#include <QQueue>
#include <QMutexLocker>

// 前置声明，避免循环
template<typename T>
struct ProtoResetter
{
    static void Reset(T*) {}
};

template<typename T>
class ObjectPool
{
public:
    explicit ObjectPool(int initNum = 32)
    {
        for (int i = 0; i < initNum; ++i)
        {
            m_pool.enqueue(new T());
        }
    }

    ~ObjectPool()
    {
        QMutexLocker locker(&m_mutex);
        while (!m_pool.isEmpty())
        {
            delete m_pool.dequeue();
        }
    }

    T* Acquire()
    {
        QMutexLocker locker(&m_mutex);

        if (m_pool.isEmpty())
            return new T();

        return m_pool.dequeue();
    }

    void Release(T* obj)
    {
        if (!obj) return;

        // 重置Protobuf对象
        ProtoResetter<T>::Reset(obj);

        QMutexLocker locker(&m_mutex);
        m_pool.enqueue(obj);
    }

private:
    QMutex m_mutex;
    QQueue<T*> m_pool;
};
