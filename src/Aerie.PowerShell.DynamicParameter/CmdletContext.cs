using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;

using Aerie.Commons.Contracts;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class CmdletContext
    {
        [NotNull]
        private static readonly ConditionalWeakTable<Cmdlet, CmdletContext> _contexts =
            new ConditionalWeakTable<Cmdlet, CmdletContext>();

        [NotNull]
        private readonly Dictionary<string, DynamicParameter> _enabledParameters =
            new Dictionary<string, DynamicParameter>();

        [NotNull]
        public Cmdlet Cmdlet { get; }

        [NotNull]
        internal CmdletTypeDescriptor TypeDescriptor { get; }

        protected CmdletContext(
            [NotNull] Cmdlet cmdlet,
            [NotNull] Type cmdletType)
        {
            Ensures.ArgumentNotNull(cmdlet, nameof(cmdlet));
            Ensures.ArgumentNotNull(cmdletType, nameof(cmdletType));

            var typeDescriptor = CmdletTypeDescriptor.GetDescriptor(cmdletType);
            typeDescriptor.CreateParameterDescriptors();

            this.Cmdlet = cmdlet;
            this.TypeDescriptor = typeDescriptor;
        }

        [NotNull]
        public static CmdletContext GetContext<TCmdlet>(
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet
        {
            Ensures.ArgumentNotNull(cmdlet, nameof(cmdlet));

            return GetContext(cmdlet, typeof(TCmdlet));
        }

        [NotNull]
        public static CmdletContext GetContext(
            [NotNull] Cmdlet cmdlet)
        {
            Ensures.ArgumentNotNull(cmdlet, nameof(cmdlet));

            return GetContext(cmdlet, null);
        }
        
        [NotNull]
        internal static CmdletContext GetContext(
            [NotNull] Cmdlet cmdlet,
            [CanBeNull] Type cmdletType)
        {
            Ensures.ArgumentNotNull(cmdlet, nameof(cmdlet));

            if (cmdletType is null)
            {
                cmdletType = cmdlet.GetType();
            }
            else
            {
                if (!cmdletType.IsAssignableFrom(cmdlet.GetType()))
                {
                    throw new ArgumentException();
                }
            }

            var context = _contexts.GetValue(
                cmdlet,
                x => new CmdletContext(cmdlet, cmdletType));

            return context;
        }

        [NotNull]
        public DynamicParameter EnableParameter(
            [NotNull] ParameterDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            var instance = new DynamicParameter(this, descriptor);
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
        public IReadOnlyCollection<DynamicParameter> GetParameters()
        {
            var parameters = this._enabledParameters.Values.ToArray();
            return parameters;
        }

        [Pure]
        public bool TryGetParameter(
            [NotNull] string parameterName,
            out DynamicParameter parameter)
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
