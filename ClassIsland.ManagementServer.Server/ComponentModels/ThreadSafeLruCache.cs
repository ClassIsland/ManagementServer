namespace ClassIsland.ManagementServer.Server.ComponentModels;

public class ThreadSafeLruCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
    private readonly LinkedList<CacheItem> _lruList;
    private readonly object _lock = new();

    public ThreadSafeLruCache(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "容量必须大于 0");

        _capacity = capacity;
        _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>();
        _lruList = new LinkedList<CacheItem>();
    }

    public void Add(TKey key, TValue value)
    {
        lock (_lock)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                // 更新并移动到头部
                node.Value.Value = value;
                _lruList.Remove(node);
                _lruList.AddFirst(node);
            }
            else
            {
                if (_cacheMap.Count >= _capacity)
                {
                    // 移除尾部最久未使用项
                    var lastNode = _lruList.Last!;
                    _cacheMap.Remove(lastNode.Value.Key);
                    _lruList.RemoveLast();
                }

                var cacheItem = new CacheItem(key, value);
                var listNode = new LinkedListNode<CacheItem>(cacheItem);
                _lruList.AddFirst(listNode);
                _cacheMap[key] = listNode;
            }
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        lock (_lock)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                // 移到链表头部
                _lruList.Remove(node);
                _lruList.AddFirst(node);
                value = node.Value.Value;
                return true;
            }

            value = default!;
            return false;
        }
    }

    public bool ContainsKey(TKey key)
    {
        lock (_lock)
        {
            return _cacheMap.ContainsKey(key);
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _cacheMap.Count;
            }
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _cacheMap.Clear();
            _lruList.Clear();
        }
    }
    
    public bool TryPop(TKey key, out TValue value)
    {
        lock (_lock)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                _lruList.Remove(node);
                _cacheMap.Remove(key);
                return true;
            }

            value = default!;
            return false;
        }
    }
    
    public bool Remove(TKey key)
    {
        lock (_lock)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                _lruList.Remove(node);
                _cacheMap.Remove(key);
                return true;
            }
            return false;
        }
    }

    private class CacheItem
    {
        public TKey Key { get; }
        public TValue Value { get; set; }

        public CacheItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
