using System;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = true)]
    [DynamicParameterInternal]
    public sealed class DynamicParameterAttribute :
        Attribute
    {
        public DynamicParameterAttribute()
        {
        }

#if POWERSHELL_6_1
        public DynamicParameterAttribute(
            string experimentName,
            ExperimentAction experimentAction)
        {
            this.ExperimentName = experimentName;
            this.ExperimentAction = experimentAction;
        }

        public string ExperimentName { get; }

        public ExperimentAction ExperimentAction { get; }
#endif

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
    }
}
