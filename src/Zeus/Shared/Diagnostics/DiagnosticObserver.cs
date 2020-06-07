using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zeus.Shared.Diagnostics
{
    public abstract class DiagnosticObserver : IObserver<DiagnosticListener>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        protected abstract bool IsMatch(string component);

        /// <inheritdoc />
        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            if (!IsMatch(diagnosticListener.Name)) 
                return;

            var subscription = diagnosticListener.SubscribeWithAdapter(this);
            _subscriptions.Add(subscription);
        }

        /// <inheritdoc />
        void IObserver<DiagnosticListener>.OnError(Exception error)
        { }

        /// <inheritdoc />
        void IObserver<DiagnosticListener>.OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }
    }
}
