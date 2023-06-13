
SSR.Net
============

SSR.Net makes it possible to server side render components in React 17, React 18 and Vue 3. It is open for extension, and can support other frameworks. It aims to be a performant, minimalistic way to support SSR in .Net. 
It is based on many of the ideas in ReactJS.Net: https://github.com/reactjs/React.NET.
This README is not finished yet.

## Installation
This library can be installed through NuGet. You will also need to install a JavaScriptEngineSwitcher engine and native engine libraries according to your environment. 

## Configuration and usage

The easiest way to get started is to copy or look at one of the example projects:
* For .Net Framework: https://github.com/knowit/SSR.Net/tree/main/src/SSR.Net.DotNetFramework
* For .Net 6: https://github.com/knowit/SSR.Net/tree/main/src/SSR.Net.DotNet6
* Example frontend for React 18: https://github.com/knowit/SSR.Net/tree/main/src/Frontend/React18Source
* Example frontend for React 17: https://github.com/knowit/SSR.Net/tree/main/src/Frontend/React17Source
* Example frontend for Vue 3: https://github.com/knowit/SSR.Net/tree/main/src/Frontend/Vue3Source

To build frontend, just run the build scripts in package.json.

To configure and use SSR.Net, you need to do several things:
* Build a JavaScript bundle fit for SSR
* Set up a web project
* Set up a controller and a view

We will look at how to do this for React 18 and .Net 6 in the following chapters. There are examples for React 17, React 18 and Vue 3 for both .Net Framework and .Net 6 in the code base. The examples in .Net 6 should be quite descriptive for .Net Core 2 and forward.

### Building a JavaScript bundle fit for SSR

Create a bundle with some components. For React 18, there is an example here: https://github.com/knowit/SSR.Net/tree/main/src/Frontend/React18Source

Expose the React, ReactDOMServer and ReactDOMClient libraries as global variables. Also expose your components somehow. See an example of how this is done here:
https://github.com/knowit/SSR.Net/blob/main/src/Frontend/React18Source/src/app.tsx
```
import * as React from 'react'
import * as ReactDOMServer from 'react-dom/server'
import * as ReactDOMClient from 'react-dom/client'
import FrontPage from './Components/FrontPage'
import ArticlePage from './Components/ArticlePage'

globalThis.React = React;
globalThis.ReactDOMServer = ReactDOMServer;
globalThis.ReactDOMClient = ReactDOMClient;
globalThis.Components = {FrontPage, ArticlePage};
```

For .Net 6 the resulting js bundle is build to the wwwroot folder as react18example.js: https://github.com/knowit/SSR.Net/tree/main/src/SSR.Net.DotNet6/wwwroot

### Set up a web project

An example of this is set up here: https://github.com/knowit/SSR.Net/tree/main/src/SSR.Net.DotNet6

Create an extension method for adding a React18Renderer to IoC. Here is an example:
https://github.com/knowit/SSR.Net/blob/main/src/SSR.Net.DotNet6/Services/ServiceCollectionExtensionsReact18SSR.cs

```
using JavaScriptEngineSwitcher.V8;
using SSR.Net.Services;

namespace SSR.Net.DotNet6.Services
{
    public static class ServiceCollectionExtensionsReact18SSR
    {
        public static void AddReact18Renderer(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var pool = new JavaScriptEnginePool(new V8JsEngineFactory(), config =>
                config
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "React18TextEncoderPolyfill.js"))
                    .AddScriptFile(Path.Combine(webHostEnvironment.WebRootPath, "react18example.js"))
            );
            services.AddSingleton(new React18Renderer(pool));
        }
    }
}
```

We use the V8 JavaScript engine for this example, and configure a new JavaScriptEnginePool. To make this work, you have to add some packages to your web project. See this file for reference:
https://github.com/knowit/SSR.Net/blob/main/src/SSR.Net.DotNet6/SSR.Net.DotNet6.csproj
``` 
    <PackageReference Include="JavaScriptEngineSwitcher.Core" Version="3.19.0" />
    <PackageReference Include="JavaScriptEngineSwitcher.V8" Version="3.20.10" />
    <PackageReference Include="Microsoft.ClearScript.Core" Version="7.3.7" />
    <PackageReference Include="Microsoft.ClearScript.V8.Native.win-x64" Version="7.3.7" />
    <PackageReference Include="Microsoft.ClearScript.V8.Native.win-x86" Version="7.3.7" />
```

Then we add a server side polyfill named React18TextEncoderPolyfill.js, and react18example.js (our bundle, with React 18, the components and the global variables). These script files will be run on each server side JavaScript engine during initialization.

After this is done, we call the extension method during the initialization of our program. See this file for reference: https://github.com/knowit/SSR.Net/blob/main/src/SSR.Net.DotNet6/Program.cs

```
builder.Services.AddReact18Renderer(builder.Environment);
```


### Set up a controller and a view

## Performance and scaling
In order to be ready to render components quickly, SSR.Net ensures that a configurable number of JavaScript engines are initialized with a given JavaScript bundle. There are several thresholds controlling this:
* Minimum engines: When starting or reconfiguring SSR.Net, it will make sure there are at least this number of engines in the engine pool.
* Maximum engines: The number of engines in the engine pool will never exceed this threshold.
* Standby engines: The number of unoccupied (spare) engines in the pool. 
* Maximum engine usages: A given engine can be used this number of times before it gets disposed of.

Example:

Let's say we have configured a minimum of 5 engines, a maximum of 25 engines and 3 engines on standby. Before any requests come in, there will be 5 vacant engines. 
Then 3 requests come in at the same time, occupying 3 engines. SSR.Net will see that there are only 2 engines on standby, and will initialize another engine to comply with the "3 engines on standby" requirement. 
Traffic increases, and at some point, 23 engines are occupied concurrently. SSR.Net will ensure that there are 25 engines initialized, but will not exceed this threshold, to comply with the "maximum of 25 engines" requirement.
Then, there are only 2-3 concurrent requests for a long time. The engines will eventually be exhausted and disposed of, and the number of engines sinks. 
At some point, there are only 5 engines left. The next time an engine gets exhausted, SSR.Net will dispose of it and see that there are only 4 engines left. Then SSR.Net will initialize a new engine to comply with the "minimum of 5 engines" requirement.



## Limitations when doing SSR:
* The window object does not exist. This means that your code needs to check if window is defined before using it:
```
if (typeof window !== 'undefined')
{
//Do stuff with window here
}
```
* Global variables must not be used (except the ones described above for exposing objects to SSR.Net). Each JavaScript Engine is used multiple times, so global variables will cause state to be shared between requests, meaning data can leak from one user to another.
* Some 3rd party packages may fail, if they use the window object or global variables.