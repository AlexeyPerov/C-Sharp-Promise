using RSG.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RSG.Exceptions;

// ReSharper disable once CheckNamespace
namespace RSG
{
    /// <summary>
    /// Implements a C# promise.
    /// https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Promise
    /// </summary>
    public interface IPromise<PromisedT> : ICancelable
    {
        /// <summary>
        /// Gets the id of the promise, useful for referencing the promise during runtime.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Set the name of the promise, useful for debugging.
        /// </summary>
        IPromise<PromisedT> WithName(string name);
        
        /// <summary>
        /// Current state of promise
        /// </summary>
        PromiseState CurState { get; }

        /// <summary>
        /// Completes the promise. 
        /// onResolved is called on successful completion.
        /// onRejected is called on error.
        /// </summary>
        void Done(Action<PromisedT> onResolved, Action<Exception> onRejected);

        /// <summary>
        /// Completes the promise. 
        /// onResolved is called on successful completion.
        /// Adds a default error handler.
        /// </summary>
        void Done(Action<PromisedT> onResolved);

        /// <summary>
        /// Complete the promise. Adds a default error handler.
        /// </summary>
        void Done();

        /// <summary>
        /// Handle errors for the promise. 
        /// </summary>
        IPromise Catch(Action<Exception> onRejected);

        /// <summary>
        /// Handle errors for the promise. 
        /// </summary>
        IPromise<PromisedT> Catch(Func<Exception, PromisedT> onRejected);

        /// <summary>
        /// Add a resolved callback that chains a value promise (optionally converting to a different value type).
        /// </summary>
        IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved);

        /// <summary>
        /// Add a resolved callback that chains a non-value promise.
        /// </summary>
        IPromise Then(Func<PromisedT, IPromise> onResolved);

        /// <summary>
        /// Add a resolved callback.
        /// </summary>
        IPromise Then(Action<PromisedT> onResolved);

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// The resolved callback chains a value promise (optionally converting to a different value type).
        /// </summary>
        IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved, 
            Func<Exception, IPromise<ConvertedT>> onRejected
        );

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// The resolved callback chains a non-value promise.
        /// </summary>
        IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected);

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// </summary>
        IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected);

        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// The resolved callback chains a value promise (optionally converting to a different value type).
        /// </summary>
        IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved,
            Func<Exception, IPromise<ConvertedT>> onRejected,
            Action<float> onProgress
        );

        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// The resolved callback chains a non-value promise.
        /// </summary>
        IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress);

        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// </summary>
        IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected, Action<float> onProgress);

        /// <summary>
        /// Return a new promise with a different value.
        /// May also change the type of the value.
        /// </summary>
        IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform);

        /// <summary>
        /// Chain an enumerable of promises, all of which must resolve.
        /// Returns a promise for a collection of the resolved results.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

        /// <summary>
        /// Chain an enumerable of promises, all of which must resolve.
        /// Converts to a non-value promise.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain);

        /// <summary>
        /// Takes a function that yields an enumerable of promises.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// </summary>
        IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

        /// <summary>
        /// Takes a function that yields an enumerable of promises.
        /// Converts to a non-value promise.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// </summary>
        IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain);

        /// <summary> 
        /// Add a finally callback. 
        /// Finally callbacks will always be called, even if any preceding promise is cancelled, rejected, or encounters an error.
        /// The returned promise will be resolved, rejected or cancelled as per the preceding promise.
        /// </summary> 
        void Finally(Action onComplete);
        
        /// <summary>
        /// Add a callback that chains a non-value promise.
        /// ContinueWith callbacks will always be called, even if any preceding promise is rejected, or encounters an error.
        /// The state of the returning promise will be based on the new non-value promise, not the preceding (rejected or resolved) promise.
        /// </summary>
        IPromise ContinueWith(Func<IPromise> onResolved);

        /// <summary> 
        /// Add a callback that chains a value promise (optionally converting to a different value type).
        /// ContinueWith callbacks will always be called, even if any preceding promise is rejected, or encounters an error.
        /// The state of the returning promise will be based on the new value promise, not the preceding (rejected or resolved) promise.
        /// </summary> 
        IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete);

        /// <summary>
        /// Add a progress callback.
        /// Progress callbacks will be called whenever the promise owner reports progress towards the resolution
        /// of the promise.
        /// </summary>
        IPromise<PromisedT> Progress(Action<float> onProgress);
        
        /// <summary>
        /// Add a cancel callback.
        /// </summary>
        /// <param name="onCancel"></param>
        void OnCancel(Action onCancel);
    }

    /// <summary>
    /// Interface for a promise that can be rejected.
    /// </summary>
    public interface IRejectable
    {
        /// <summary>
        /// Reject the promise with an exception. Doesn't use LogError.
        /// </summary>
        void RejectSilent(Exception ex);
        
        /// <summary>
        /// Reject the promise with an exception and LogError log.
        /// </summary>
        void Reject(Exception ex);
    }

    /// <summary>
    /// Interface for a promise that can be rejected or resolved.
    /// </summary>
    public interface IPendingPromise<PromisedT> : IRejectable
    {
        /// <summary>
        /// ID of the promise, useful for debugging.
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Just a shortcut to check whether the promise is in Pending state.
        /// </summary>
        bool CanBeResolved { get; }

        /// <summary>
        /// Resolve the promise with a particular value.
        /// </summary>
        void Resolve(PromisedT value);

        /// <summary>
        /// Report progress in a promise.
        /// </summary>
        void ReportProgress(float progress);
    }

    /// <summary>
    /// Specifies the state of a promise.
    /// </summary>
    public enum PromiseState
    {
        Pending,    // The promise is in-flight.
        Rejected,   // The promise has been rejected.
        Resolved,    // The promise has been resolved.
        Cancelled   // The promise has been cancelled.
    };

    /// <summary>
    /// Implements a C# promise.
    /// https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Promise
    /// </summary>
    public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>, IPromiseInfo
    {
        /// <summary>
        /// The exception when the promise is rejected.
        /// </summary>
        private Exception rejectionException;

        /// <summary>
        /// The value when the promises is resolved.
        /// </summary>
        private PromisedT resolveValue;

        /// <summary>
        /// Error handler.
        /// </summary>
        private List<RejectHandler> rejectHandlers;

        /// <summary>
        /// Cancel handler.
        /// </summary>
        private List<Promise.ResolveHandler> cancelHandlers;

        /// <summary>
        /// Progress handlers.
        /// </summary>
        private List<ProgressHandler> progressHandlers;

        /// <summary>
        /// Completed handlers that accept a value.
        /// </summary>
        private List<Action<PromisedT>> resolveCallbacks;
        private List<IRejectable> resolveRejectables;

        /// <summary>
        /// ID of the promise, useful for debugging.
        /// </summary>
        public int Id => id;

        private readonly int id;

        /// <summary>
        /// Name of the promise, when set, useful for debugging.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Tracks the current state of the promise.
        /// </summary>
        public PromiseState CurState { get; private set; }
        
        /// <summary>
        /// Shortcut property for checking if promise is pending.
        /// </summary>
        public bool CanBeResolved => CurState == PromiseState.Pending;
        
        /// <summary>
        /// Shortcut property for checking if promise is pending.
        /// </summary>
        public bool CanBeCanceled => CurState == PromiseState.Pending;
        
        /// <summary>
        /// Promise parent in chain.
        /// </summary>
        public ICancelable Parent { get; private set; }
        
        /// <summary>
        /// Promise children in chain.
        /// </summary>
        public HashSet<ICancelable> Children { get; } = new HashSet<ICancelable>();
        
        /// <summary>
        /// Get loggable name.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return string.IsNullOrEmpty(Name) ? "Promise" : $"Promise = {Name}";
        }

        public Promise(string name = null)
        {
            this.CurState = PromiseState.Pending;
            this.id = Promise.NextId();
            Name = name;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Add(this);
            }
        }

        public Promise(Action<Action<PromisedT>, Action<Exception>> resolver)
        {
            this.CurState = PromiseState.Pending;
            this.id = Promise.NextId();

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Add(this);
            }

            try
            {
                resolver(Resolve, RejectSilent);
            }
            catch (Exception ex)
            {
                RejectSilent(ex);
            }
        }

        private Promise(PromiseState initialState)
        {
            CurState = initialState;
            id = Promise.NextId();
        }
        
        /// <summary>
        /// Attach a parent in chain.
        /// </summary>
        /// <param name="parent"></param>
        public void AttachParent(ICancelable parent)
        {
            if (parent.Parent == this)
            {
                EventsReceiver.OnWarningMinor(
                    $"Skip attempt to create cycled refs in promises parents {GetName()}");
                return;
            }

            if (Parent != null)
            {
                EventsReceiver.OnWarningMinor($"Overwriting existing parent {GetName()}");
            }
            
            Parent = parent;
            parent.AttachChild(this);
        }

        /// <summary>
        /// Add a child in chain.
        /// </summary>
        /// <param name="child"></param>
        public void AttachChild(ICancelable child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Add a rejection handler for this promise.
        /// </summary>
        private void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
        {
            if (rejectHandlers == null)
            {
                rejectHandlers = new List<RejectHandler>();
            }

            rejectHandlers.Add(new RejectHandler { callback = onRejected, rejectable = rejectable });
        }

        /// <summary>
        /// Add a cancellation handler for this promise.
        /// </summary>
        /// <param name="onCanceled"></param>
        /// <param name="rejectable"></param>
        private void AddCancelHandler(Action onCanceled, IRejectable rejectable)
        {
            if (cancelHandlers == null)
            {
                cancelHandlers = new List<Promise.ResolveHandler>();
            }
                
            cancelHandlers.Add(new Promise.ResolveHandler {
                callback = onCanceled,
                rejectable = rejectable
            });
        }

        /// <summary>
        /// Add a resolve handler for this promise.
        /// </summary>
        private void AddResolveHandler(Action<PromisedT> onResolved, IRejectable rejectable)
        {
            if (resolveCallbacks == null)
            {
                resolveCallbacks = new List<Action<PromisedT>>();
            }

            if (resolveRejectables == null)
            {
                resolveRejectables = new List<IRejectable>();
            }

            resolveCallbacks.Add(onResolved);
            resolveRejectables.Add(rejectable);
        }

        /// <summary>
        /// Add a progress handler for this promise.
        /// </summary>
        private void AddProgressHandler(Action<float> onProgress, IRejectable rejectable)
        {
            if (progressHandlers == null)
            {
                progressHandlers = new List<ProgressHandler>();
            }

            progressHandlers.Add(new ProgressHandler { callback = onProgress, rejectable = rejectable });
        }

        /// <summary>
        /// Invoke a single handler.
        /// </summary>
        private void InvokeHandler<T>(Action<T> callback, IRejectable rejectable, T value)
        {
            try
            {
                callback(value);
            }
            catch (Exception ex)
            {
                EventsReceiver.OnException(ex);
                rejectable.RejectSilent(ex);
            }
        }

        /// <summary>
        /// Helper function clear out all handlers after resolution or rejection.
        /// </summary>
        private void ClearHandlers()
        {
            rejectHandlers = null;
            resolveCallbacks = null;
            resolveRejectables = null;
            cancelHandlers = null;
            progressHandlers = null;
        }

        /// <summary>
        /// Invoke all reject handlers.
        /// </summary>
        private void InvokeRejectHandlers(Exception ex)
        {
            if (rejectHandlers != null)
            {
                for (int i = 0, maxI = rejectHandlers.Count; i < maxI; ++i)
                    InvokeHandler(rejectHandlers[i].callback, rejectHandlers[i].rejectable, ex);
            }

            ClearHandlers();
        }

        /// <summary>
        /// Invoke all resolve handlers.
        /// </summary>
        private void InvokeResolveHandlers(PromisedT value)
        {
            if (resolveCallbacks != null)
            {
                for (int i = 0, maxI = resolveCallbacks.Count; i < maxI; i++) {
                    InvokeHandler(resolveCallbacks[i], resolveRejectables[i], value);
                }
            }

            ClearHandlers();
        }
        
        /// <summary>
        /// Invoke all cancel handlers.
        /// </summary>
        private void InvokeCancelHandlers()
        {
            cancelHandlers?.Each(handler => InvokeCancelHandler(handler.callback, handler.rejectable));

            ClearHandlers();
        }

        /// <summary>
        /// Invoke a single cancel handler.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="rejectable"></param>
        private void InvokeCancelHandler(Action callback, IRejectable rejectable)
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                rejectable.RejectSilent(ex);
            }
        }

        /// <summary>
        /// Invoke all progress handlers.
        /// </summary>
        private void InvokeProgressHandlers(float progress)
        {
            if (progressHandlers != null)
            {
                for (int i = 0, maxI = progressHandlers.Count; i < maxI; ++i)
                    InvokeHandler(progressHandlers[i].callback, progressHandlers[i].rejectable, progress);
            }
        }

        /// <summary>
        /// Reject the promise with an exception. Calls OnError.
        /// </summary>
        public void Reject(Exception ex)
        {
            if (ex != null)
            {
                EventsReceiver.OnException(ex);
            }
            else
            {
                EventsReceiver.OnWarningMinor("Rejecting promise with null exception");
            }

            RejectSilent(ex);
        }
        
        /// <summary>
        /// Reject the promise with an exception. Doesn't call OnError.
        /// </summary>
        public void RejectSilent(Exception ex)
        {
            if (CurState != PromiseState.Pending)
            {
                EventsReceiver.OnStateException(new PromiseStateException(
                    "Attempt to reject a promise that is already in state: " + CurState 
                                                                             + ", a promise can only be rejected when it is still in state: " 
                                                                             + PromiseState.Pending
                ));
                return;
            }

            rejectionException = ex;
            CurState = PromiseState.Rejected;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeRejectHandlers(ex);
        }
        
        /// <summary>
        /// Cancels sequence of promises from the first pending parent towards this promise.
        /// </summary>
        public void Cancel()
        {
            var sequence = this.GetCancelSequenceFromParentToThis();
            foreach (var cancelable in sequence)
            {
                cancelable.CancelSelf();
            }
        }
        
        /// <summary>
        /// Cancels only current promise. In most cases it is not what you might think.
        /// </summary>
        public void CancelSelf()
        {
            if (CurState != PromiseState.Pending)
            {
                return;
            }

            CurState = PromiseState.Cancelled;
            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }
            InvokeCancelHandlers();
            ClearHandlers();
        }
        
        /// <summary>
        /// Cancels self and all of its children no matter of the state of their parents.
        /// </summary>
        public void CancelSelfAndAllChildren()
        {
            var sequence = this.CollectSelfAndAllPendingChildren();
            
            foreach (var cancelable in sequence)
            {
                cancelable.CancelSelf();
            }
        }

        public bool TryResolve(PromisedT value)
        {
            if (CurState != PromiseState.Pending)
            {
                return false;
            }
            
            Resolve(value);
            return true;
        }

        /// <summary>
        /// Resolve the promise with a particular value.
        /// </summary>
        public void Resolve(PromisedT value)
        {
            if (CurState != PromiseState.Pending)
            {
                EventsReceiver.OnStateException(new PromiseStateException(
                    "Attempt to resolve a promise that is already in state: " + CurState 
                    + ", a promise can only be resolved when it is still in state: " 
                    + PromiseState.Pending
                ));
                return;
            }

            resolveValue = value;
            CurState = PromiseState.Resolved;

            if (Promise.EnablePromiseTracking)
            {
                Promise.PendingPromises.Remove(this);
            }

            InvokeResolveHandlers(value);
        }

        /// <summary>
        /// Report progress on the promise.
        /// </summary>
        public void ReportProgress(float progress)
        {
            if (CurState != PromiseState.Pending)
            {
                EventsReceiver.OnStateException(new PromiseStateException(
                    "Attempt to report progress on a promise that is already in state: " 
                    + CurState + ", a promise can only report progress when it is still in state: " 
                    + PromiseState.Pending
                ));
                return;
            }

            InvokeProgressHandlers(progress);
        }

        /// <summary>
        /// Completes the promise. 
        /// onResolved is called on successful completion.
        /// onRejected is called on error.
        /// </summary>
        public void Done(Action<PromisedT> onResolved, Action<Exception> onRejected)
        {
            Then(onResolved, onRejected)
                .Catch(ex =>
                    Promise.PropagateUnhandledException(this, ex)
                );
        }

        /// <summary>
        /// Completes the promise. 
        /// onResolved is called on successful completion.
        /// Adds a default error handler.
        /// </summary>
        public void Done(Action<PromisedT> onResolved)
        {
            Then(onResolved)
                .Catch(ex =>
                    Promise.PropagateUnhandledException(this, ex)
                );
        }

        /// <summary>
        /// Complete the promise. Adds a default error handler.
        /// </summary>
        public void Done()
        {
            if (CurState == PromiseState.Resolved)
                return;

            Catch(ex =>
                Promise.PropagateUnhandledException(this, ex)
            );
        }

        /// <summary>
        /// Set the name of the promise, useful for debugging.
        /// </summary>
        public IPromise<PromisedT> WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <summary>
        /// Handle errors for the promise. 
        /// </summary>
        public IPromise Catch(Action<Exception> onRejected)
        {
            if (CurState == PromiseState.Resolved)
            {
                return Promise.Resolved();
            }

            var resultPromise = new Promise();
            resultPromise.WithName(Name);
            resultPromise.AttachParent(this);

            Action<PromisedT> resolveHandler = _ => resultPromise.Resolve();

            Action<Exception> rejectHandler = ex =>
            {
                try
                {
                    onRejected(ex);
                    resultPromise.Resolve();
                }
                catch(Exception cbEx)
                {
                    resultPromise.RejectSilent(cbEx);
                }
            };

            Action cancelHandler = () => resultPromise.Cancel();

            ActionHandlers(resultPromise, resolveHandler, rejectHandler, cancelHandler);
            ProgressHandlers(resultPromise, v => resultPromise.ReportProgress(v));

            return resultPromise;
        }

        /// <summary>
        /// Handle errors for the promise.
        /// </summary>
        public IPromise<PromisedT> Catch(Func<Exception, PromisedT> onRejected)
        {
            if (CurState == PromiseState.Resolved)
            {
                return this;
            }

            var resultPromise = new Promise<PromisedT>();
            resultPromise.WithName(Name);
            resultPromise.AttachParent(this);

            Action<PromisedT> resolveHandler = v => resultPromise.Resolve(v);

            Action<Exception> rejectHandler = ex =>
            {
                try
                {
                    resultPromise.Resolve(onRejected(ex));
                }
                catch (Exception cbEx)
                {
                    resultPromise.RejectSilent(cbEx);
                }
            };
            
            Action cancelHandler = () => resultPromise.Cancel();

            ActionHandlers(resultPromise, resolveHandler, rejectHandler, cancelHandler);
            ProgressHandlers(resultPromise, v => resultPromise.ReportProgress(v));

            return resultPromise;
        }

        /// <summary>
        /// Handle cancel for the promise
        /// </summary>
        /// <param name="onCancel"></param>
        public void OnCancel(Action onCancel)
        {
            ActionHandlers(this, v => { }, ex => { }, onCancel);
        }

        /// <summary>
        /// Add a resolved callback that chains a value promise (optionally converting to a different value type).
        /// </summary>
        public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved)
        {
            return Then(onResolved, null, null);
        }

        /// <summary>
        /// Add a resolved callback that chains a non-value promise.
        /// </summary>
        public IPromise Then(Func<PromisedT, IPromise> onResolved)
        {
            return Then(onResolved, null, null);
        }

        /// <summary>
        /// Add a resolved callback.
        /// </summary>
        public IPromise Then(Action<PromisedT> onResolved)
        {
            return Then(onResolved, null, null);
        }

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// The resolved callback chains a value promise (optionally converting to a different value type).
        /// </summary>
        public IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved,
            Func<Exception, IPromise<ConvertedT>> onRejected
        )
        {
            return Then(onResolved, onRejected, null);
        }

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// The resolved callback chains a non-value promise.
        /// </summary>
        public IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }

        /// <summary>
        /// Add a resolved callback and a rejected callback.
        /// </summary>
        public IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected)
        {
            return Then(onResolved, onRejected, null);
        }


        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// The resolved callback chains a value promise (optionally converting to a different value type).
        /// </summary>
        public IPromise<ConvertedT> Then<ConvertedT>(
            Func<PromisedT, IPromise<ConvertedT>> onResolved, 
            Func<Exception, IPromise<ConvertedT>> onRejected,
            Action<float> onProgress
        )
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved(resolveValue);
                }
                catch (Exception ex)
                {
                    return Promise<ConvertedT>.Rejected(ex);
                }
            }

            // This version of the function must supply an onResolved.
            // Otherwise there is no way to get the converted value to pass to the resulting promise.

            var resultPromise = new Promise<ConvertedT>();
            resultPromise.WithName(Name);
            resultPromise.AttachParent(this);

            Action<PromisedT> resolveHandler = v =>
            {
                onResolved(v)
                    .Progress(progress => resultPromise.ReportProgress(progress))
                    .Then(
                        // Should not be necessary to specify the arg type on the next line, but Unity (mono) has an internal compiler error otherwise.
                        chainedValue => resultPromise.Resolve(chainedValue),
                        ex => resultPromise.Reject(ex)
                    )
                    .OnCancel(() => resultPromise.Cancel());
            };

            Action<Exception> rejectHandler = ex =>
            {
                if (onRejected == null)
                {
                    resultPromise.Reject(ex);
                    return;
                }

                try
                {
                    onRejected(ex)
                        .Then(
                            chainedValue => resultPromise.Resolve(chainedValue),
                            callbackEx => resultPromise.RejectSilent(callbackEx)
                        );
                }
                catch (Exception callbackEx)
                {
                    resultPromise.Reject(callbackEx);
                }
            };
            
            Action cancelHandler = () => resultPromise.Cancel();

            ActionHandlers(resultPromise, resolveHandler, rejectHandler, cancelHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// The resolved callback chains a non-value promise.
        /// </summary>
        public IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    return onResolved(resolveValue);
                }
                catch (Exception ex)
                {
                    return Promise.Rejected(ex);
                }
            }

            var resultPromise = new Promise();
            resultPromise.WithName(Name);
            resultPromise.AttachParent(this);

            Action<PromisedT> resolveHandler = v =>
            {
                if (onResolved != null)
                {
                    onResolved(v)
                        .Progress(progress => resultPromise.ReportProgress(progress))
                        .Then(
                            () => resultPromise.Resolve(),
                            ex => resultPromise.Reject(ex)
                        )
                        .OnCancel(() => resultPromise.Cancel());
                }
                else
                {
                    resultPromise.Resolve();
                }
            };

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = ex =>
                {
                    onRejected(ex);
                    resultPromise.RejectSilent(ex);
                };
            }
            else
            {
                rejectHandler = resultPromise.Reject;
            }
            
            Action cancelHandler = () => resultPromise.Cancel();

            ActionHandlers(resultPromise, resolveHandler, rejectHandler, cancelHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        /// <summary>
        /// Add a resolved callback, a rejected callback and a progress callback.
        /// </summary>
        public IPromise Then(Action<PromisedT> onResolved, Action<Exception> onRejected, Action<float> onProgress)
        {
            if (CurState == PromiseState.Resolved)
            {
                try
                {
                    onResolved(resolveValue);
                    return Promise.Resolved();
                }
                catch (Exception ex)
                {
                    return Promise.Rejected(ex);
                }
            }

            var resultPromise = new Promise();
            resultPromise.WithName(Name);
            resultPromise.AttachParent(this);

            Action<PromisedT> resolveHandler = v =>
            {
                if (onResolved != null)
                {
                    onResolved(v);
                }

                resultPromise.Resolve();
            };

            Action<Exception> rejectHandler;
            if (onRejected != null)
            {
                rejectHandler = ex =>
                {
                    onRejected(ex);
                    resultPromise.RejectSilent(ex);
                };
            }
            else
            {
                rejectHandler = resultPromise.Reject;
            }
            
            Action cancelHandler = () => resultPromise.Cancel();

            ActionHandlers(resultPromise, resolveHandler, rejectHandler, cancelHandler);
            if (onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }

            return resultPromise;
        }

        /// <summary>
        /// Return a new promise with a different value.
        /// May also change the type of the value.
        /// </summary>
        public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform)
        {
            return Then(value => Promise<ConvertedT>.Resolved(transform(value)));
        }

        /// <summary>
        /// Helper function to invoke or register resolve/reject handlers.
        /// </summary>
        private void ActionHandlers(IRejectable resultPromise, Action<PromisedT> resolveHandler, 
            Action<Exception> rejectHandler, Action cancelHandler)
        {
            if (CurState == PromiseState.Resolved)
            {
                InvokeHandler(resolveHandler, resultPromise, resolveValue);
            }
            else if (CurState == PromiseState.Rejected)
            {
                InvokeHandler(rejectHandler, resultPromise, rejectionException);
            }
            else if (CurState == PromiseState.Cancelled)
            {
                InvokeCancelHandler(cancelHandler, resultPromise);
            }
            else
            {
                AddResolveHandler(resolveHandler, resultPromise);
                AddRejectHandler(rejectHandler, resultPromise);
                AddCancelHandler(cancelHandler, resultPromise);
            }
        }

        /// <summary>
        /// Helper function to invoke or register progress handlers.
        /// </summary>
        private void ProgressHandlers(IRejectable resultPromise, Action<float> progressHandler)
        {
            if (CurState == PromiseState.Pending)
            {
                AddProgressHandler(progressHandler, resultPromise);
            }
        }

        /// <summary>
        /// Chain a number of operations using promises.
        /// Returns the value of the first promise that resolves, or otherwise the exception thrown by the last operation.
        /// </summary>
        public static IPromise<T> First<T>(params Func<IPromise<T>>[] fns)
        {
            return First((IEnumerable<Func<IPromise<T>>>)fns);
        }

        /// <summary>
        /// Chain a number of operations using promises.
        /// Returns the value of the first promise that resolves, or otherwise the exception thrown by the last operation.
        /// </summary>
        public static IPromise<T> First<T>(IEnumerable<Func<IPromise<T>>> fns)
        {
            var promise = new Promise<T>();

            int count = 0;

            fns.Aggregate(
                Promise<T>.Rejected(null),
                (prevPromise, fn) =>
                {
                    int itemSequence = count;
                    ++count;

                    var newPromise = new Promise<T>();
                    prevPromise
                        .Progress(v =>
                        {
                            var sliceLength = 1f / count;
                            promise.ReportProgress(sliceLength * (v + itemSequence));
                        })
                        .Then((Action<T>)newPromise.Resolve)
                        .Catch(ex =>
                        {
                            var sliceLength = 1f / count;
                            promise.ReportProgress(sliceLength * itemSequence);

                            fn()
                                .Then(value => newPromise.Resolve(value))
                                .Catch(newPromise.Reject)
                                .Done()
                            ;
                        })
                    ;
                    return newPromise;
                })
            .Then(value => promise.Resolve(value))
            .Catch(ex =>
            {
                promise.ReportProgress(1f);
                promise.Reject(ex);
            });

            return promise;
        }

        /// <summary>
        /// Chain an enumerable of promises, all of which must resolve.
        /// Returns a promise for a collection of the resolved results.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(value => Promise<ConvertedT>.All(chain(value)));
        }

        /// <summary>
        /// Chain an enumerable of promises, all of which must resolve.
        /// Converts to a non-value promise.
        /// The resulting promise is resolved when all of the promises have resolved.
        /// It is rejected as soon as any of the promises have been rejected.
        /// </summary>
        public IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain)
        {
            return Then(value => Promise.All(chain(value)));
        }

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise<IEnumerable<PromisedT>> All(params IPromise<PromisedT>[] promises)
        {
            return All((IEnumerable<IPromise<PromisedT>>)promises); // Cast is required to force use of the other All function.
        }

        /// <summary>
        /// Returns a promise that resolves when all of the promises in the enumerable argument have resolved.
        /// Returns a promise of a collection of the resolved results.
        /// </summary>
        public static IPromise<IEnumerable<PromisedT>> All(IEnumerable<IPromise<PromisedT>> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                return Promise<IEnumerable<PromisedT>>.Resolved(Enumerable.Empty<PromisedT>());
            }

            var remainingCount = promisesArray.Length;
            var results = new PromisedT[remainingCount];
            var progress = new float[remainingCount];
            var resultPromise = new Promise<IEnumerable<PromisedT>>();
            resultPromise.WithName("All");

            promisesArray.Each((promise, index) =>
            {
                promise.AttachParent(resultPromise);
                promise.OnCancel(resultPromise.Cancel);
                
                promise
                    .Progress(v =>
                    {
                        progress[index] = v;
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.ReportProgress(progress.Average());
                        }
                    })
                    .Then(result =>
                    {
                        progress[index] = 1f;
                        results[index] = result;

                        --remainingCount;
                        if (remainingCount <= 0 && resultPromise.CurState == PromiseState.Pending)
                        {
                            // This will never happen if any of the promises errorred.
                            resultPromise.Resolve(results);
                        }
                    })
                    .Catch(ex =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // If a promise errored and the result promise is still pending, reject it.
                            resultPromise.RejectSilent(ex);
                        }
                    })
                    .Done();
            });

            return resultPromise;
        }

        /// <summary>
        /// Takes a function that yields an enumerable of promises.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// </summary>
        public IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
        {
            return Then(value => Promise<ConvertedT>.Race(chain(value)));
        }

        /// <summary>
        /// Takes a function that yields an enumerable of promises.
        /// Converts to a non-value promise.
        /// Returns a promise that resolves when the first of the promises has resolved.
        /// Yields the value from the first promise that has resolved.
        /// </summary>
        public IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain)
        {
            return Then(value => Promise.Race(chain(value)));
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        public static IPromise<PromisedT> Race(params IPromise<PromisedT>[] promises)
        {
            return Race((IEnumerable<IPromise<PromisedT>>)promises); // Cast is required to force use of the other function.
        }

        /// <summary>
        /// Returns a promise that resolves when the first of the promises in the enumerable argument have resolved.
        /// Returns the value from the first promise that has resolved.
        /// </summary>
        public static IPromise<PromisedT> Race(IEnumerable<IPromise<PromisedT>> promises)
        {
            var promisesArray = promises.ToArray();
            if (promisesArray.Length == 0)
            {
                var ex = new InvalidOperationException(
                    "At least 1 input promise must be provided for Race"
                );
                EventsReceiver.OnException(ex);
                return Rejected(ex);
            }

            var resultPromise = new Promise<PromisedT>();
            resultPromise.WithName("Race");

            var progress = new float[promisesArray.Length];

            promisesArray.Each((promise, index) =>
            {
                promise
                    .Progress(v =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            progress[index] = v;
                            resultPromise.ReportProgress(progress.Max());
                        }
                    })
                    .Then(result =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            resultPromise.Resolve(result);
                        }
                    })
                    .Catch(ex =>
                    {
                        if (resultPromise.CurState == PromiseState.Pending)
                        {
                            // If a promise errored and the result promise is still pending, reject it.
                            resultPromise.RejectSilent(ex);
                        }
                    })
                    .Done();
            });

            return resultPromise;
        }

        /// <summary>
        /// Convert a simple value directly into a resolved promise.
        /// </summary>
        public static IPromise<PromisedT> Resolved(PromisedT promisedValue)
        {
            var promise = new Promise<PromisedT>(PromiseState.Resolved);
            promise.resolveValue = promisedValue;
            return promise;
        }

        /// <summary>
        /// Convert an exception directly into a rejected promise.
        /// </summary>
        public static IPromise<PromisedT> Rejected(Exception ex)
        {
            var promise = new Promise<PromisedT>(PromiseState.Rejected);
            promise.rejectionException = ex;
            return promise;
        }
        
        /// <summary>
        /// Convert a simple value directly into a canceled promise.
        /// </summary>
        public static IPromise<PromisedT> Canceled()
        {
            return new Promise<PromisedT>(PromiseState.Cancelled);
        }

        public void Finally(Action onComplete)
        {
            Promise promise = new Promise();
            promise.WithName(Name);
            promise.AttachParent(this);

            this.Then((x) => { promise.Resolve(); });
            this.Catch((e) => { promise.Resolve(); });
            this.OnCancel(() => promise.Resolve());

            promise.Then(onComplete);
        }

        public IPromise ContinueWith(Func<IPromise> onComplete)
        {
            var promise = new Promise();
            promise.WithName(Name);
            promise.AttachParent(this);

            this.Then(x => promise.Resolve());
            this.Catch(e => promise.Resolve());

            return promise.Then(onComplete);
        }

        public IPromise<ConvertedT> ContinueWith<ConvertedT>(Func<IPromise<ConvertedT>> onComplete)
        {
            var promise = new Promise();
            promise.WithName(Name);
            promise.AttachParent(this);

            this.Then(x => promise.Resolve());
            this.Catch(e => promise.Resolve());

            return promise.Then(onComplete);
        }

        public IPromise<PromisedT> Progress(Action<float> onProgress)
        {
            if (CurState == PromiseState.Pending && onProgress != null)
            {
                ProgressHandlers(this, onProgress);
            }
            return this;
        }

        public static Promise<PromisedT> FromCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            var promise = new Promise<PromisedT>();
            promise.OnCancel(cancellationTokenSource.Cancel);
            return promise;
        }

        public static IPromise<T> Canceled<T>()
        {
            var promise = new Promise<T>();
            promise.Cancel();
            return promise;
        }
    }
}
