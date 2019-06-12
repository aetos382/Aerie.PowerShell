﻿using System;
using System.Management.Automation;

using Aerie.PowerShell.DynamicParameter;

namespace Aerie.PowerShell.Samples
{
    [DynamicParameterObjectProvider(typeof(RuntimeDefinedParametersProvider))]
    [Cmdlet(VerbsDiagnostic.Test, "DynamicParameter")]
    public class TestDynamicParameterCommand :
        Cmdlet,
        IDynamicParameters
    {
        public object GetDynamicParameters()
        {
            var dynamicParams = this.GetDynamicParameterObject();
            return dynamicParams;
        }
    }
}
