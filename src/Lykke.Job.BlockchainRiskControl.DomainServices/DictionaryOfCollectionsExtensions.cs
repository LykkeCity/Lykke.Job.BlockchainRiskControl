using System;
using System.Collections.Generic;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    internal static class DictionaryOfCollectionsExtensions
    {
        public static void AddCollectionItem<TKey, TItem>(this IDictionary<TKey, ICollection<TItem>> dictionary, TKey collectionKey, TItem item)
        {
            if(!dictionary.TryGetValue(collectionKey, out var list))
            {
                list = new List<TItem>();
                dictionary.Add(collectionKey, list);
            }

            list.Add(item);
        }

        public static IEnumerable<TItem> GetCollectionItems<TKey, TItem>(this IDictionary<TKey, ICollection<TItem>> dictionary, TKey collectionKey)
        {
            return dictionary.TryGetValue(collectionKey, out var list)
                ? list
                : Array.Empty<TItem>();
        }
    }
}
