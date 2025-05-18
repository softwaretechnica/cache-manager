# Caching Strategies
 
 This document provides a comparison of common caching strategies, outlining who handles data loading, fallback logic, and cache updates.
 
 | *Strategy*     | *Who loads the data on cache miss?*             | *Who owns the fallback logic?*                     | *Cache update behavior*                                |
 |------------------|---------------------------------------------------|-------------------------------------------------------|-----------------------------------------------------------|
 | *Cache-Aside*  | The application (via fallback delegate)           | Application provides fallback                         | Application sets cache manually                          |
 | *Read-Through* | The cache provider                                | Cache provider knows how to load data                 | Cache provider handles cache population                  |
 | **Write-Through**| The cache provider                                | Cache provider writes to both cache and data store    | Cache and data store are updated simultaneously          |
 | *Write-Around* | The application                                   | Application writes directly to data store             | Cache is updated only on read miss                       |
 | *Pass-Through* | The application                                   | Application fetches directly from data store          | No cache update (cache is bypassed)                      |
 
 ## Strategy Details
 
 ### Cache-Aside
 - *Who loads the data on cache miss?* The application (via fallback delegate)  
 - *Who owns the fallback logic?* Application provides fallback  
 - *Cache update behavior* Application sets cache manually  
 
 ### Read-Through
 - *Who loads the data on cache miss?* The cache provider  
 - *Who owns the fallback logic?* Cache provider knows how to load data  
 - *Cache update behavior* Cache provider handles cache population  
 
 ### Write-Through
 - *Who loads the data on cache miss?* The cache provider  
 - *Who owns the fallback logic?* Cache provider writes to both cache and data store  
 - *Cache update behavior* Cache and data store are updated simultaneously  
 
 ### Write-Around
 - *Who loads the data on cache miss?* The application  
 - *Who owns the fallback logic?* Application writes directly to data store  
 - *Cache update behavior* Cache is updated only on read miss  
 
 ### Pass-Through
 - *Who loads the data on cache miss?* The application  
 - *Who owns the fallback logic?* Application fetches directly from data store  
 - *Cache update behavior* No cache update (cache is bypassed)