﻿using System.IO;
using System.Threading.Tasks;
using MadsKristensen.EditorExtensions.Settings;

namespace MadsKristensen.EditorExtensions
{
    public abstract class CssCompilerBase : NodeExecutorBase
    {
        protected async override Task RtlVariantHandler(CompilerResult result)
        {
            if (!WESettings.Instance.Css.RtlCss || result.RtlTargetFileName == null)
                return;

            string value = PostProcessResult(result.RtlResult, result.RtlTargetFileName, result.RtlSourceFileName);

            // Write output file
            if (result.RtlTargetFileName != null && (MinifyInPlace || !File.Exists(result.RtlTargetFileName) ||
                value != await FileHelpers.ReadAllTextRetry(result.RtlTargetFileName)))
            {
                ProjectHelpers.CheckOutFileFromSourceControl(result.RtlTargetFileName);
                await FileHelpers.WriteAllTextRetry(result.RtlTargetFileName, value);
                ProjectHelpers.AddFileToProject(result.RtlSourceFileName, result.RtlTargetFileName);
            }

            // Write map file
            if (GenerateSourceMap && (!File.Exists(result.RtlMapFileName) ||
                result.RtlResultMap != await FileHelpers.ReadAllTextRetry(result.RtlMapFileName)))
            {
                ProjectHelpers.CheckOutFileFromSourceControl(result.RtlMapFileName);
                await FileHelpers.WriteAllTextRetry(result.RtlMapFileName, result.RtlResultMap);
                ProjectHelpers.AddFileToProject(result.RtlTargetFileName, result.RtlMapFileName);
            }
        }
    }
}
