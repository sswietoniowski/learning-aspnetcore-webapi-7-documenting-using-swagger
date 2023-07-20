namespace Contacts.Api.Configurations.Extensions;

// ReSharper disable once InconsistentNaming - IListExtensions is a better name than ListExtensions give we are using IList<T> as the type
public static class IListExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null)
        {
            throw new ArgumentNullException("list");
        }

        if (items == null)
        {
            throw new ArgumentNullException("items");
        }

        if (list is List<T>)
        {
            ((List<T>)list).AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}