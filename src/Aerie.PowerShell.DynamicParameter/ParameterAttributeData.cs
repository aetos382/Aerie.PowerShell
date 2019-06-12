using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ParameterAttributeData :
        CustomAttributeData
    {
        public ParameterAttributeData(
            [NotNull] ParameterAttribute parameterAttribute)
        {
            if (parameterAttribute is null)
            {
                throw new ArgumentNullException(nameof(parameterAttribute));
            }
            
            if (!string.IsNullOrEmpty(parameterAttribute.ExperimentName) &&
                parameterAttribute.ExperimentAction != ExperimentAction.None)
            {
                this.Constructor = _experimentalConstructor;

                this.ConstructorArguments.Add(new CustomAttributeTypedArgument(parameterAttribute.ExperimentName));
                this.ConstructorArguments.Add(new CustomAttributeTypedArgument(parameterAttribute.ExperimentAction));
            }
            else
            {
                this.Constructor = _defaultConstructor;
            }

            if (parameterAttribute.DontShow)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _dontShow, true));
            }

            if (!string.IsNullOrEmpty(parameterAttribute.HelpMessage))
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _helpMessage, parameterAttribute.HelpMessage));
            }

            if (!string.IsNullOrEmpty(parameterAttribute.HelpMessageBaseName) &&
                !string.IsNullOrEmpty(parameterAttribute.HelpMessageResourceId))
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _helpMessageBaseName, parameterAttribute.HelpMessageBaseName));

                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _helpMessageResourceId, parameterAttribute.HelpMessageResourceId));
            }

            if (parameterAttribute.Mandatory)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _mandatory, true));
            }

            if (!string.Equals(
                parameterAttribute.ParameterSetName,
                ParameterAttribute.AllParameterSets,
                StringComparison.OrdinalIgnoreCase))
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _parameterSetName, parameterAttribute.ParameterSetName));
            }

            if (parameterAttribute.Position != int.MinValue)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _position, parameterAttribute.Position));
            }

            if (parameterAttribute.ValueFromPipeline)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _valueFromPipeline, true));
            }

            if (parameterAttribute.ValueFromPipelineByPropertyName)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _valueFromPipelineByPropertyName, true));
            }

            if (parameterAttribute.ValueFromRemainingArguments)
            {
                this.NamedArguments.Add(new CustomAttributeNamedArgument(
                    _valueFromRemainingArguments, true));
            }
        }

        public ParameterAttributeData(
            [NotNull] CustomAttributeData attributeData)
        {
            if (attributeData is null)
            {
                throw new ArgumentNullException(nameof(attributeData));
            }

            var type = typeof(ParameterAttribute);

            foreach (var na in attributeData.NamedArguments)
            {
                MemberInfo member;

                switch (na.MemberName)
                {
                    case nameof(ParameterAttribute.DontShow):
                        member = _dontShow;
                        break;

                    case nameof(ParameterAttribute.HelpMessage):
                        member = _helpMessage;
                        break;

                    case nameof(ParameterAttribute.HelpMessageBaseName):
                        member = _helpMessageBaseName;
                        break;

                    case nameof(ParameterAttribute.HelpMessageResourceId):
                        member = _helpMessageResourceId;
                        break;

                    case nameof(ParameterAttribute.Mandatory):
                        member = _mandatory;
                        break;

                    case nameof(ParameterAttribute.ParameterSetName):
                        member = _parameterSetName;
                        break;

                    case nameof(ParameterAttribute.Position):
                        member = _position;
                        break;

                    case nameof(ParameterAttribute.ValueFromPipeline):
                        member = _valueFromPipeline;
                        break;

                    case nameof(ParameterAttribute.ValueFromPipelineByPropertyName):
                        member = _valueFromPipelineByPropertyName;
                        break;

                    case nameof(ParameterAttribute.ValueFromRemainingArguments):
                        member = _valueFromRemainingArguments;
                        break;

                    default:
                        member = type.GetProperty(na.MemberName);

                        if (member is null)
                        {
                            continue;
                        }

                        break;
                }

                var namedArgument = new CustomAttributeNamedArgument(member, na.TypedValue);

                this.NamedArguments.Add(namedArgument);
            }

            this.ConstructorArguments = attributeData.ConstructorArguments.ToArray();
        }

        [NotNull]
        private static readonly ConstructorInfo _defaultConstructor;

        [NotNull]
        private static readonly ConstructorInfo _experimentalConstructor;

        [NotNull]
        private static readonly PropertyInfo _dontShow;

        [NotNull]
        private static readonly PropertyInfo _helpMessage;

        [NotNull]
        private static readonly PropertyInfo _helpMessageBaseName;

        [NotNull]
        private static readonly PropertyInfo _helpMessageResourceId;

        [NotNull]
        private static readonly PropertyInfo _mandatory;

        [NotNull]
        private static readonly PropertyInfo _parameterSetName;

        [NotNull]
        private static readonly PropertyInfo _position;

        [NotNull]
        private static readonly PropertyInfo _valueFromPipeline;

        [NotNull]
        private static readonly PropertyInfo _valueFromPipelineByPropertyName;

        [NotNull]
        private static readonly PropertyInfo _valueFromRemainingArguments;

        static ParameterAttributeData()
        {
            var type = typeof(ParameterAttribute);

            _defaultConstructor = type.GetConstructor(Type.EmptyTypes);
            _experimentalConstructor = type.GetConstructor(new[] { typeof(string), typeof(ExperimentAction) });

            _dontShow = type.GetProperty(nameof(ParameterAttribute.DontShow));
            _helpMessage = type.GetProperty(nameof(ParameterAttribute.HelpMessage));
            _helpMessageBaseName = type.GetProperty(nameof(ParameterAttribute.HelpMessageBaseName));
            _helpMessageResourceId = type.GetProperty(nameof(ParameterAttribute.HelpMessageResourceId));
            _mandatory = type.GetProperty(nameof(ParameterAttribute.Mandatory));
            _parameterSetName = type.GetProperty(nameof(ParameterAttribute.ParameterSetName));
            _position = type.GetProperty(nameof(ParameterAttribute.Position));
            _valueFromPipeline = type.GetProperty(nameof(ParameterAttribute.ValueFromPipeline));
            _valueFromPipelineByPropertyName = type.GetProperty(nameof(ParameterAttribute.ValueFromPipelineByPropertyName));
            _valueFromRemainingArguments = type.GetProperty(nameof(ParameterAttribute.ValueFromRemainingArguments));
        }

        public override ConstructorInfo Constructor { get; }

        public override IList<CustomAttributeNamedArgument> NamedArguments { [Pure] get; } = new List<CustomAttributeNamedArgument>();

        public override IList<CustomAttributeTypedArgument> ConstructorArguments { [Pure] get; } = new List<CustomAttributeTypedArgument>();
    }
}
