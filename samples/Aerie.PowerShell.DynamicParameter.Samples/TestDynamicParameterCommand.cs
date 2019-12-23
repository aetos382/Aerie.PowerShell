using System;
using System.Management.Automation;

using Aerie.PowerShell.DynamicParameter;

namespace Aerie.PowerShell.Samples
{
    [DynamicParameterObjectProvider(typeof(RuntimeDefinedParametersBuilder))]
    [Cmdlet(VerbsDiagnostic.Test, "DynamicParameter")]
    public class TestDynamicParameterCommand :
        Cmdlet,
        IDynamicParameters
    {
        private readonly ICmdletContext _context;

        public TestDynamicParameterCommand()
        {
            var builder = new CmdletContextBuilder();

            this._context = builder.Build();
        }

        public object GetDynamicParameters()
        {
            var dynamicParams = this.GetDynamicParameterObject();
            return dynamicParams;
        }
    }
}
