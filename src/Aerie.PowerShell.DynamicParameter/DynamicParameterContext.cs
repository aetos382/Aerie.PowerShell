using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class DynamicParameterContext :
        IDynamicParameterContext
    {
        [NotNull]
        private static readonly ConditionalWeakTable<Cmdlet, DynamicParameterContext> _contexts =
            new ConditionalWeakTable<Cmdlet, DynamicParameterContext>();

        [NotNull]
        private readonly Dictionary<string, DynamicParameterInstance> _enabledParameters =
            new Dictionary<string, DynamicParameterInstance>();

        [NotNull]
        private readonly Dictionary<IDynamicParameterObjectProvider, object> _dynamicParameterObjects =
            new Dictionary<IDynamicParameterObjectProvider, object>();

        [NotNull]
        public Type CmdletType { [Pure] get; }

        public Cmdlet Cmdlet { get; }

        public DynamicParameterContext(
            [NotNull] Type cmdletType,
            [NotNull] Cmdlet cmdlet)
        {
            if (cmdletType is null)
            {
                throw new ArgumentNullException(nameof(cmdletType));
            }

            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (!cmdletType.IsInstanceOfType(cmdlet))
            {
                throw new ArgumentException();
            }

            this.CmdletType = cmdletType;
            this.Cmdlet = cmdlet;
        }

        [NotNull]
        public static DynamicParameterContext GetContext<TCmdlet>(
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var context = _contexts.GetValue(
                cmdlet,
                x => new DynamicParameterContext(typeof(TCmdlet), cmdlet));

            return context;
        }

        [NotNull]
        public DynamicParameterInstance EnableParameter(
            [NotNull] DynamicParameterDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            var instance = new DynamicParameterInstance(this, descriptor);
            this._enabledParameters.Add(descriptor.ParameterName, instance);

            return instance;
        }

/*
        public void DisableParameter(
            [NotNull] DynamicParameterDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            throw new NotImplementedException();
            // this._enabledParameters.Remove(descriptor);
        }
*/

        [Pure]
        public IReadOnlyCollection<DynamicParameterInstance> GetParameters()
        {
            var parameters = this._enabledParameters.Values.ToArray();
            return parameters;
        }

        [NotNull]
        public object GetDynamicParameterObject(
            [NotNull] IDynamicParameterObjectProvider factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!this._dynamicParameterObjects.TryGetValue(factory, out var obj))
            {
                this._dynamicParameterObjects[factory] = obj = factory.GetDynamicParameterObject(this);
            }

            return obj;
        }

        [Pure]
        public bool TryGetParameter(
            [NotNull] string parameterName,
            out DynamicParameterInstance parameter)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            bool enabled = this._enabledParameters.TryGetValue(parameterName, out parameter);
            return enabled;
        }

        [CanBeNull]
        public object GetParameterValue(
            [NotNull] string parameterName)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (!this._enabledParameters.TryGetValue(parameterName, out var i))
            {
                // parameter is not enabled
                throw new ArgumentException();
            }

            var value = i.Value;

            return value;
        }

        public void SetParameterValue(
            [NotNull] string parameterName,
            [CanBeNull] object value)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (!this._enabledParameters.TryGetValue(parameterName, out var i))
            {
                // parameter is not enabled
                throw new ArgumentException();
            }

            i.Value = value;
        }
    }
}
