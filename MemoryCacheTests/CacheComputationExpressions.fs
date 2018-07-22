module CacheComputationExpressions

    open System
    open System.Runtime.Caching

    // Refresh item inside the cache.
    let setCache (cache: MemoryCache) key value cacheSecondsDuration = 
        // Create expire policy.
        let policy = new CacheItemPolicy()
        policy.AbsoluteExpiration <- DateTimeOffset.Now + TimeSpan.FromSeconds(cacheSecondsDuration)

        // Set item in cache.
        cache.Set(key, value,  policy)

    /// Computation expression for accessing cache with variable key names.
    type MemoryCacheExpression(cache: MemoryCache, cacheSecondsDuration) =
        member x.Bind((key, valueFun: unit -> 'b), func: 'b -> 'b) =
            // If it's not in the cache, add it.
            if cache.[key] = null then 
                setCache cache key (valueFun()) cacheSecondsDuration

            // It will return the right type iso of obj.
            func (cache.[key] :?> 'b)
        
        /// Return just an item of the current type.
        member x.Return(value) = value

        /// Return from the cache by passing the key.
        member x.ReturnFrom(keyName) = 
            cache.[keyName] :?> 'b

    /// Computation expression for accessing cache always with the same key name.
    type SingleNameMemoryCacheExpression(cache: MemoryCache, cacheSecondsDuration) =
        let fixedKey = Guid.NewGuid().ToString()

        member x.Bind(valueFun: unit -> 'b, func: 'b -> 'b) =
            // It it's not in the cache, add it.
            if cache.[fixedKey] = null then
                setCache cache fixedKey (valueFun()) cacheSecondsDuration

            func (cache.[fixedKey] :?> 'b)

        /// Return just an item of the current type.
        member x.Return(value) = value

        /// Return from the cache by passing the key.
        member x.ReturnFrom(keyName) = cache.[keyName] :?> 'b