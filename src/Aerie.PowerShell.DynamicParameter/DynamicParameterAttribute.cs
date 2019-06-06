using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = true)]
    [DynamicParameterInternal]
    public sealed class DynamicParameterAttribute :
        Attribute,
        IDynamicParameterDescriptorProvider
    {
        public DynamicParameterAttribute()
        {
        }

        public string ParameterSetName { [Pure] get; set; } = ParameterAttribute.AllParameterSets;

        public bool Mandatory { [Pure] get; set; }

        public int Position { [Pure] get; set; } = int.MinValue;

        public bool ValueFromPipeline { [Pure] get; set; }

        public bool ValueFromPipelineByPropertyName { [Pure] get; set; }

        public bool ValueFromRemainingArguments { [Pure] get; set; }

        public bool DontShow { [Pure] get; set; }

        public string HelpMessage { [Pure] get; set; }

        public string HelpMessageBaseName { [Pure] get; set; }

        public string HelpMessageResourceId { [Pure] get; set; }

        public DynamicParameterDescriptor GetDynamicParameterDescriptor(
            IReadOnlyList<MemberInfo> members)
        {
            Ensure.ArgumentNotNull(members, nameof(members), true);

            throw new NotImplementedException();
        }
    }
}
