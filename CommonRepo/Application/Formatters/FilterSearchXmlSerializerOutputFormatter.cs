using FilterBuilder;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace CommonRepo.Application.Formatters;

/// <summary>
/// Custom XmlSerializerOutputFormatter that ignores the NullValue property
/// from FilterBuilder.FilterSearch when creating XmlSerializer instances.
/// The NullValue property is of type IEnumerable&lt;(Type, object)&gt; which
/// cannot be serialized by XmlSerializer because it is an interface with ValueTuple elements.
/// </summary>
public class FilterSearchXmlSerializerOutputFormatter : XmlSerializerOutputFormatter
{
    private static readonly XmlAttributeOverrides _overrides = CreateOverrides();
    private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializerCache = new();

    private static XmlAttributeOverrides CreateOverrides()
    {
        var overrides = new XmlAttributeOverrides();
        var ignoreAttribute = new XmlAttributes { XmlIgnore = true };
        overrides.Add(typeof(FilterSearch), nameof(FilterSearch.NullValue), ignoreAttribute);
        return overrides;
    }

    protected override XmlSerializer CreateSerializer(Type type)
    {
        return _serializerCache.GetOrAdd(type, t =>
        {
            try
            {
                return new XmlSerializer(t, _overrides);
            }
            catch
            {
                // Fall back to the base implementation if the overrides cause issues
                // for types that don't involve FilterSearch
                return base.CreateSerializer(t);
            }
        });
    }
}
