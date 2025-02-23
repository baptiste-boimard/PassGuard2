using Blazored.LocalStorage;

namespace Tests.MockService;

public class MockLocalStorage : ILocalStorageService
{
    public ValueTask ClearAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = new CancellationToken())
    {
        if (key == "token")
        {
            object token = "fake-token";
            return new ValueTask<T?>((T?)token);
        }
        return new ValueTask<T?>(default(T));
    }

    public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> LengthAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public event EventHandler<ChangingEventArgs>? Changing;
    public event EventHandler<ChangedEventArgs>? Changed;
}