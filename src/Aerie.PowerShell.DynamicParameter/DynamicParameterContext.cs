using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
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
        private static readonly Dictionary<Type, ReflectParameterDescriptor[]> _staticCompoundParameterCache =
            new Dictionary<Type, ReflectParameterDescriptor[]>();

        [NotNull]
        private readonly Dictionary<string, DynamicParameterInstance> _enabledParameters =
            new Dictionary<string, DynamicParameterInstance>();

        [NotNull]
        private readonly Dictionary<IDynamicParameterObjectBuilder, object> _dynamicParameterObjects =
            new Dictionary<IDynamicParameterObjectBuilder, object>();

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

            var context = _contexts.GetValue(cmdlet, x => new DynamicParameterContext(typeof(TCmdlet), cmdlet));
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

        private bool _compoundParametersProcessed = false;

        [NotNull]
        public object GetDynamicParameterObject(
            [NotNull] IDynamicParameterObjectBuilder factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!this._compoundParametersProcessed)
            {
                this.ProcessCompoundParameters();

                this._compoundParametersProcessed = true;
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

            this._interceptor.GetValue(this, i.ParameterDescriptor, ref value);

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

            this._interceptor.SetValue(this, i.ParameterDescriptor, ref value);

            i.Value = value;
        }

        [NotNull]
        private IParameterInterceptor _interceptor = DefaultInterceptor.Instance;

        public void SetParameterInterceptor(
            [NotNull] IParameterInterceptor interceptor)
        {
            this._interceptor = interceptor;
        }

        private void ProcessCompoundParameters()
        {
            var staticDescriptors = new List<ReflectParameterDescriptor>();

            bool staticParameterCached = _staticCompoundParameterCache.TryGetValue(this.CmdletType, out var cachedDescriptors);
            if (staticParameterCached)
            {
                staticDescriptors.AddRange(cachedDescriptors);
            }
            
            var compoundMembers = Utilities.GetParameterMembers(this.CmdletType);

            foreach (var compoundMember in compoundMembers)
            {
                var memberType = Utilities.GetMemberType(compoundMember);

                if (!staticParameterCached && (
                    Attribute.IsDefined(compoundMember, typeof(CompoundParameterAttribute), true) ||
                    Attribute.IsDefined(memberType, typeof(CompoundParameterAttribute), true)))
                {
                    var newDescriptors = this.ProcessNonDynamicCompoundParameter(compoundMember, memberType);
                    staticDescriptors.AddRange(newDescriptors);
                }
                else if (typeof(ICompoundParameter).IsAssignableFrom(memberType))
                {
                    var compoundParameter = (ICompoundParameter)Utilities.GetMemberValue(this.Cmdlet, compoundMember);

                    if (compoundParameter is null)
                    {
                        continue;
                    }

                    this.ProcessDynamicCompoundParameter(compoundMember, compoundParameter);
                }
            }

            foreach (var descriptor in staticDescriptors)
            {
                this.EnableParameter(descriptor);
            }

            if (!staticParameterCached)
            {
                _staticCompoundParameterCache[this.CmdletType] = staticDescriptors.ToArray();
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ReflectParameterDescriptor> ProcessNonDynamicCompoundParameter(
            [NotNull] MemberInfo compoundMember,
            [NotNull] Type compoundParameterType)
        {
            var parameterMembers = Utilities.GetParameterMembers(compoundParameterType);

            foreach (var parameterMember in parameterMembers)
            {
                if (!Attribute.IsDefined(parameterMember, typeof(ParameterAttribute), true))
                {
                    continue;
                }

                var descriptor = ReflectParameterDescriptor.GetParameterDescriptor(
                    new[] { compoundMember, parameterMember });

                yield return descriptor;
            }
        }

        private void ProcessDynamicCompoundParameter(
            [NotNull] MemberInfo compoundMember,
            [NotNull] ICompoundParameter compoundParameter)
        {
            var context = CompoundParameterContext.Register(this, compoundMember, compoundParameter);

            compoundParameter.ConfigureDynamicParameters(context);
        }
    }
}
