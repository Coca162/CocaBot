using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord;



public class JsonList<T> : List<T>
{
    public string Filename { get; }

    public JsonList(string filename) : base() => Filename = filename;

    public JsonList(string filename, IEnumerable<T> list) : base(list) => Filename = filename;

    public new async Task Add(T item)
    {
        base.Add(item);
        Stream file = File.Create(Filename);
        await JsonSerializer.SerializeAsync(file, this, new JsonSerializerOptions { IncludeFields = true });
        file.Close();
    }

    public new async Task Remove(T item)
    {
        base.Remove(item);
        Stream file = File.Create(Filename);
        await JsonSerializer.SerializeAsync(file, this, new JsonSerializerOptions { IncludeFields = true });
        file.Close();
    }
}

public class JsonDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>
{
    public string Filename { get; }

    public JsonDictionary(string filename, IEqualityComparer<Tkey>? equalityComparer = null) : base(equalityComparer)
        => Filename = filename;

    public JsonDictionary(string filename, IDictionary<Tkey, Tvalue> dictionary, IEqualityComparer<Tkey>? equalityComparer = null) : base(dictionary, equalityComparer) 
        => Filename = filename;

    public new async Task Add(Tkey key, Tvalue value)
    {
        base.Add(key, value);
        Stream file = File.Create(Filename);
        await JsonSerializer.SerializeAsync(file, this, new JsonSerializerOptions { IncludeFields = true });
        file.Close();
    }

    public new async Task Remove(Tkey item)
    {
        base.Remove(item);
        Stream file = File.Create(Filename);
        await JsonSerializer.SerializeAsync(file, this, new JsonSerializerOptions { IncludeFields = true });
        file.Close();
    }
}