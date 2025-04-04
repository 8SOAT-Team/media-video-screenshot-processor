using Microsoft.Extensions.Options;
using VideoScreenshot.Application.Configurations;

namespace VideoScreenshot.MessageConsumer;

[OptionsValidator]
public partial class ValidateSettingsOptions : IValidateOptions<AppConfiguration>
{
}