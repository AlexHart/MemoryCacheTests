open System
open System.Runtime.Caching
open System.Threading

let keyName = "cached_date"

let cacheSecondsDuration = 3.0

let getDate() = DateTime.Now

// Refresh item inside the cache.
let setCache (cache: MemoryCache) = 
    // Create expire policy.
    let policy = new CacheItemPolicy()
    policy.AbsoluteExpiration <- DateTimeOffset.Now + TimeSpan.FromSeconds(cacheSecondsDuration)

    // Set item in cache.
    cache.Set(keyName, getDate(),  policy) |> ignore

[<EntryPoint>]
let main argv =    
    let cache = MemoryCache.Default

    printfn "Cache that expires every %f seconds" cacheSecondsDuration

    [1..15]
    |> List.iter(fun iteration ->
        Thread.Sleep 1000
        
        // Check that the item is not in the cache and add it.
        if (cache.[keyName] = null) then
            setCache cache
        
        // Get the item from the cache and print it.
        let cacheContents = cache.[keyName]
        let date: DateTime = Convert.ToDateTime(cacheContents)
        let dateAsString = date.ToLongTimeString() + " - " + date.Ticks.ToString()

        printfn "%d\t%s\tcached: %s" iteration (DateTime.Now.ToLongTimeString()) dateAsString)
        
    0