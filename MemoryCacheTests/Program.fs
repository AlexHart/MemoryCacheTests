open System
open System.Runtime.Caching
open System.Threading
open CacheComputationExpressions

let keyName = "cached_date"
let cacheSecondsDuration = 3.0
let getDate() = DateTime.Now

[<EntryPoint>]
let main argv =    
    // Create cache.
    let cache = MemoryCache.Default

    printfn "Cache that expires every %f seconds" cacheSecondsDuration

    // Build the expression.
    let cached = new MemoryCacheExpression(cache, cacheSecondsDuration)

    [1..15]
    |> List.iter(fun iteration ->
        // Loop every second.
        Thread.Sleep 1000

        let date = cached {
            // It will only execute getDate if the value is not in the cache.
            let! dateFromCache = (keyName, getDate)

            // Print the cached value.
            let dateAsString = dateFromCache.ToLongTimeString() + " - " + dateFromCache.Ticks.ToString()
            printfn "%d\tNow: %s\tcached: %s" iteration (DateTime.Now.ToLongTimeString()) dateAsString

            // Using the key you could return the item in the cache.
            // But it could be outdated, so it depends on the users preference.
            // return! keyName 

            // Without the ! just return the cached value from this call.
            return dateFromCache
        }

        printfn "Returned from computation expression: %s" (date.ToLongTimeString()))
    
    0