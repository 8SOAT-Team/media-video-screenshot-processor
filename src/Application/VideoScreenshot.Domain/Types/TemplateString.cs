namespace VideoScreenshot.Domain.Types;

public struct TemplateString
{
    public TemplateString(string template)
    {
        Template = template;
    }

    private string Template { get; }

    public readonly string Format(params object?[] values) => string.Format(Template, values);
    
    
    public override string ToString()
    {
        return Template;
    }

    public static implicit operator string(TemplateString templateString)
    {
        return templateString.Template;
    }

    public static implicit operator TemplateString(string value)
    {
        return new TemplateString(value);
    }
}