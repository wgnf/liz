using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Projects.Contracts.Models;

[ExcludeFromCodeCoverage] // DTO
internal sealed class ProjectReference : IEquatable<ProjectReference>
{
    private readonly List<ProjectReference> _projectReferences = new();

    public ProjectReference(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        Name = name;
    }

    public string Name { get; }

    public IEnumerable<ProjectReference> ProjectReferences => _projectReferences;

    public void AddReference(ProjectReference projectReference)
    {
        if (projectReference == null) throw new ArgumentNullException(nameof(projectReference));

        if (_projectReferences.Contains(projectReference)) return;
        
        _projectReferences.Add(projectReference);
    }

    public bool Equals(ProjectReference? other)
    {
        if (ReferenceEquals(null, other)) return false;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (ReferenceEquals(this, other)) return true;
        
        return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ProjectReference other && Equals(other));
    }

    public override int GetHashCode()
    {
        return StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name);
    }

    public static bool operator ==(ProjectReference? left, ProjectReference? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProjectReference? left, ProjectReference? right)
    {
        return !Equals(left, right);
    }
}
