using System;
using System.Collections.Generic;

namespace EventProcess
{
    /// <summary>
    /// 事件类
    /// </summary>
    public interface ISignal
    {
        string Hash { get; }
    }
    
    public static class Signals
    {
        private static readonly SignalHub hub = new SignalHub();

        public static SType Get<SType>() where SType : ISignal, new() {
            return hub.Get<SType>();
        }
    }

    /// <summary>
    /// 事件中心
    /// </summary>
    public class SignalHub
    {
        private Dictionary<Type, ISignal> signals = new Dictionary<Type, ISignal>();

        /// <summary>
        /// 根据类型获取事件
        /// </summary>
        public SType Get<SType>() where SType : ISignal, new() {
            Type signalType = typeof(SType);
            ISignal signal;

            if (signals.TryGetValue(signalType, out signal)) {
                return (SType) signal;
            }

            return (SType) Bind(signalType);
        }

        /// <summary>
        /// 手动提供一个事件的哈希，并将其绑定到给定的监听器
        /// </summary> 
        public void AddListenerToHash(string signalHash, Action handler) {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal) {
                (signal as ASignal).AddListener(handler);
            }
        }
        
        public void RemoveListenerFromHash(string signalHash, Action handler) {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal) {
                (signal as ASignal).RemoveListener(handler);
            }
        }

        private ISignal Bind(Type signalType) {
            ISignal signal;
            if (signals.TryGetValue(signalType, out signal)) {
                UnityEngine.Debug.LogError(string.Format("Signal already registered for type {0}",
                    signalType.ToString()));
                return signal;
            }

            signal = (ISignal) Activator.CreateInstance(signalType);
            signals.Add(signalType, signal);
            return signal;
        }

        private ISignal Bind<T>() where T : ISignal, new() {
            return Bind(typeof(T));
        }

        private ISignal GetSignalByHash(string signalHash) {
            foreach (ISignal signal in signals.Values) {
                if (signal.Hash == signalHash) {
                    return signal;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 信号事件的抽象类
    /// </summary>
    public abstract class ABaseSignal : ISignal
    {
        protected string _hash;

        /// <summary>
        /// 本身的哈希值
        /// </summary>
        public string Hash {
            get {
                if (string.IsNullOrEmpty(_hash)) {
                    _hash = this.GetType().ToString();
                }

                return _hash;
            }
        }
    }

    /// <summary>
    /// 事件的具体实现
    /// </summary>
    public abstract class ASignal : ABaseSignal
    {
        private Action callback;

        /// <summary>
        /// 事件加监听
        /// </summary>
        public void AddListener(Action handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// 事件的移除监听
        /// </summary>
        public void RemoveListener(Action handler) {
            callback -= handler;
        }

        /// <summary>
        /// 广播事件
        /// </summary>
        public void Dispatch() {
            if (callback != null) {
                callback();
            }
        }
    }

    /// <summary>
    /// 带一个参数的事件
    /// </summary>
    public abstract class ASignal<T> : ABaseSignal
    {
        private Action<T> callback;

        /// <summary>
        /// 事件加监听
        /// </summary>
        public void AddListener(Action<T> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveListener(Action<T> handler) {
            callback -= handler;
        }

        /// <summary>
        /// 广播事件，带一个参数
        /// </summary>
        public void Dispatch(T arg1) {
            if (callback != null) {
                callback(arg1);
            }
        }
    }

    /// <summary>
    /// 同理，2个参数的事件
    /// </summary>
    public abstract class ASignal<T, U> : ABaseSignal
    {
        private Action<T, U> callback;
        
        public void AddListener(Action<T, U> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }
        
        public void RemoveListener(Action<T, U> handler) {
            callback -= handler;
        }
        
        public void Dispatch(T arg1, U arg2) {
            if (callback != null) {
                callback(arg1, arg2);
            }
        }
    }

    /// <summary>
    /// 同理三个参数的事件
    /// </summary>
    public abstract class ASignal<T, U, V> : ABaseSignal
    {
        private Action<T, U, V> callback;
        
        public void AddListener(Action<T, U, V> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }
        
        public void RemoveListener(Action<T, U, V> handler) {
            callback -= handler;
        }
        
        public void Dispatch(T arg1, U arg2, V arg3) {
            if (callback != null) {
                callback(arg1, arg2, arg3);
            }
        }
    }
}