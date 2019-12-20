using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class CmdletDynamicParameterExtensions
    {
        [NotNull]
        public static DynamicParameter EnableDynamicParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterExpression)
            where TCmdlet : Cmdlet
        {
            return EnableDynamicParameter(cmdlet, parameterExpression, null);
        }

        [NotNull]
        public static DynamicParameter EnableDynamicParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterExpression,
            [CanBeNull] IDynamicParameterAttributeProvider attributeProvider)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var members = new ParameterMemberInfo(typeof(TCmdlet), parameterExpression);
            var descriptor = ReflectParameterDescriptor.GetDescriptor(members, attributeProvider);

            var instance = EnableDynamicParameter(cmdlet, descriptor);

            return instance;
        }

        [NotNull]
        public static DynamicParameter EnableDynamicParameter<TCmdlet, TParameter>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TParameter>> parameterExpression)
            where TCmdlet : Cmdlet
        {
            return EnableDynamicParameter(cmdlet, parameterExpression, null);
        }

        [NotNull]
        public static DynamicParameter EnableDynamicParameter<TCmdlet, TParameter>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TParameter>> parameterExpression,
            [CanBeNull] IDynamicParameterAttributeProvider attributeProvider)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var members = new ParameterMemberInfo((MemberExpression)parameterExpression.Body);
            var descriptor = ReflectParameterDescriptor.GetDescriptor(members, attributeProvider);

            var instance = EnableDynamicParameter(cmdlet, descriptor);

            return instance;
        }

        [NotNull]
        public static DynamicParameter EnableDynamicParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] DynamicParameterDescriptor parameterDescriptor)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterDescriptor is null)
            {
                throw new ArgumentNullException(nameof(parameterDescriptor));
            }

            var context = CmdletContext.GetContext(cmdlet);
            var instance = context.EnableParameter(parameterDescriptor);

            return instance;
        }

/*
        public static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            bool enabled = TryGetParameter(cmdlet, parameterName, out _);
            return enabled;
        }

        private static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] DynamicParameterDescriptor descriptor)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }

        public static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterExpression)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var descriptor = DynamicParameterDescriptor.GetParameterDescriptor(typeof(TCmdlet), parameterExpression);

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }

        public static bool IsParameterEnabled<TCmdlet, TParameter>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TParameter>> parameterExpression)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var descriptor = DynamicParameterDescriptor.GetParameterDescriptor(parameterExpression);

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }
*/

        [NotNull]
        public static object GetDynamicParameterObject<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet
        {
            return GetDynamicParameterObject(cmdlet, null);
        }

        [NotNull]
        public static object GetDynamicParameterObject<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] IDynamicParameterObjectBuilder provider)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (provider is null)
            {
                var providerAttribute = typeof(TCmdlet)
                    .GetCustomAttribute<DynamicParameterObjectProviderAttribute>(true);

                if (providerAttribute is null)
                {
                    provider = DynamicProxyProvider.Instance;
                }
                else
                {
                    var providerType = providerAttribute.ProviderType;

                    provider = (IDynamicParameterObjectBuilder)Activator.CreateInstance(providerType);

                    Debug.Assert(!(provider is null));
                }
            }

            var context = CmdletContext.GetContext(cmdlet);
            
            var dynamicParameterObject = provider.GetDynamicParameterObject(context);

            return dynamicParameterObject;
        }

        public static bool TryGetParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName,
            [NotNull] out DynamicParameter parameter)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            var context = CmdletContext.GetContext(cmdlet);

            bool enabled = context.TryGetParameter(parameterName, out parameter);
            return enabled;
        }

        public static bool TryGetParameterValue<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName,
            [CanBeNull] out object parameterValue)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            parameterValue = default;

            bool enabled = TryGetParameter(cmdlet, parameterName, out var parameter);
            if (!enabled)
            {
                return false;
            }

            parameterValue = parameter.Value;
            return parameter.IsSet;
        }

        [Pure]
        public static bool IsParameterSet<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            bool enabled = TryGetParameter(cmdlet, parameterName, out var parameter);
            if (!enabled)
            {
                return false;
            }

            return parameter.IsSet;
        }

        public static void PopulateRuntimeDefinedParameterValues<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] RuntimeDefinedParameterDictionary runtimeDefinedParameters)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (runtimeDefinedParameters is null)
            {
                throw new ArgumentNullException(nameof(runtimeDefinedParameters));
            }

            var context = CmdletContext.GetContext(cmdlet);

            foreach (var parameter in runtimeDefinedParameters.Values)
            {
                context.SetParameterValue(parameter.Name, parameter.Value);
            }
        }
    }
}
