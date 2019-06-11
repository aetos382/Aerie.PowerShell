﻿using System;
using System.Collections.Generic;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = true)]
    [NotParameter]
    public sealed class DynamicParameterAttribute :
        Attribute,
        IDynamicParameterDescriptionProvider
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

        public IEnumerable<DynamicParameterDescriptor> GetParameterDescriptors(
            ParameterMemberInfo member)
        {
            Ensure.ArgumentNotNull(member, nameof(member));

            throw new NotImplementedException();
        }
    }
}
