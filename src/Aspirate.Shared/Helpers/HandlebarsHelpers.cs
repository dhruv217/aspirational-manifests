using HandlebarsDotNet;
using Aspirate.Shared.Models.Aspirate; // Assuming KubernetesDeploymentData is here
using System; // For StringComparison

namespace Aspirate.Shared.Helpers;

/// <summary>
/// Contains custom Handlebars helper functions.
/// </summary>
public static class HandlebarsHelpers
{
    /// <summary>
    /// Handlebars block helper to conditionally render content based on the resource name.
    /// Usage: {{#IsProjectName "YourProjectName"}}...{{/IsProjectName}}
    /// </summary>
    public static void IsProjectNameHelper(EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments)
    {
        if (arguments.Length != 1 || arguments[0] is not string projectToCheck)
        {
            // Handle incorrect arguments - render the {{else}} block or nothing
            options.Inverse(writer, context);
            return;
        }

        // Check if the context is the expected type (adjust if needed based on actual context structure)
        if (context.Value is KubernetesDeploymentData data)
        {
            // Perform case-sensitive comparison
            if (data.Name?.Equals(projectToCheck, StringComparison.Ordinal) ?? false)
            {
                // Render the main block {{#IsProjectName}}...{{/IsProjectName}}
                options.Template(writer, context);
            }
            else
            {
                // Render the {{else}} block or nothing
                options.Inverse(writer, context);
            }
        }
        else
        {
            // Context is not the expected type, render inverse/nothing
            options.Inverse(writer, context);
        }
    }

    // Add other helpers here if needed in the future
}
