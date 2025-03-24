namespace VideoScreenshot.Domain.ValueObjects;

public class FileName
{
    public FileName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("File name cannot be null or empty", nameof(name));
        
        Name = name;
    }

    public string Name { get; init; }

    public override string ToString() => Name;
    public static implicit operator string(FileName fileName) => fileName.Name;
    public static implicit operator FileName(string fileName) => new(fileName);
}